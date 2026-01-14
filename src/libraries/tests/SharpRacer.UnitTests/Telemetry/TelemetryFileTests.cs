using SharpRacer.Interop;
using SharpRacer.IO;
using SharpRacer.Telemetry.TestUtilities;

namespace SharpRacer.Telemetry;

public class TelemetryFileTests
{
    [Fact]
    public void SimpleFile_Test()
    {
        TelemetryVariableHeader intVarHeader = default;
        TelemetryVariableHeader float3ArrayVarHeader = default;

        var fileBuilder = TelemetryFileBuilder.Create(
            varBuilder =>
            {
                varBuilder.AddScalarVariable("Foo", TelemetryVariableValueType.Int, "test", "Description", out intVarHeader);

                varBuilder.AddArrayVariable("Bar", TelemetryVariableValueType.Float, 3, "float/s", "Float array", out float3ArrayVarHeader);
            })
            .SetSessionInfo("Test session", version: 99)
            .AddDataFrame(
                frame =>
                {
                    frame.Write<int>(intVarHeader, 12);
                    frame.WriteArray<float>(float3ArrayVarHeader, [1.0f, 2.0f, 3.0f]);
                });

        var fileName = Path.GetTempFileName();

        fileBuilder.Write(fileName, out var writtenHeader);

        var header = TelemetryFile.ReadHeader(fileName);

        Assert.Equal(99, header.SessionInfoVersion);

        Assert.True(TelemetryFile.ValidateHeader(header), "Header validation failed");

        Assert.Equal(new FileInfo(fileName).Length, TelemetryFile.CalculateFileLengthFromHeader(header));
    }

    [Fact]
    public void ReadHeader_ThrowOnFileTooSmallTest()
    {
        var fileName = Path.GetTempFileName();

        Assert.Throws<IOException>(() => TelemetryFile.ReadHeader(fileName));

        File.Delete(fileName);
    }
}
