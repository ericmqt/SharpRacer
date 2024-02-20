using System.CommandLine;

namespace SharpRacer.Tools.TelemetryVariables.Commands;
internal class DatabaseCommand : CliCommand
{
    public DatabaseCommand()
        : base("database", "Contains database creation and migration commands.")
    {
        Subcommands.Add(new CreateDatabaseCommand());
        Subcommands.Add(new MigrateDatabaseCommand());
    }
}
