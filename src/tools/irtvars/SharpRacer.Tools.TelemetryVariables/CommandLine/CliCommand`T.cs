using System.CommandLine;
using System.CommandLine.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace SharpRacer.Tools.TelemetryVariables.CommandLine;

internal abstract class CliCommand<THandler> : CliCommand
    where THandler : ICommandHandler
{
    protected CliCommand(string name, string? description = null)
        : base(name, description)
    {
        SetAction((parseResult, cancellationToken) =>
        {
            var serviceProvider = parseResult.GetHost().Services;
            var handler = CreateHandler(parseResult);

            return InvokeAsync(handler, serviceProvider, cancellationToken);
        });
    }

    protected abstract Task<int> InvokeAsync(THandler handler, IServiceProvider serviceProvider, CancellationToken cancellationToken);

    protected virtual THandler CreateHandler(ParseResult parseResult)
    {
        var host = parseResult.GetHost();

        return ActivatorUtilities.CreateInstance<THandler>(host.Services);
    }
}
