using System.CommandLine;
using System.CommandLine.Help;

namespace SharpRacer.Tools.TelemetryVariables.Commands;
internal class ImportCommand : CliCommand
{
    public ImportCommand()
        : base("import", description: "Root command for telemetry variable import commands.")
    {
        Subcommands.Add(new ImportJsonCommand());
        Subcommands.Add(new ImportTelemetryCommand());

        if (SimulatorPlatformGuard.IsSupportedPlatform())
        {
            Subcommands.Add(new ImportSimulatorCommand());
        }

        SetAction((parseResult, cancellationToken) =>
        {
            var helpAction = new HelpAction();
            return Task.FromResult(helpAction.Invoke(parseResult));
        });
    }
}
