using System.CommandLine;
using System.Runtime.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharpRacer.Tools.TelemetryVariables.CommandLine;

namespace SharpRacer.Tools.TelemetryVariables.Commands;

[SupportedOSPlatform("windows5.1.2600")]
internal sealed class ImportSimulatorCommand
    : CliCommand<ImportSimulatorCommandHandler, ImportSimulatorCommandOptions>, IDatabaseFileNameProviderCommand
{
    public ImportSimulatorCommand(CliOption<FileInfo> databaseFileOption)
        : base("simulator", "Imports telemetry variables from a simulator session.")
    {
        DatabaseFileOption = databaseFileOption ?? throw new ArgumentNullException(nameof(databaseFileOption));

        WaitForConnectionOption = new CliOption<bool>("--wait", ["--wait", "-w"])
        {
            DefaultValueFactory = _ => false,
            Description = "Wait for a connection to the simulator instead of exiting immediately."
        };

        Options.Add(WaitForConnectionOption);
    }

    public CliOption<FileInfo> DatabaseFileOption { get; }
    public CliOption<bool> WaitForConnectionOption { get; }

    public string GetDatabaseFileName(ParseResult parseResult)
    {
        return parseResult.GetValue(DatabaseFileOption)!.FullName;
    }

    protected override ImportSimulatorCommandOptions CreateOptions(ParseResult parseResult)
    {
        return new ImportSimulatorCommandOptions()
        {
            WaitForConnection = parseResult.GetValue(WaitForConnectionOption)
        };
    }

    protected override async Task<int> InvokeAsync(
        ImportSimulatorCommandHandler handler,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<ImportSimulatorCommand>>();

        try { return await handler.ExecuteAsync(cancellationToken).ConfigureAwait(false); }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception was thrown while executing the operation.");

            return -2;
        }
    }
}
