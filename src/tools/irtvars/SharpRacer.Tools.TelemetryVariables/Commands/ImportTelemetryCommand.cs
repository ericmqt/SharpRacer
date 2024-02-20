using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharpRacer.Tools.TelemetryVariables.CommandLine;

namespace SharpRacer.Tools.TelemetryVariables.Commands;
internal sealed class ImportTelemetryCommand
    : CliCommand<ImportTelemetryCommandHandler, ImportTelemetryCommandOptions>, IDatabaseFileNameProviderCommand
{
    public ImportTelemetryCommand(CliOption<FileInfo> databaseFileOption)
        : base(name: "telemetry", description: "Imports telemetry variables from telemetry files (*.IBT).")
    {
        DatabaseFileOption = databaseFileOption ?? throw new ArgumentNullException(nameof(databaseFileOption));

        InputFileOrDirectoryArgument = new CliArgument<FileSystemInfo>("input-file-or-dir")
        {
            Arity = new ArgumentArity(0, 1),
            DefaultValueFactory = _ => ImportTelemetryCommandOptions.GetDefaultInputDirectory(),
            Description = "Input file name or directory. Defaults to iRacing telemetry folder."
        };

        RecursiveOption = new CliOption<bool>("--recurse", ["--recurse", "-r"]);

        Arguments.Add(InputFileOrDirectoryArgument);
        Options.Add(RecursiveOption);
    }

    public CliOption<FileInfo> DatabaseFileOption { get; }
    public CliArgument<FileSystemInfo> InputFileOrDirectoryArgument { get; }
    public CliOption<bool> RecursiveOption { get; }

    public string GetDatabaseFileName(ParseResult parseResult)
    {
        return parseResult.GetValue(DatabaseFileOption)!.FullName;
    }

    protected override ImportTelemetryCommandOptions CreateOptions(ParseResult parseResult)
    {
        return new ImportTelemetryCommandOptions()
        {
            EnumerateInputFilesRecursively = parseResult.GetValue(RecursiveOption),
            InputFileOrDirectory = parseResult.GetValue(InputFileOrDirectoryArgument) ?? ImportTelemetryCommandOptions.GetDefaultInputDirectory()
        };
    }

    protected override async Task<int> InvokeAsync(
        ImportTelemetryCommandHandler handler,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<ImportTelemetryCommand>>();

        try { return await handler.ExecuteAsync(cancellationToken).ConfigureAwait(false); }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception was thrown while executing the operation.");

            return -2;
        }
    }
}
