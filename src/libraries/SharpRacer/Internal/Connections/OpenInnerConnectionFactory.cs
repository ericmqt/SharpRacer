using System.Runtime.Versioning;
using SharpRacer.Interop;
using SharpRacer.IO;

namespace SharpRacer.Internal.Connections;

[SupportedOSPlatform("windows")]
internal sealed class OpenInnerConnectionFactory : IOpenInnerConnectionFactory
{
    private readonly IDataReadyEventFactory _dataReadyEventFactory;
    private readonly TimeProvider _timeProvider;

    public OpenInnerConnectionFactory(IDataReadyEventFactory dataReadyEventFactory, TimeProvider timeProvider)
    {
        _dataReadyEventFactory = dataReadyEventFactory;
        _timeProvider = timeProvider;
    }

    public IOpenInnerConnection Create(IOpenInnerConnectionOwner owner)
    {
        var dataFile = MemoryMappedDataFile.Open();

        try { return new OpenInnerConnection(owner, dataFile, new OuterConnectionTracker(), _dataReadyEventFactory, _timeProvider); }
        catch
        {
            dataFile.Dispose();

            throw;
        }
    }
}
