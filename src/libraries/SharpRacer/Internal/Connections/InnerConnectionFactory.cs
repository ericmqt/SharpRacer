using System.Runtime.Versioning;
using SharpRacer.Interop;
using SharpRacer.IO.Internal;

namespace SharpRacer.Internal.Connections;

internal sealed class InnerConnectionFactory : IClosedInnerConnectionFactory, IOpenInnerConnectionFactory
{
    private readonly IConnectionDataFileFactory _dataFileFactory;
    private readonly IDataReadyEventFactory _dataReadyEventFactory;
    private readonly TimeProvider _timeProvider;

    public InnerConnectionFactory(
        IConnectionDataFileFactory dataFileFactory,
        IDataReadyEventFactory dataReadyEventFactory,
        TimeProvider timeProvider)
    {
        _dataFileFactory = dataFileFactory;
        _dataReadyEventFactory = dataReadyEventFactory;
        _timeProvider = timeProvider;
    }

    [SupportedOSPlatform("windows")]
    public IOpenInnerConnection Create(IOpenInnerConnectionOwner owner)
    {
        IConnectionDataFile? dataFile = null;

        try
        {
            dataFile = _dataFileFactory.Create();

            return new OpenInnerConnection(
                owner,
                dataFile,
                new OuterConnectionTracker(closeOnEmpty: true),
                _dataReadyEventFactory,
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
        return new ClosedInnerConnection(dataFile, new OuterConnectionTracker(closeOnEmpty: false));
    }
}
