using System.IO.MemoryMappedFiles;
using DotNext.IO.MemoryMappedFiles;
using Nito.AsyncEx.Interop;
using SharpRacer.Simulator.Interop;

namespace SharpRacer.Simulator;
internal sealed class SimulatorDataFile : ISimulatorDataFile
{
    public const string MemoryMappedFileName = "Local\\IRSDKMemMapFileName";

    private readonly MemoryMappedDirectAccessor _dataAccessor;
    private readonly MemoryMappedFile _dataFile;
    private bool _isDisposed;

    public SimulatorDataFile(MemoryMappedFile dataFile)
    {
        _dataFile = dataFile ?? throw new ArgumentNullException(nameof(dataFile));
        _dataAccessor = dataFile.CreateDirectAccessor(offset: 0, size: 0, access: MemoryMappedFileAccess.Read);
    }

    ~SimulatorDataFile()
    {
        Dispose();
    }

    public static async Task<SimulatorDataFile> OpenOnDataReadyAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var hDataReadyEvent = DataReadyEventHandle.CreateSafeWaitHandle();
        using var dataReadyEvent = new AutoResetEvent(false) { SafeWaitHandle = hDataReadyEvent };

        if (await WaitHandleAsyncFactory.FromWaitHandle(dataReadyEvent, timeout, cancellationToken).ConfigureAwait(false))
        {
            return new SimulatorDataFile(MemoryMappedFile.OpenExisting(MemoryMappedFileName, MemoryMappedFileRights.Read));
        }

        throw new TimeoutException("The timeout period elapsed before the file could be acquired.");
    }

    /// <inheritdoc />
    public ReadOnlySpan<byte> Span
    {
        get
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(SimulatorDataFile));
            }

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
