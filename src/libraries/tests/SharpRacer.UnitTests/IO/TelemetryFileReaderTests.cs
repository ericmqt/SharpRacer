using SharpRacer.Interop;
using SharpRacer.IO.TestUtilities;

namespace SharpRacer.IO;
public class TelemetryFileReaderTests
{
    [Fact]
    public void SimpleFile_Test()
    {
        DataVariableHeader intVarHeader = default;
        DataVariableHeader float3ArrayVarHeader = default;

        var fileBuilder = TelemetryFileBuilder.Create(
            varBuilder =>
            {
                varBuilder.AddScalarVariable("Foo", Telemetry.DataVariableValueType.Int, "test", "Description", out intVarHeader);

                varBuilder.AddArrayVariable("Bar", Telemetry.DataVariableValueType.Float, 3, "float/s", "Float array", out float3ArrayVarHeader);
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

        // Read and assert
        using var reader = new TelemetryFileReader(fileName);

        Assert.Equal(writtenHeader, reader.FileHeader);

        var testVariableHeaders = new[] { intVarHeader, float3ArrayVarHeader };
        var fileVariableHeaders = reader.ReadDataVariableHeaders();

        Assert.True(testVariableHeaders.SequenceEqual(fileVariableHeaders));

        File.Delete(fileName);
    }
}
