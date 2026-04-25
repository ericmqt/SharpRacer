using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SharpRacer.Tools.TelemetryVariables.CommandLine;

internal abstract class Command<THandler> : Command
    where THandler : ICommandHandler
{
    protected Command(string name, string? description = null)
        : base(name, description)
    {
        SetAction(OnInvokedAsync);
    }

    protected virtual THandler CreateHandler(IServiceProvider serviceProvider, ParseResult parseResult)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(parseResult);

        return ActivatorUtilities.CreateInstance<THandler>(serviceProvider);
    }

    protected virtual async Task<int> InvokeAsync(THandler handler, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<THandler>>();

        try
        {
            return await handler.ExecuteAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception was thrown while executing the operation.");

            return -2;
        }
    }

    private async Task<int> OnInvokedAsync(ParseResult parseResult, CancellationToken cancellationToken)
    {
        var serviceProvider = parseResult.GetServiceProvider();

        var logger = serviceProvider.GetRequiredService<ILogger<Command<THandler>>>();

        using var serviceScope = serviceProvider.CreateScope();

        try
        {
            var handler = CreateHandler(serviceScope.ServiceProvider, parseResult);

            var exitCode = await InvokeAsync(handler, serviceScope.ServiceProvider, cancellationToken).ConfigureAwait(false);

            return exitCode;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception was thrown while executing the operation.");

            return -2;
        }
    }
}
