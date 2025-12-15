using System.Runtime.Versioning;
using SharpRacer.Interop;

namespace SharpRacer.Internal.Connections;
internal sealed class ConnectionWorkerThreadFactory : IConnectionWorkerThreadFactory
{
    private readonly IDataReadyEventFactory _dataReadyEventFactory;

    public ConnectionWorkerThreadFactory(IDataReadyEventFactory dataReadyEventFactory)
    {
        _dataReadyEventFactory = dataReadyEventFactory ?? throw new ArgumentNullException(nameof(dataReadyEventFactory));
    }

    [SupportedOSPlatform("windows")]
    public static IConnectionWorkerThreadFactory Default { get; } = new ConnectionWorkerThreadFactory(DataReadyEventFactory.Default);

    public IConnectionWorkerThread Create(IConnectionWorkerThreadOwner owner, TimeProvider timeProvider)
    {
        return new ConnectionWorkerThread(owner, _dataReadyEventFactory, timeProvider);
    }
}
