using System.CommandLine;
using System.CommandLine.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace SharpRacer.Tools.TelemetryVariables.CommandLine;
internal abstract class CliCommand<THandler, TOptions> : CliCommand<THandler>
    where THandler : ICommandHandler<TOptions>
    where TOptions : class
{
    protected CliCommand(string name, string? description = null)
        : base(name, description)
    {

    }

    protected abstract TOptions CreateOptions(ParseResult parseResult);

    protected override THandler CreateHandler(ParseResult parseResult)
    {
        var commandOptions = CreateOptions(parseResult);
        var serviceProvider = parseResult.GetHost().Services;

        return ActivatorUtilities.CreateInstance<THandler>(serviceProvider, commandOptions);
    }
}
