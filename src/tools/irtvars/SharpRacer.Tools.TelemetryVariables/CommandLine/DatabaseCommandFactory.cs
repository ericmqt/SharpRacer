using System.CommandLine;
using System.CommandLine.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharpRacer.Tools.TelemetryVariables.Commands;
using SharpRacer.Tools.TelemetryVariables.Configuration;

namespace SharpRacer.Tools.TelemetryVariables.CommandLine;
internal static class DatabaseCommandFactory
{
    public static CliCommand DatabaseCommand()
    {
        var cmd = new CliCommand("database");

        cmd.Subcommands.Add(CreateDatabaseCommand());
        cmd.Subcommands.Add(UpdateDatabaseCommand());

        return cmd;
    }

    private static CliCommand CreateDatabaseCommand()
    {
        var cmd = new CliCommand("create");

        // Arguments
        var dbArg = new CliArgument<FileInfo?>("database-file")
        {
            Arity = ArgumentArity.ZeroOrOne
        };

        dbArg.AcceptLegalFilePathsOnly();

        cmd.Arguments.Add(dbArg);

        // Handler
        cmd.SetAction((parseResult, cancellationToken) =>
        {
            var host = parseResult.GetHost();
            var databaseOptions = host.Services.GetRequiredService<IOptions<DatabaseOptions>>();

            var commandOptions = new CreateDatabaseCommandOptions()
            {
                DatabaseFile = parseResult.GetValue(dbArg) ?? new FileInfo(databaseOptions.Value.Path)
            };

            var handler = ActivatorUtilities.CreateInstance<CreateDatabaseCommandHandler>(host.Services, commandOptions);

            return handler.ExecuteAsync(cancellationToken);
        });

        return cmd;
    }

    private static CliCommand UpdateDatabaseCommand()
    {
        var cmd = new CliCommand("update");

        // Arguments
        var dbArg = new CliArgument<FileInfo>("database-file");

        dbArg.AcceptLegalFilePathsOnly();

        cmd.Arguments.Add(dbArg);

        // Handler
        cmd.SetAction((parseResult, cancellationToken) =>
        {
            var host = parseResult.GetHost();
            var databaseOptions = host.Services.GetRequiredService<IOptions<DatabaseOptions>>();

            var commandOptions = new UpdateDatabaseCommandOptions()
            {
                DatabaseFile = parseResult.GetValue(dbArg) ?? new FileInfo(databaseOptions.Value.Path)
            };

            var handler = ActivatorUtilities.CreateInstance<UpdateDatabaseCommandHandler>(host.Services, commandOptions);

            return handler.ExecuteAsync(cancellationToken);
        });

        return cmd;
    }
}
