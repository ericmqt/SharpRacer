using System.CommandLine;
using System.CommandLine.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharpRacer.Tools.TelemetryVariables.Commands;
using SharpRacer.Tools.TelemetryVariables.Configuration;

namespace SharpRacer.Tools.TelemetryVariables.CommandLine;
internal static class CommandFactory
{
    internal static CliCommand ConfigureCommand()
    {
        var cmd = new CliCommand("configure", "Configures application options.");

        var setDatabasePathOption = new CliOption<FileInfo?>("--set-database-path")
        {
            Description = "The path to the SQLite database file used for storing variable information."
        };

        setDatabasePathOption.AcceptLegalFilePathsOnly();

        cmd.Options.Add(setDatabasePathOption);

        cmd.SetAction((parseResult, cancellationToken) =>
        {
            var host = parseResult.GetHost();

            var commandOptions = new ConfigureCommandOptions()
            {
                DatabaseFile = parseResult.GetValue(setDatabasePathOption)
            };

            var handler = ActivatorUtilities.CreateInstance<ConfigureCommandHandler>(host.Services, commandOptions);

            return handler.ExecuteAsync(cancellationToken);
        });

        return cmd;
    }

    internal static CliCommand ExportCommand()
    {
        // Arguments
        var outputFileOrDirectoryArg = new CliArgument<FileSystemInfo>("output-file-or-dir")
        {
            Arity = new ArgumentArity(0, 1),
            DefaultValueFactory = _ => new DirectoryInfo(Environment.CurrentDirectory)
        };

        outputFileOrDirectoryArg.AcceptLegalFilePathsOnly();

        // Options
        var includeDeprecatedOption = new CliOption<bool>("--include-deprecated")
        {
            DefaultValueFactory = _ => false
        };

        var exportVariablesOnlyOption = new CliOption<bool>("--variables-only") // TODO: Need a better name for this flag
        {
            Description = "Export variables only.",
            DefaultValueFactory = _ => false
        };

        var cmd = new CliCommand("export", "Exports telemetry variable information.");

        cmd.Arguments.Add(outputFileOrDirectoryArg);
        cmd.Options.Add(includeDeprecatedOption);
        cmd.Options.Add(exportVariablesOnlyOption);

        cmd.SetAction((parseResult, cancellationToken) =>
        {
            var host = parseResult.GetHost();
            var databaseOptions = host.Services.GetRequiredService<IOptions<DatabaseOptions>>();

            var commandOptions = new ExportCommandOptions()
            {
                ExportVariablesOnly = parseResult.GetValue(exportVariablesOnlyOption),
                IncludeDeprecated = parseResult.GetValue(includeDeprecatedOption),
                OutputFileOrDirectory = parseResult.GetValue(outputFileOrDirectoryArg)!
            };

            var handler = ActivatorUtilities.CreateInstance<ExportCommandHandler>(host.Services, commandOptions);

            return handler.ExecuteAsync(cancellationToken);
        });

        return cmd;
    }
}
