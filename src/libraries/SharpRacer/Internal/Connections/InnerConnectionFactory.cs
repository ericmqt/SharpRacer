using System.Runtime.Versioning;
using SharpRacer.Interop;
using SharpRacer.IO.Internal;

namespace SharpRacer.Internal.Connections;

internal sealed class InnerConnectionFactory : IClosedInnerConnectionFactory, IOpenInnerConnectionFactory
{
    private readonly IConnectionDataFileFactory _dataFileFactory;
    private readonly IDataReadyEventFactory _dataReadyEventFactory;
    private readonly IConnectionWorkerThreadFactory _openConnectionWorkerThreadFactory;
    private readonly TimeProvider _timeProvider;

    public InnerConnectionFactory(
        IConnectionDataFileFactory dataFileFactory,
        IDataReadyEventFactory dataReadyEventFactory,
        TimeProvider timeProvider)
    {
        _dataFileFactory = dataFileFactory ?? throw new ArgumentNullException(nameof(dataFileFactory));
        _dataReadyEventFactory = dataReadyEventFactory ?? throw new ArgumentNullException(nameof(dataReadyEventFactory));
        _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));

        _openConnectionWorkerThreadFactory = new ConnectionWorkerThreadFactory(_dataReadyEventFactory);
    }

    [SupportedOSPlatform("windows")]
    public IOpenInnerConnection Create(IOpenInnerConnectionOwner owner)
    {
        var dataFile = _dataFileFactory.Create();

        try
        {
            return new OpenInnerConnection(
                owner,
                dataFile,
                new OuterConnectionTracker(closeOnEmpty: true),
                this,
                _openConnectionWorkerThreadFactory,
                _timeProvider);
        }
        catch
        {
            dataFile?.Dispose();

            throw;
        }
    }

    public IClosedInnerConnection CreateClosedInnerConnection(IConnectionDataFile dataFile)
    {
        return new ClosedInnerConnection(dataFile, new OuterConnectionTracker(closeOnEmpty: true));
    }

    public IClosedInnerConnection CreateClosedInnerConnection(IOpenInnerConnection openConnection)
    {
        return new ClosedInnerConnection(openConnection, new OuterConnectionTracker(closeOnEmpty: true));
    }
}
