using System.Runtime.InteropServices;
using SharpRacer.IO;
using SharpRacer.Simulator.Interop;

namespace SharpRacer.Simulator;
internal sealed class SimulatorConnection : ISimulatorConnection
{
    private int _connectionTimeoutMs;
    private readonly ISimulatorDataFile _dataFile;
    private readonly DataReadySignal _dataReadySignal;
    private bool _isDisposed;
    private readonly Thread _thread;
    private readonly CancellationTokenSource _threadCancellationTokenSource;

    internal SimulatorConnection(ISimulatorDataFile dataFile)
    {
        _dataFile = dataFile ?? throw new ArgumentNullException(nameof(dataFile));
        _dataReadySignal = new DataReadySignal();

        ConnectionTimeout = TimeSpan.FromSeconds(5);

        // Start the thread
        _thread = new Thread(ConnectionWorker);
        _threadCancellationTokenSource = new CancellationTokenSource();

        _thread.Start();
    }

    #region Properties

    /// <inheritdoc />
    public TimeSpan ConnectionTimeout
    {
        get => TimeSpan.FromMilliseconds(_connectionTimeoutMs);
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

            Interlocked.Exchange(ref _connectionTimeoutMs, (int)value.TotalMilliseconds);
        }
    }

    /// <inheritdoc />
    public ReadOnlySpan<byte> Data => _dataFile.Span;

    /// <inheritdoc />
    public bool IsActive { get; private set; }

    /// <inheritdoc />
    public bool IsOpen { get; private set; }

    /// <inheritdoc />
    public int SessionInfoVersion { get; private set; }

    #endregion Properties

    public event EventHandler<EventArgs>? ConnectionClosed;

    /// <inheritdoc />
    public bool WaitForDataReady()
    {
        ThrowIfDisposed();

        if (!IsOpen)
        {
            return false;
        }

        // Use the thread CancellationToken to ensure calls to this method will return in case the connection dies while waiting
        return _dataReadySignal.Wait(_threadCancellationTokenSource.Token);
    }

    /// <inheritdoc />
    public ValueTask<bool> WaitForDataReadyAsync()
    {
        ThrowIfDisposed();

        if (!IsOpen)
        {
            return ValueTask.FromResult(false);
        }

        return _dataReadySignal.WaitAsync(_threadCancellationTokenSource.Token);
    }

    /// <inheritdoc />
    public async ValueTask<bool> WaitForDataReadyAsync(CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        if (!IsOpen)
        {
            return false;
        }

        // Combine our event monitor thread cancellation token with the provided one to ensure waiters are returned false as soon as
        // the the connection is closed.

        using var linkedCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(_threadCancellationTokenSource.Token, cancellationToken);

        return await _dataReadySignal.WaitAsync(linkedCancellationSource.Token).ConfigureAwait(false);
    }

    private void ConnectionWorker()
    {
        Thread.CurrentThread.Name = $"{nameof(SimulatorConnection)}_{nameof(ConnectionWorker)}";

        var cancellationToken = _threadCancellationTokenSource.Token;

        IsOpen = true;

        // Set up the event waiter and our wait handle array
        using var dataReadyEventHandle = DataReadyEventHandle.CreateSafeWaitHandle();
        using var dataReadyEvent = new AutoResetEvent(false) { SafeWaitHandle = dataReadyEventHandle };
        var waitHandles = new WaitHandle[] { dataReadyEvent, cancellationToken.WaitHandle };

        while (!cancellationToken.IsCancellationRequested)
        {
            // Wait for data-ready signal, cancellation, or timeout
            var waitIndex = WaitHandle.WaitAny(waitHandles, _connectionTimeoutMs);

            var header = MemoryMarshal.Read<DataFileHeader>(_dataFile.Span[..DataFileHeader.Size]);

            // Toggle IsActive based on current header value
            IsActive = header.Status == 1;

            if (waitIndex == 0)
            {
                if (header.SessionInfoVersion != SessionInfoVersion)
                {
                    // TODO: Update or event maybe?
                }

                // TODO: Signal to waiters
            }
            else
            {
                // Cancelled or timed out
                IsOpen = false;
                IsActive = false;

                break;
            }
        }

        IsOpen = false;
        ConnectionClosed?.Invoke(this, EventArgs.Empty);
    }

    private void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                IsActive = false;
                IsOpen = false;

                _threadCancellationTokenSource.Cancel();
                _thread.Join();

                _threadCancellationTokenSource.Dispose();
                _dataFile.Dispose();
                _dataReadySignal.Dispose();
            }

            _isDisposed = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void ThrowIfDisposed()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException(nameof(SimulatorConnection));
        }
    }
}
