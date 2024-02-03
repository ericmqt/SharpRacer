using System.IO.MemoryMappedFiles;
using System.Runtime.Versioning;
using DotNext.IO.MemoryMappedFiles;

namespace SharpRacer.Internal;
[SupportedOSPlatform("windows5.1.2600")]
internal sealed class SimulatorDataFile : ISimulatorDataFile
{
    public const string MemoryMappedFileName = "Local\\IRSDKMemMapFileName";

    private readonly MemoryMappedDirectAccessor _dataAccessor;
    private readonly MemoryMappedFile _dataFile;
    private bool _isDisposed;

    private SimulatorDataFile(MemoryMappedFile dataFile)
    {
        _dataFile = dataFile ?? throw new ArgumentNullException(nameof(dataFile));
        _dataAccessor = dataFile.CreateDirectAccessor(offset: 0, size: 0, access: MemoryMappedFileAccess.Read);
    }

    ~SimulatorDataFile()
    {
        Dispose();
    }

    public static SimulatorDataFile Open()
    {
        return new SimulatorDataFile(MemoryMappedFile.OpenExisting(MemoryMappedFileName, MemoryMappedFileRights.Read));
    }

    /// <inheritdoc />
    public ReadOnlySpan<byte> Span
    {
        get
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);
            return _dataAccessor.Bytes;
        }
    }

    public void Dispose()
    {
        if (!_isDisposed)
        {
            _dataAccessor.Dispose();
            _dataFile.Dispose();

            _isDisposed = true;
        }

        GC.SuppressFinalize(this);
    }
}
