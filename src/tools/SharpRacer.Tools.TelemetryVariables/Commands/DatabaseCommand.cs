using System.CommandLine;
using System.CommandLine.Help;

namespace SharpRacer.Tools.TelemetryVariables.Commands;

internal class DatabaseCommand : Command
{
    public DatabaseCommand()
        : base("database", "Contains database creation and migration commands.")
    {
        Subcommands.Add(new CreateDatabaseCommand());
        Subcommands.Add(new MigrateDatabaseCommand());

        SetAction((parseResult, cancellationToken) =>
        {
            var helpAction = new HelpAction();
            return Task.FromResult(helpAction.Invoke(parseResult));
        });
    }
}
