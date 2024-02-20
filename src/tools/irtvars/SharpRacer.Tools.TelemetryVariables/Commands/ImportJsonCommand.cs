using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharpRacer.Tools.TelemetryVariables.CommandLine;

namespace SharpRacer.Tools.TelemetryVariables.Commands;
internal sealed class ImportJsonCommand
    : CliCommand<ImportJsonCommandHandler, ImportJsonCommandOptions>, IDatabaseFileNameProviderCommand
{
    public ImportJsonCommand(CliOption<FileInfo> databaseFileOption)
        : base("json", "Imports telemetry variables from a JSON document exported by this tool.")
    {
        DatabaseFileOption = databaseFileOption ?? throw new ArgumentNullException(nameof(databaseFileOption));

        InputFileArgument = new CliArgument<FileInfo>("input-file")
        {
            Arity = ArgumentArity.ExactlyOne,
            Description = "Path to JSON-formatted variables database file."
        }
        .AcceptExistingOnly();

        Arguments.Add(InputFileArgument);
    }

    public CliOption<FileInfo> DatabaseFileOption { get; }
    public CliArgument<FileInfo> InputFileArgument { get; }

    public string GetDatabaseFileName(ParseResult parseResult)
    {
        return parseResult.GetValue(DatabaseFileOption)!.FullName;
    }

    protected override ImportJsonCommandOptions CreateOptions(ParseResult parseResult)
    {
        return new ImportJsonCommandOptions()
        {
            InputFile = parseResult.GetValue(InputFileArgument)!
        };
    }

    protected override async Task<int> InvokeAsync(
        ImportJsonCommandHandler handler,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<ImportJsonCommand>>();

        try { return await handler.ExecuteAsync(cancellationToken).ConfigureAwait(false); }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception was thrown while executing the operation.");

            return -2;
        }
    }
}
