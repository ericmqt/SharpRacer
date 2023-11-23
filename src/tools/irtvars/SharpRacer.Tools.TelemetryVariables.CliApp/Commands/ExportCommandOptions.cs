namespace SharpRacer.Tools.TelemetryVariables.Commands;
internal class ExportCommandOptions
{
    public bool ExportVariablesOnly { get; set; }
    public bool IncludeDeprecated { get; set; }
    public FileSystemInfo OutputFileOrDirectory { get; set; } = default!;
}
