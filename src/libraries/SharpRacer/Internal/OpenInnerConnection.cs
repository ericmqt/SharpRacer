using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using SharpRacer.Internal.Connections;
using SharpRacer.Interop;
using SharpRacer.IO;

namespace SharpRacer.Internal;

[SupportedOSPlatform("windows5.1.2600")]
internal sealed class OpenInnerConnection : IOpenInnerConnection
{
    private readonly DotNext.Threading.AsyncManualResetEvent _asyncDataReadySignal;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly IConnectionManager _connectionManager;
    private readonly Thread _connectionThread;
    private readonly MemoryMappedDataFile _dataFile;
    private readonly IDisposable _dataFileLifetimeHandle;
    private readonly ManualResetEvent _dataReadySignal;
    private int _idleTimeoutMs;
    private bool _isDisposed;
    private int _simulatorStatus;

    public OpenInnerConnection(
        MemoryMappedDataFile dataFile,
        int connectionId,
        IConnectionManager connectionManager)
    {
        _dataFile = dataFile ?? throw new ArgumentNullException(nameof(dataFile));
        _connectionManager = connectionManager ?? throw new ArgumentNullException(nameof(connectionManager));

        _dataFileLifetimeHandle = _dataFile.AcquireLifetimeHandle();

        ConnectionId = connectionId;
        IdleTimeout = TimeSpan.FromSeconds(5);
        State = SimulatorConnectionState.Open;

        _dataReadySignal = new ManualResetEvent(false);
        _asyncDataReadySignal = new DotNext.Threading.AsyncManualResetEvent(false);

        _cancellationTokenSource = new CancellationTokenSource();
        _connectionThread = new Thread(ConnectionWorkerThread) { IsBackground = true };
        _connectionThread.Start();
    }

    public int ConnectionId { get; }
    public ReadOnlySpan<byte> Data => _dataFile.Span;
    public ISimulatorDataFile DataFile => _dataFile;

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

    public SimulatorConnectionState State { get; }

    public void Dispose()
    {
        if (!_isDisposed)
        {
            _cancellationTokenSource.Cancel();

            if (_connectionThread.ThreadState != ThreadState.Unstarted)
            {
                _connectionThread.Join();
            }

            _asyncDataReadySignal.Dispose();
            _dataReadySignal.Dispose();
            _cancellationTokenSource.Dispose();

            // Don't dispose the data file as it is managed by the connection pool. Instead, release our lifetime handle to it and the
            // data file will dispose itself when all of the rented handles are returned.

            _dataFileLifetimeHandle.Dispose();

            _isDisposed = true;
        }

        GC.SuppressFinalize(this);
    }

    public bool WaitForDataReady(CancellationToken cancellationToken)
    {
        // Order is important. If both handles are set, lowest index is returned first. Prioritize cancellation.
        var waitHandles = new[] { cancellationToken.WaitHandle, _dataReadySignal };

        return WaitHandle.WaitAny(waitHandles) == 1;
    }

    public async ValueTask<bool> WaitForDataReadyAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _asyncDataReadySignal.WaitAsync(cancellationToken).ConfigureAwait(false);

            return true;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
    }

    private void ConnectionWorkerThread()
    {
        // Wait at most two frames before checking if sim has closed and evaluating idle timeout. Even with the latency of waking the
        // thread after the data-ready event handle is signaled, a 32ms window should be sufficient.
        const int DataReadyWaitTimeout = 2 * 16;

        using var dataReadyEventHandle = DataReadyEventHandle.CreateSafeWaitHandle();
        using var dataReadyEvent = new AutoResetEvent(false) { SafeWaitHandle = dataReadyEventHandle };

        var waitHandles = new WaitHandle[] { dataReadyEvent, _cancellationTokenSource.Token.WaitHandle };
        long lastDataReadyTimestamp = TimeProvider.System.GetTimestamp();

        while (true)
        {
            // Wait for data-ready signal, cancellation, or timeout
            var waitIndex = WaitHandle.WaitAny(waitHandles, DataReadyWaitTimeout);

            // TODO: See if simulator status is set to zero with a sync event when sim exits, then we could return to the pool without 
            // needing to wait a full two frames

            Interlocked.Exchange(ref _simulatorStatus, MemoryMarshal.Read<int>(Data.Slice(DataFileHeader.FieldOffsets.Status, sizeof(int))));

            if (waitIndex == 0)
            {
                _dataReadySignal.Set();
                _asyncDataReadySignal.Set();

                lastDataReadyTimestamp = TimeProvider.System.GetTimestamp();

                _dataReadySignal.Reset();
                _asyncDataReadySignal.Reset();
            }
            else if (waitIndex == WaitHandle.WaitTimeout)
            {
                // Close the connection if the simulator indicates it has exited, or if the idle timeout period has elapsed without
                // detecting the simulator has exited (e.g. simulator froze for too long or crashed without updating the field).

                if (_simulatorStatus == 0 || TimeProvider.System.GetElapsedTime(lastDataReadyTimestamp) >= IdleTimeout)
                {
                    // Return ourselves because we are disconnecting. Run it on the thread pool so Dispose() won't block when invoked by
                    // the pool.

                    ThreadPool.QueueUserWorkItem(_connectionManager.Return, this, false);

                    break;
                }

                // Otherwise, continue as normal.
            }
            else if (waitIndex == 1)
            {
                // Cancellation was requested.
                // Do not return to pool, cancellation is only signaled in Dispose() which should only be called by the pool

                break;
            }
        }
    }

    IDisposable IOpenInnerConnection.AcquireDataFileLifetimeHandle()
    {
        return _dataFile.AcquireLifetimeHandle();
    }
}
