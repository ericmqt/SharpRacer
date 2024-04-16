using System.IO.MemoryMappedFiles;
using System.Runtime.Versioning;
using DotNext.IO.MemoryMappedFiles;

namespace SharpRacer.IO;
[SupportedOSPlatform("windows5.1.2600")]
internal sealed class MemoryMappedDataFile : ISimulatorDataFile
{
    public const string MemoryMappedFileName = "Local\\IRSDKMemMapFileName";

    private readonly MemoryMappedDirectAccessor _dataAccessor;
    private readonly MemoryMappedFile _dataFile;
    private bool _isDisposed;

    private MemoryMappedDataFile(MemoryMappedFile dataFile)
    {
        _dataFile = dataFile ?? throw new ArgumentNullException(nameof(dataFile));
        _dataAccessor = dataFile.CreateDirectAccessor(offset: 0, size: 0, access: MemoryMappedFileAccess.Read);
    }

    ~MemoryMappedDataFile()
    {
        Dispose();
    }

    public static MemoryMappedDataFile Open()
    {
        return new MemoryMappedDataFile(MemoryMappedFile.OpenExisting(MemoryMappedFileName, MemoryMappedFileRights.Read));
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

    public ISimulatorDataFile Freeze()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        var frozenData = new byte[Span.Length];

        Span.CopyTo(frozenData);

        return new FrozenDataFile(frozenData);
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
