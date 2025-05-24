using System.Diagnostics;
using System.Runtime.Versioning;
using SharpRacer.Interop;
using SharpRacer.IO.Internal;

namespace SharpRacer.Internal.Connections;

[SupportedOSPlatform("windows")]
internal sealed class OpenInnerConnection : IOpenInnerConnection, IConnectionWorkerThreadOwner
{
    private readonly DotNext.Threading.AsyncManualResetEvent _asyncDataReadySignal;
    private readonly object _closeLock = new();
    private readonly ClosedInnerConnection _closedInnerConnection;
    private readonly IConnectionDataFile _dataFile;
    private readonly ManualResetEvent _dataReadySignal;
    private int _idleTimeoutMs;
    private bool _isClosed;
    private bool _isDisposed;
    private readonly IOpenInnerConnectionOwner _owner;
    private readonly IOuterConnectionTracker _outerConnectionTracker;
    private readonly TimeProvider _timeProvider;
    private readonly ConnectionWorkerThread _workerThread;

    public OpenInnerConnection(IOpenInnerConnectionOwner owner, IConnectionDataFile dataFile)
        : this(owner, dataFile, new OuterConnectionTracker(closeOnEmpty: true), DataReadyEventFactory.Default, TimeProvider.System)
    {

    }

    public OpenInnerConnection(
        IOpenInnerConnectionOwner owner,
        IConnectionDataFile dataFile,
        IOuterConnectionTracker outerConnectionTracker,
        IDataReadyEventFactory dataReadyEventFactory,
        TimeProvider timeProvider)
    {
        _owner = owner;
        _dataFile = dataFile;

        _outerConnectionTracker = outerConnectionTracker;
        _timeProvider = timeProvider;

        _closedInnerConnection = new ClosedInnerConnection(_dataFile, new OuterConnectionTracker(closeOnEmpty: false));

        ConnectionId = owner.NewConnectionId();
        IdleTimeout = TimeSpan.FromSeconds(5);

        _dataReadySignal = new ManualResetEvent(false);
        _asyncDataReadySignal = new DotNext.Threading.AsyncManualResetEvent(false);

        _workerThread = new ConnectionWorkerThread(this, dataReadyEventFactory, timeProvider);
    }

    public int ConnectionId { get; }
    public ReadOnlySpan<byte> Data => _dataFile.Memory.Span;
    public IConnectionDataFile DataFile => _dataFile;

    public TimeSpan IdleTimeout
    {
        get => TimeSpan.FromMilliseconds(_idleTimeoutMs);
        set
        {
            if (value == Timeout.InfiniteTimeSpan)
            {
                throw new InvalidOperationException($"Value cannot be an infinite {nameof(TimeSpan)}.");
            }

            if (value < TimeSpan.Zero)
            {
                throw new InvalidOperationException($"Value cannot be a {nameof(TimeSpan)} less than the constant {nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}.");
            }

            Interlocked.Exchange(ref _idleTimeoutMs, (int)value.TotalMilliseconds);
        }
    }

    public SimulatorConnectionState State { get; } = SimulatorConnectionState.Open;

    public bool Attach(IOuterConnection outerConnection)
    {
        if (_isDisposed)
        {
            return false;
        }

        // This will automatically return false when we have become orphaned, preventing connection requests from completing successfully
        // while we are in the middle of closing prior to the connection manager being notified we are closing.
        if (_outerConnectionTracker.Attach(outerConnection))
        {
            // Set the inner connection
            outerConnection.SetOpenInnerConnection(this);

            return true;
        }

        return false;
    }

    public void Close()
    {
        lock (_closeLock)
        {
            // Notify the owner that we are closing, which will subsequently call Dispose() for us
            if (!_isClosed)
            {
                _owner.OnConnectionClosing(this);
                _isClosed = true;
            }
        }
    }

    public void CloseOuterConnection(IOuterConnection outerConnection)
    {
        if (!_outerConnectionTracker.Detach(outerConnection))
        {
            return;
        }

        // NOTE: If we are orphaned, we don't have to worry about a new connection attaching in the tiny window of time between detecting
        // that we're orphaned and notifying the connection manager that we're closing, because Attach() returns false, which will prevent
        // the connection request from completing.

        // Set the outer connection closed
        outerConnection.SetClosedInnerConnection(_closedInnerConnection);

        if (_outerConnectionTracker.IsClosed)
        {
            // Shut ourselves down. No more outer connections can be attached.
            Close();
        }
    }

    public void Detach(IOuterConnection outerConnection)
    {
        if (!_outerConnectionTracker.Detach(outerConnection))
        {
            // Don't destroy our connection if the outer connection wasn't one of ours. That really shouldn't be possible but it's better
            // to be safe than sorry.

            return;
        }

        if (_outerConnectionTracker.IsClosed)
        {
            Close();
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public void StartWorkerThread()
    {
        _workerThread.Start();
    }

    public bool WaitForDataReady(CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        try
        {
            // Order is important. If both handles are set, lowest index is returned first. Prioritize cancellation.
            var waitHandles = new[] { cancellationToken.WaitHandle, _dataReadySignal };

            return WaitHandle.WaitAny(waitHandles) == 1;
        }
        catch (ObjectDisposedException)
        {
            return false;
        }
    }

    public async ValueTask<bool> WaitForDataReadyAsync(CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        try
        {
            await _asyncDataReadySignal.WaitAsync(cancellationToken).ConfigureAwait(false);

            return true;
        }
        catch (ObjectDisposedException)
        {
            return false;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
    }

    private void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                // Stop the worker thread, if it is running
                _workerThread.Dispose();

                // Dispose signals
                _asyncDataReadySignal.Dispose();
                _dataReadySignal.Dispose();

                SetTrackedConnectionsClosed();

                _dataFile.Close();
                _closedInnerConnection.Close();

                _isDisposed = true;
            }
        }
    }

    private void SetTrackedConnectionsClosed()
    {
        // Transition tracked outer connections to closed state
        foreach (var detachedOuterConnection in _outerConnectionTracker.DetachAll())
        {
            try { detachedOuterConnection.SetClosedInnerConnection(_closedInnerConnection); }
            catch
            {
                // Swallow exceptions, not that there should ever be any.
            }
        }
    }

    [StackTraceHidden]
    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
    }

    #region IConnectionWorkerThreadOwner Implementation

    void IConnectionWorkerThreadOwner.OnDataReady()
    {
        if (_isDisposed)
        {
            return;
        }

        // Unblock waiters
        _dataReadySignal.Set();
        _asyncDataReadySignal.Set();

        // Block waiters
        _dataReadySignal.Reset();
        _asyncDataReadySignal.Reset();
    }

    void IConnectionWorkerThreadOwner.OnWorkerThreadExit(bool canceled)
    {
        if (canceled)
        {
            // Worker thread should only exit with canceled = true if we're the ones who have canceled it, so don't do anything
            return;
        }

        // Prevent new connections from attaching because we are about to shut down.
        _outerConnectionTracker.Close();

        ThreadPool.QueueUserWorkItem(_ => Close(), null);
    }

    #endregion IConnectionWorkerThreadOwner Implementation
}
