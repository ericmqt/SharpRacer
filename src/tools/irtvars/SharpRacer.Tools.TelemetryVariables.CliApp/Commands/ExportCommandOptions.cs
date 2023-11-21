namespace SharpRacer.Tools.TelemetryVariables.Commands;
internal class ExportCommandOptions
{
    public bool IncludeDeprecated { get; set; }
    public FileSystemInfo OutputFileOrDirectory { get; set; } = default!;
}
