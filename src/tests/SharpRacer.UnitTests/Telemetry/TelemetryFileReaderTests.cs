using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using SharpRacer.Interop;
using SharpRacer.IO;
using SharpRacer.Telemetry.TestUtilities;

namespace SharpRacer.Telemetry;
public class TelemetryFileReaderTests
{
    [Fact]
    public void Ctor_ThrowOnClosedHandleTest()
    {
        var tempFile = Path.GetTempFileName();

        using (var closedHandle = File.OpenHandle(tempFile, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            closedHandle.Close();

            Assert.True(closedHandle.IsClosed);

            Assert.Throws<ArgumentException>(() =>
            {
                using var reader = new TelemetryFileReader(closedHandle);
            });
        }

        File.Delete(tempFile);
    }

    [Fact]
    public void Ctor_ThrowOnFileTooSmallForContentsTest()
    {
        DataVariableHeader intVarHeader = default;
        DataVariableHeader float3ArrayVarHeader = default;

        var fileBuilder = TelemetryFileBuilder.Create(
            varBuilder =>
            {
                varBuilder.AddScalarVariable("Foo", DataVariableValueType.Int, "test", "Description", out intVarHeader);

                varBuilder.AddArrayVariable("Bar", DataVariableValueType.Float, 3, "float/s", "Float array", out float3ArrayVarHeader);
            })
            .SetSessionInfo("Test session", version: 1)
            .AddDataFrame(
                frame =>
                {
                    frame.Write<int>(intVarHeader, 12);
                    frame.WriteArray<float>(float3ArrayVarHeader, [1.0f, 2.0f, 3.0f]);
                });

        // Write the file
        var fileName = Path.GetTempFileName();

        fileBuilder.Write(fileName, out var writtenHeader);

        // Truncate the file by the size of two of our floats from the above data frame array
        using (var truncateStream = File.OpenWrite(fileName))
        {
            truncateStream.SetLength(truncateStream.Length - (Unsafe.SizeOf<float>() * 2));
        }

        // Assert
        Assert.Throws<IOException>(() =>
        {
            using var reader = new TelemetryFileReader(fileName);
        });

        File.Delete(fileName);
    }

    [Fact]
    public void Ctor_ThrowOnFileTooSmallForFileHeaderTest()
    {
        var fileName = Path.GetTempFileName();

        Assert.Throws<IOException>(() =>
        {
            using var reader = new TelemetryFileReader(fileName);
        });

        File.Delete(fileName);
    }

    [Fact]
    public void Ctor_ThrowOnInvalidHandleTest()
    {
        using var invalidHandle = new SafeFileHandle();

        Assert.True(invalidHandle.IsInvalid);

        Assert.Throws<ArgumentException>(() =>
        {
            using var reader = new TelemetryFileReader(invalidHandle);
        });
    }

    [Fact]
    public void Ctor_ThrowOnInvalidHeaderTest()
    {
        var fileBuilder = TelemetryFileBuilder.Create()
            .SetHeaderVersion(42);

        // Write the file
        var fileName = Path.GetTempFileName();

        fileBuilder.Write(fileName, out _);

        Assert.Throws<IOException>(() =>
        {
            using var reader = new TelemetryFileReader(fileName);
        });

        File.Delete(fileName);
    }

    [Fact]
    public void Ctor_ThrowOnNullHandleTest()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            using var reader = new TelemetryFileReader(fileHandle: null!);
        });
    }

    [Fact]
    public void ReadDataFrame_Test()
    {
        DataVariableHeader intVarHeader = default;
        DataVariableHeader float3ArrayVarHeader = default;

        int fooVarValue = 12;
        float[] barVarValue = [1.0f, 2.0f, 3.0f];

        var fileBuilder = TelemetryFileBuilder.Create(
            varBuilder =>
            {
                varBuilder.AddScalarVariable("Foo", DataVariableValueType.Int, "test", "Description", out intVarHeader);

                varBuilder.AddArrayVariable("Bar", DataVariableValueType.Float, 3, "float/s", "Float array", out float3ArrayVarHeader);
            })
            .AddDataFrame(
                frame =>
                {
                    frame.Write<int>(intVarHeader, fooVarValue);
                    frame.WriteArray<float>(float3ArrayVarHeader, barVarValue);
                });

        // Write the file
        var fileName = Path.GetTempFileName();

        fileBuilder.Write(fileName, out var writtenHeader);

        Span<byte> dataFrameBlob;

        using (var reader = new TelemetryFileReader(fileName))
        {
            dataFrameBlob = reader.ReadDataFrame(0);
        }

        // Read 'Foo' value
        var fooFileValue = MemoryMarshal.Read<int>(dataFrameBlob.Slice(intVarHeader.Offset, intVarHeader.GetDataLength()));
        Assert.Equal(fooVarValue, fooFileValue);

        // Read 'Bar' value
        var barFileValue = new float[3];
        dataFrameBlob.Slice(float3ArrayVarHeader.Offset, float3ArrayVarHeader.GetDataLength())
            .CopyTo(MemoryMarshal.AsBytes((Span<float>)barFileValue));

        Assert.True(barVarValue.SequenceEqual(barFileValue));

        // Cleanup
        File.Delete(fileName);
    }

    [Fact]
    public void ReadDataFrame_Span_Test()
    {
        DataVariableHeader intVarHeader = default;
        DataVariableHeader float3ArrayVarHeader = default;

        int fooVarValue = 12;
        float[] barVarValue = [1.0f, 2.0f, 3.0f];

        var fileBuilder = TelemetryFileBuilder.Create(
            varBuilder =>
            {
                varBuilder.AddScalarVariable("Foo", DataVariableValueType.Int, "test", "Description", out intVarHeader);

                varBuilder.AddArrayVariable("Bar", DataVariableValueType.Float, 3, "float/s", "Float array", out float3ArrayVarHeader);
            })
            .AddDataFrame(
                frame =>
                {
                    frame.Write<int>(intVarHeader, fooVarValue);
                    frame.WriteArray<float>(float3ArrayVarHeader, barVarValue);
                });

        // Write the file
        var fileName = Path.GetTempFileName();

        fileBuilder.Write(fileName, out var writtenHeader);

        Span<byte> dataFrameBlob;

        using (var reader = new TelemetryFileReader(fileName))
        {
            dataFrameBlob = new byte[reader.FileHeader.DataBufferElementLength];

            var bytesRead = reader.ReadDataFrame(0, dataFrameBlob);

            Assert.Equal(bytesRead, reader.FileHeader.DataBufferElementLength);
        }

        // Read 'Foo' value
        var fooFileValue = MemoryMarshal.Read<int>(dataFrameBlob.Slice(intVarHeader.Offset, intVarHeader.GetDataLength()));
        Assert.Equal(fooVarValue, fooFileValue);

        // Read 'Bar' value
        var barFileValue = new float[3];
        dataFrameBlob.Slice(float3ArrayVarHeader.Offset, float3ArrayVarHeader.GetDataLength())
            .CopyTo(MemoryMarshal.AsBytes((Span<float>)barFileValue));

        Assert.True(barVarValue.SequenceEqual(barFileValue));

        // Cleanup
        File.Delete(fileName);
    }

    [Fact]
    public void ReadDataFrame_Span_ThrowOnBufferTooSmallTest()
    {
        DataVariableHeader intVarHeader = default;
        DataVariableHeader float3ArrayVarHeader = default;

        var fileBuilder = TelemetryFileBuilder.Create(
            varBuilder =>
            {
                varBuilder.AddScalarVariable("Foo", DataVariableValueType.Int, "test", "Description", out intVarHeader);

                varBuilder.AddArrayVariable("Bar", DataVariableValueType.Float, 3, "float/s", "Float array", out float3ArrayVarHeader);
            })
            .AddDataFrame(
                frame =>
                {
                    frame.Write<int>(intVarHeader, 12);
                    frame.WriteArray<float>(float3ArrayVarHeader, [1.0f, 2.0f, 3.0f]);
                });

        // Write the file
        var fileName = Path.GetTempFileName();

        fileBuilder.Write(fileName, out var writtenHeader);

        using (var reader = new TelemetryFileReader(TelemetryFile.OpenHandle(fileName)))
        {
            Assert.Throws<ArgumentException>(() =>
            {
                Span<byte> dataFrameBlob = new byte[reader.FileHeader.DataBufferElementLength - 1];

                reader.ReadDataFrame(0, dataFrameBlob);
            });
        }

        // Cleanup
        File.Delete(fileName);
    }

    [Fact]
    public void SimpleFile_Test()
    {
        DataVariableHeader intVarHeader = default;
        DataVariableHeader float3ArrayVarHeader = default;
        string sessionInfo = "Test session";

        var fileBuilder = TelemetryFileBuilder.Create(
            varBuilder =>
            {
                varBuilder.AddScalarVariable("Foo", DataVariableValueType.Int, "test", "Description", out intVarHeader);

                varBuilder.AddArrayVariable("Bar", DataVariableValueType.Float, 3, "float/s", "Float array", out float3ArrayVarHeader);
            })
            .SetSessionInfo(sessionInfo, version: 1)
            .AddDataFrame(
                frame =>
                {
                    frame.Write<int>(intVarHeader, 12);
                    frame.WriteArray<float>(float3ArrayVarHeader, [1.0f, 2.0f, 3.0f]);
                });

        // Write the file
        var fileName = Path.GetTempFileName();

        fileBuilder.Write(fileName, out var writtenHeader);

        // Read and assert
        using (var reader = new TelemetryFileReader(fileName))
        {
            Assert.Equal(writtenHeader, reader.FileHeader);

            var testVariableHeaders = new[] { intVarHeader, float3ArrayVarHeader };
            var fileVariableHeaders = reader.ReadDataVariableHeaders();
            var fileSessionInfo = reader.ReadSessionInfo();

            Assert.True(testVariableHeaders.SequenceEqual(fileVariableHeaders));
            Assert.Equal(sessionInfo, fileSessionInfo);
        }

        File.Delete(fileName);
    }

    [Fact]
    public void VerifyCanRead_ThrowOnClosedHandle()
    {
        DataVariableHeader intVarHeader = default;
        DataVariableHeader float3ArrayVarHeader = default;

        var fileBuilder = TelemetryFileBuilder.Create(
            varBuilder =>
            {
                varBuilder.AddScalarVariable("Foo", DataVariableValueType.Int, "test", "Description", out intVarHeader);

                varBuilder.AddArrayVariable("Bar", DataVariableValueType.Float, 3, "float/s", "Float array", out float3ArrayVarHeader);
            })
            .AddDataFrame(
                frame =>
                {
                    frame.Write<int>(intVarHeader, 12);
                    frame.WriteArray<float>(float3ArrayVarHeader, [1.0f, 2.0f, 3.0f]);
                });

        // Write the file
        var fileName = Path.GetTempFileName();

        fileBuilder.Write(fileName, out var writtenHeader);

        using (var fileHandle = TelemetryFile.OpenHandle(fileName))
        using (var reader = new TelemetryFileReader(fileHandle, ownsHandle: false))
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                fileHandle.Close();

                Span<byte> dataFrameBlob = new byte[reader.FileHeader.DataBufferElementLength];

                reader.ReadDataFrame(0, dataFrameBlob);
            });
        }
    }
}
