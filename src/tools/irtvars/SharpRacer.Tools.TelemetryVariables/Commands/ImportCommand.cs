using System.CommandLine;
using SharpRacer.Tools.TelemetryVariables.CommandLine;

namespace SharpRacer.Tools.TelemetryVariables.Commands;
internal class ImportCommand : CliCommand
{
    public ImportCommand()
        : base("import", description: "Contains telemetry variable import commands.")
    {
        DatabaseFileOption = new CliOption<FileInfo>("--database", ["--database", "-d"])
        {
            Description = "The database file path.",
            Recursive = true,
            Required = true,
        };

        DatabaseFileOption.AcceptExistingOnly();
        DatabaseFileOption.Validators.Add(ArgumentValidators.SqliteDbContextFileHasNoPendingMigrations);

        Options.Add(DatabaseFileOption);

        Subcommands.Add(new ImportJsonCommand(DatabaseFileOption));
        Subcommands.Add(new ImportTelemetryCommand(DatabaseFileOption));

        if (SimulatorPlatformGuard.IsSupportedPlatform())
        {
            Subcommands.Add(new ImportSimulatorCommand(DatabaseFileOption));
        }
    }

    public CliOption<FileInfo> DatabaseFileOption { get; }
}
