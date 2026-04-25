using System.CommandLine;

namespace SharpRacer.Tools.TelemetryVariables.CommandLine;

public class DependencyInjectionInvocationConfiguration : InvocationConfiguration
{
    public DependencyInjectionInvocationConfiguration(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public IServiceProvider ServiceProvider { get; }
}
