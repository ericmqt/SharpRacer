using System.CommandLine;
using System.CommandLine.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

            return OnInvokedAsync(parseResult, serviceProvider, cancellationToken);
        });
    }

    protected abstract Task<int> InvokeAsync(THandler handler, IServiceProvider serviceProvider, CancellationToken cancellationToken);

    protected virtual THandler CreateHandler(ParseResult parseResult, IServiceProvider serviceProvider)
    {
        return ActivatorUtilities.CreateInstance<THandler>(serviceProvider);
    }

    private async Task<int> OnInvokedAsync(ParseResult parseResult, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<CliCommand<THandler>>>();

        using var serviceScope = serviceProvider.CreateScope();

        try
        {
            var handler = CreateHandler(parseResult, serviceScope.ServiceProvider);

            return await InvokeAsync(handler, serviceScope.ServiceProvider, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception was thrown while executing the operation.");

            return -2;
        }
    }
}
