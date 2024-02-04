namespace SharpRacer.Tools.TelemetryVariables.Commands;
internal class ImportTelemetryCommandOptions
{
    public ImportTelemetryCommandOptions()
    {
        InputFileOrDirectory = GetDefaultInputDirectory();
    }

    public bool EnumerateInputFilesRecursively { get; set; }
    public FileSystemInfo InputFileOrDirectory { get; set; }

    internal static DirectoryInfo GetDefaultInputDirectory()
    {
        var telemetryDirPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "iRacing", "telemetry");

        return new DirectoryInfo(telemetryDirPath);
    }
}
