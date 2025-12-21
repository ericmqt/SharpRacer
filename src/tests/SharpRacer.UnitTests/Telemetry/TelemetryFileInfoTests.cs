using SharpRacer.Interop;
using SharpRacer.IO;
using SharpRacer.Telemetry.TestUtilities;

namespace SharpRacer.Telemetry;
public class TelemetryFileInfoTests
{
    [Fact]
    public void Ctor_Test()
    {
        TelemetryVariableHeader intVarHeader = default;
        TelemetryVariableHeader float3ArrayVarHeader = default;
        string sessionInfo = "Test session info";
        var sessionStart = new DateTimeOffset(2023, 9, 22, 14, 30, 12, TimeSpan.FromHours(-5));
        var sessionDuration = TimeSpan.FromMinutes(19);
        var sessionEnd = sessionStart.Add(sessionDuration);

        var fileBuilder = TelemetryFileBuilder.Create(
            varBuilder =>
            {
                varBuilder.AddScalarVariable("Foo", TelemetryVariableValueType.Int, "test", "Description", out intVarHeader);

                varBuilder.AddArrayVariable("Bar", TelemetryVariableValueType.Float, 3, "float/s", "Float array", out float3ArrayVarHeader);
            })
            .SetSessionInfo(sessionInfo, 1)
            .SetSessionStartAndDuration(sessionStart, sessionDuration)
            .AddDataFrame(
                frame =>
                {
                    frame.Write<int>(intVarHeader, 12);
                    frame.WriteArray<float>(float3ArrayVarHeader, [1.0f, 2.0f, 3.0f]);
                });

        // Write the file
        var fileName = Path.GetTempFileName();

        fileBuilder.Write(fileName, out var writtenHeader);

        // Assert
        var fileInfo = new TelemetryFileInfo(fileName);

        Assert.NotNull(fileInfo.FileInfo);
        Assert.Equal(sessionInfo, fileInfo.SessionInfo);
        Assert.Equal(writtenHeader.VariableCount, fileInfo.Variables.Count());
        Assert.Equal(sessionStart, fileInfo.SessionStart);
        Assert.Equal(sessionEnd, fileInfo.SessionEnd);
        Assert.Equal(fileInfo.FileName, fileInfo.FileInfo.FullName);
    }

    [Fact]
    public void Ctor_ThrowOnNullOrEmptyFileNameTest()
    {
        Assert.Throws<ArgumentException>(() => new TelemetryFileInfo(fileName: null!));
        Assert.Throws<ArgumentException>(() => new TelemetryFileInfo(fileName: string.Empty));
    }

    [Fact]
    public void Ctor_ThrowOnFileDoesNotExistTest()
    {
        var fileName = Path.GetTempFileName();

        File.Delete(fileName);

        Assert.Throws<FileNotFoundException>(() => new TelemetryFileInfo(fileName));
    }
}
