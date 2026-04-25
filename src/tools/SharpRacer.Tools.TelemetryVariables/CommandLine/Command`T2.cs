using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;

namespace SharpRacer.Tools.TelemetryVariables.CommandLine;

internal abstract class Command<THandler, TOptions> : Command<THandler>
    where THandler : ICommandHandler
    where TOptions : class
{
    protected Command(string name, string? description = null)
        : base(name, description)
    {
    }

    protected abstract TOptions CreateOptions(ParseResult parseResult);

    protected override THandler CreateHandler(IServiceProvider serviceProvider, ParseResult parseResult)
    {
        var commandOptions = CreateOptions(parseResult);

        return ActivatorUtilities.CreateInstance<THandler>(serviceProvider, commandOptions);
    }
}
