using System.CommandLine;
using System.CommandLine.Hosting;
using System.Runtime.Versioning;
using Microsoft.Extensions.DependencyInjection;
using SharpRacer.Tools.TelemetryVariables.Commands;

namespace SharpRacer.Tools.TelemetryVariables.CommandLine;
internal static class ImportCommandFactory
{
    public static CliCommand ImportCommand()
    {
        var cmd = new CliCommand("import");

        cmd.Subcommands.Add(JsonCommand());

        if (IsPlatformCompatible())
        {
            cmd.Subcommands.Add(SimulatorCommand());
        }

        cmd.Subcommands.Add(TelemetryCommand());

        return cmd;
    }

    private static CliCommand JsonCommand()
    {
        // Arguments
        var inputFile = new CliArgument<FileInfo>("input-file")
        {
            Arity = ArgumentArity.ExactlyOne,
            Description = "Path to JSON-formatted variables database file."
        }
        .AcceptExistingOnly();

        var cmd = new CliCommand("json");

        cmd.Arguments.Add(inputFile);

        // Handler
        cmd.SetAction((parseResult, cancellationToken) =>
        {
            var host = parseResult.GetHost();

            var commandOptions = new ImportJsonCommandOptions()
            {
                InputFile = parseResult.GetValue(inputFile)!
            };

            var handler = ActivatorUtilities.CreateInstance<ImportJsonCommandHandler>(host.Services, commandOptions);

            return handler.ExecuteAsync(cancellationToken);
        });

        return cmd;
    }

    [SupportedOSPlatform("windows5.1.2600")]
    private static CliCommand SimulatorCommand()
    {
        var cmd = new CliCommand("simulator");

        cmd.SetAction((parseResult, cancellationToken) =>
        {
            var host = parseResult.GetHost();

            var handler = ActivatorUtilities.CreateInstance<ImportSimulatorCommandHandler>(host.Services);

            return handler.ExecuteAsync(cancellationToken);
        });

        return cmd;
    }

    private static CliCommand TelemetryCommand()
    {
        var cmd = new CliCommand(
            name: "telemetry",
            description: "Imports variables from telemetry files (*.IBT).");

        // Arguments
        var inputFileOrDirectoryArg = new CliArgument<FileSystemInfo>("input-file-or-dir")
        {
            Arity = new ArgumentArity(0, 1),
            DefaultValueFactory = _ => ImportTelemetryCommandOptions.GetDefaultInputDirectory(),
            Description = "Input file name or directory. Defaults to iRacing telemetry folder."
        };

        // Options
        var recursiveOption = new CliOption<bool>("--recurse", ["--recurse", "-r"]);

        cmd.Arguments.Add(inputFileOrDirectoryArg);
        cmd.Options.Add(recursiveOption);

        // Handler
        cmd.SetAction((parseResult, cancellationToken) =>
        {
            var host = parseResult.GetHost();

            var commandOptions = new ImportTelemetryCommandOptions()
            {
                EnumerateInputFilesRecursively = parseResult.GetValue(recursiveOption),
                InputFileOrDirectory = parseResult.GetValue(inputFileOrDirectoryArg) ?? ImportTelemetryCommandOptions.GetDefaultInputDirectory()
            };

            var handler = ActivatorUtilities.CreateInstance<ImportTelemetryCommandHandler>(host.Services, commandOptions);

            return handler.ExecuteAsync(cancellationToken);
        });

        return cmd;
    }

    [SupportedOSPlatformGuard("windows5.1.2600")]
    private static bool IsPlatformCompatible()
    {
        return OperatingSystem.IsOSPlatformVersionAtLeast("windows", 5, 1, 2600);
    }
}
