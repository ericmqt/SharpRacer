namespace SharpRacer.Tools.TelemetryVariables.Commands;
internal class UpdateDatabaseCommandOptions
{
    public UpdateDatabaseCommandOptions()
    {
        DatabaseFile = GetDefaultDatabaseFile();
    }

    public FileInfo DatabaseFile { get; set; }

    internal static FileInfo GetDefaultDatabaseFile()
    {
        return new FileInfo(Path.Combine(Environment.CurrentDirectory, "TelemetryVariables.db"));
    }
}
