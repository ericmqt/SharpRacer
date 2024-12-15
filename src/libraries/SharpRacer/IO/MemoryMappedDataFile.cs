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
    private readonly List<LifetimeHandle> _owners;

    private MemoryMappedDataFile(MemoryMappedFile dataFile)
    {
        _dataFile = dataFile ?? throw new ArgumentNullException(nameof(dataFile));
        _dataAccessor = dataFile.CreateDirectAccessor(offset: 0, size: 0, access: MemoryMappedFileAccess.Read);
        _owners = new List<LifetimeHandle>();
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

    /// <summary>
    /// Returns an object that must be disposed when the receiving object no longer requires the data file.
    /// </summary>
    public IDisposable AcquireLifetimeHandle()
    {
        // This method is critical to ensuring we get disposed properly.
        //
        // Because we have exposed the span for the memory-mapped file, a consumer may have kept the returned span on the stack, say while
        // looping to read data. If the connection closes, and the memory-mapped file is immediately disposed by the connection pool while
        // that span is being used, the memory-mapped file data that retained span points to no longer exists, which is dangerous. 
        //
        // We can safely keep the memory-mapped file around until the consumer's connections have been disposed. If the simulator starts
        // again in the meantime, it will continue writing to the same file without issue, and connections for the new session will proceed
        // as normal, with a brand new MemoryMappedDataFile instance. The old connections, if they are *still* trying to read from that
        // retained span, would then see the new session data on the span, which technically would be "bad" data from their perspective (if
        // they call SimulatorConnection.Data after it has closed they'll get the "frozen" data file where there is no issue), but that is
        // acceptable given they've ignored every indication that the connection has closed and that they should stop reading.
        //
        // To accomplish this, we will hand out a simple object whose only job is to be disposed when the connection is itself disposed.
        // When all of these handles have been disposed, there are no remaining connection objects alive associated with that session and
        // so we can finally dispose ourselves.
        //
        // When a connection is established, OpenInternalConnection is the first to call this method, and every SimulatorConnection object
        // for a given session sharing that OpenInternalConnection instance also calls this method. As the SimulatorConnection objects
        // are disposed, they also dispose their acquired handles.
        //
        // If the OpenInternalConnection object closes on its own because the simulator exited, the associated SimulatorConnection objects
        // transition into a closed state with a new frozen data file. It will dispose and release its handle. Ideally, anyone still using
        // the now-closed SimulatorConnection objects finishes up their work and dutifully disposes them, dropping the number of handles
        // to zero.
        //
        // If the consumer disposes their connections prior to the simulator exiting, disposal of the last connection sharing the
        // OpenInternalConnection object triggers the pool to dispose the internal connection as well, also dropping the number of handles
        // to zero.
        //
        // Whenever our number of handles *drops* to zero (it will be zero initially until first acquired), we can safely dispose.

        ObjectDisposedException.ThrowIf(_isDisposed, this);

        var handle = new LifetimeHandle(this);

        _owners.Add(handle);

        return handle;
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

    public ISimulatorDataFile Freeze()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        var frozenData = new byte[Span.Length];

        Span.CopyTo(frozenData);

        return new FrozenDataFile(frozenData);
    }

    private void ReleaseLifetimeHandle(LifetimeHandle handle)
    {
        _owners.Remove(handle);

        // If we still have at least one owner, there is nothing left to do as at least one object claims a reference.
        if (_owners.Count > 0)
        {
            return;
        }

        // If we have no more owners, we are safe to release here.
        //
        // The internal connection object, which is managed and disposed by the connection pool, also has a reference to the data file, so
        // if we have zero owners then that means that not only did all of the outer connections get disposed, the pool is done with the
        // internal connection and there is no risk of a new connection getting a reference to us afterwards.

        Dispose();
    }

    private class LifetimeHandle : IDisposable
    {
        private readonly MemoryMappedDataFile _dataFile;
        private bool _isDisposed;

        public LifetimeHandle(MemoryMappedDataFile dataFile)
        {
            _dataFile = dataFile ?? throw new ArgumentNullException(nameof(dataFile));
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _dataFile.ReleaseLifetimeHandle(this);

                _isDisposed = true;
            }

            GC.SuppressFinalize(this);
        }
    }
}
