using System.CommandLine;
using System.CommandLine.Parsing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharpRacer.Tools.TelemetryVariables.CommandLine;

namespace SharpRacer.Tools.TelemetryVariables.Commands;
internal class CreateDatabaseCommand : CliCommand<CreateDatabaseCommandHandler, CreateDatabaseCommandOptions>, IDatabaseFileNameProviderCommand
{
    private static string _DefaultDatabaseFileName = "TelemetryVariables.db";

    public CreateDatabaseCommand()
        : base("create", "Creates a new telemetry variables database.")
    {
        DatabaseFileArgument = new CliArgument<FileInfo?>("database-file")
        {
            Arity = ArgumentArity.ZeroOrOne,
            Description = $"The path of the database file to create.",
            DefaultValueFactory = _ => new FileInfo(_DefaultDatabaseFileName)
        };

        DatabaseFileArgument.AcceptLegalFilePathsOnly();
        DatabaseFileArgument.Validators.Add(r => ValidateDatabaseFileDoesNotExist(DatabaseFileArgument, r));

        Arguments.Add(DatabaseFileArgument);
    }

    public CliArgument<FileInfo?> DatabaseFileArgument { get; }

    public string GetDatabaseFileName(ParseResult parseResult)
    {
        var databaseFile = parseResult.GetValue(DatabaseFileArgument) ?? GetDefaultDatabaseFile();

        return databaseFile.FullName;
    }

    protected override CreateDatabaseCommandOptions CreateOptions(ParseResult parseResult)
    {
        return new CreateDatabaseCommandOptions
        {
            DatabaseFile = parseResult.GetValue(DatabaseFileArgument) ?? GetDefaultDatabaseFile()
        };
    }

    protected override async Task<int> InvokeAsync(
        CreateDatabaseCommandHandler handler,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<CreateDatabaseCommand>>();

        try { return await handler.ExecuteAsync(cancellationToken).ConfigureAwait(false); }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception was thrown while executing the operation.");

            return -2;
        }
    }

    internal static FileInfo GetDefaultDatabaseFile()
    {
        return new FileInfo(Path.Combine(Environment.CurrentDirectory, _DefaultDatabaseFileName));
    }

    private static void ValidateDatabaseFileDoesNotExist(CliArgument<FileInfo?> databaseFileArgument, ArgumentResult argumentResult)
    {
        var file = argumentResult.GetValue(databaseFileArgument);

        if (file is null)
        {
            return;
        }

        if (file.Exists)
        {
            argumentResult.AddError("Database file already exists.");
        }
    }
}
