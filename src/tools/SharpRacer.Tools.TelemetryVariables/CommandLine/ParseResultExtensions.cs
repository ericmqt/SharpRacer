using System.CommandLine;

namespace SharpRacer.Tools.TelemetryVariables.CommandLine;

internal static class ParseResultExtensions
{
    internal static IServiceProvider GetServiceProvider(this ParseResult parseResult)
    {
        ArgumentNullException.ThrowIfNull(parseResult);

        if (parseResult.InvocationConfiguration is DependencyInjectionInvocationConfiguration serviceProviderInvocationConfiguration)
        {
            return serviceProviderInvocationConfiguration.ServiceProvider;
        }

        throw new InvalidOperationException(
            $"'{nameof(parseResult)}' property '{nameof(ParseResult.InvocationConfiguration)}' is not of type {nameof(DependencyInjectionInvocationConfiguration)}");
    }
}
