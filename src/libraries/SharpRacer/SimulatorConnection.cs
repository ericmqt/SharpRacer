using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Nito.AsyncEx.Interop;
using SharpRacer.Internal;
using SharpRacer.Interop;
using SharpRacer.Telemetry.Variables;

namespace SharpRacer;

/// <summary>
/// Represents a connection to a simulator session.
/// </summary>
[SupportedOSPlatform("windows5.1.2600")]
public sealed class SimulatorConnection : ISimulatorConnection
{
    private int _connectionStateValue;
    private int _connectionTimeoutMs;
    private ISimulatorDataFile _dataFile;
    private readonly DataReadySignal _dataReadySignal;
    private bool _isDisposed;
    private Task? _openTask;
    private readonly object _openTaskLock = new object();
    private readonly Thread _thread;
    private readonly CancellationTokenSource _cancellationTokenSource;

    /// <summary>
    /// Initializes a new instance of <see cref="SimulatorConnection"/>.
    /// </summary>
    public SimulatorConnection()
    {
        ConnectionTimeout = TimeSpan.FromSeconds(5);

        _dataFile = new EmptySimulatorDataFile();
        _dataReadySignal = new DataReadySignal();

        _cancellationTokenSource = new CancellationTokenSource();
        _thread = new Thread(ConnectionWorkerThread);
    }

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
    public bool IsOpen => State == SimulatorConnectionState.Open;

    /// <inheritdoc />
    public SimulatorConnectionState State => (SimulatorConnectionState)_connectionStateValue;

    /// <inheritdoc />
    public event EventHandler<EventArgs>? Closed;

    /// <inheritdoc />
    public event EventHandler<SimulatorConnectionStateChangedEventArgs>? StateChanged;

    /// <inheritdoc />
    public void Close()
    {
        _cancellationTokenSource.Cancel();

        if (_thread.ThreadState != ThreadState.Unstarted)
        {
            _thread.Join();
        }

        SetState(SimulatorConnectionState.Closed);
        Closed?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Gets a collection of <see cref="DataVariableInfo"/> objects representing the telemetry variables exposed by the simulator.
    /// </summary>
    /// <returns>A collection of <see cref="DataVariableInfo"/> objects from the simulator.</returns>
    /// <exception cref="InvalidOperationException">The connection is not open.</exception>
    public IEnumerable<DataVariableInfo> GetDataVariables()
    {
        if (State != SimulatorConnectionState.Open)
        {
            throw new InvalidOperationException("The connection is not open.");
        }

        var header = MemoryMarshal.Read<DataFileHeader>(Data);

        var variableHeaders = new DataVariableHeader[header.VariableCount];
        var variableHeaderBytes = Data.Slice(header.VariableHeaderOffset, DataVariableHeader.Size * header.VariableCount);

        variableHeaderBytes.CopyTo(MemoryMarshal.AsBytes<DataVariableHeader>(variableHeaders));

        return variableHeaders.Select(x => new DataVariableInfo(x)).ToArray();
    }

    /// <inheritdoc />
    public Task OpenAsync(CancellationToken cancellationToken = default)
    {
        return OpenAsync(Timeout.InfiniteTimeSpan, cancellationToken);
    }

    /// <inheritdoc />
    public Task OpenAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        if (State == SimulatorConnectionState.Open)
        {
            return Task.CompletedTask;
        }

        if (State == SimulatorConnectionState.Closed)
        {
            throw new InvalidOperationException("The connection is closed and may not be reused.");
        }

        // Capture task reference in case it changes out from under us
        var capturedTask = _openTask;

        // Examine captured task to avoid hitting the lock if at all possible
        if (capturedTask is null || capturedTask.IsCompleted)
        {
            // Prevent concurrent call from duplicating task creation
            lock (_openTaskLock)
            {
                // Re-examine cached task
                if (_openTask is null || _openTask.IsCompleted)
                {
                    _openTask = OpenCoreAsync(timeout, cancellationToken);
                    capturedTask = _openTask;
                }
                else
                {
                    // Task has already started, update the captured task
                    capturedTask = _openTask;
                }
            }
        }

        return capturedTask;
    }

    /// <inheritdoc />
    public bool WaitForDataReady()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (State == SimulatorConnectionState.Closed)
        {
            return false;
        }

        // Use the thread CancellationToken to ensure calls to this method will return in case the connection dies while waiting
        return _dataReadySignal.Wait(_cancellationTokenSource.Token);
    }

    /// <inheritdoc />
    public ValueTask<bool> WaitForDataReadyAsync()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (State == SimulatorConnectionState.Closed)
        {
            return ValueTask.FromResult(false);
        }

        return _dataReadySignal.WaitAsync(_cancellationTokenSource.Token);
    }

    /// <inheritdoc />
    public async ValueTask<bool> WaitForDataReadyAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (State == SimulatorConnectionState.Closed)
        {
            return false;
        }

        // Combine our event monitor thread cancellation token with the provided one to ensure waiters are returned false as soon as
        // the the connection is closed.

        using var linkedCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);

        return await _dataReadySignal.WaitAsync(linkedCancellationSource.Token).ConfigureAwait(false);
    }

    private async Task OpenCoreAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        if (State == SimulatorConnectionState.Open)
        {
            return;
        }

        if (State != SimulatorConnectionState.None)
        {
            throw new InvalidOperationException(
                $"Expected property {nameof(State)} to have value {nameof(SimulatorConnectionState.None)}.");
        }

        SetState(SimulatorConnectionState.Connecting);

        try
        {
            using var dataReadyEventHandle = DataReadyEventHandle.CreateSafeWaitHandle();
            using var dataReadyEvent = new AutoResetEvent(false) { SafeWaitHandle = dataReadyEventHandle };

            if (!await WaitHandleAsyncFactory.FromWaitHandle(dataReadyEvent, timeout, cancellationToken).ConfigureAwait(false))
            {
                throw new TimeoutException("The timeout period elapsed before a connection could be established.");
            }

            // Acquire the data file
            var dataFile = SimulatorDataFile.Open();

            // Atomically swap data file references and dispose of the placeholder file
            var oldDataFile = Interlocked.Exchange(ref _dataFile, dataFile);

            oldDataFile.Dispose();
        }
        catch
        {
            // Atomically change the state back to None without raising the StateChanged event before re-throwing to allow caller to
            // re-attempt opening the connection.

            Interlocked.Exchange(ref _connectionStateValue, (int)SimulatorConnectionState.None);

            throw;
        }

        // Start the worker
        _thread.Start();

        SetState(SimulatorConnectionState.Open);
    }

    private void ConnectionWorkerThread()
    {
        using var dataReadyEventHandle = DataReadyEventHandle.CreateSafeWaitHandle();
        using var dataReadyEvent = new AutoResetEvent(false) { SafeWaitHandle = dataReadyEventHandle };

        var waitHandles = new WaitHandle[] { dataReadyEvent, _cancellationTokenSource.Token.WaitHandle };

        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            // Wait for data-ready signal, cancellation, or timeout
            var waitIndex = WaitHandle.WaitAny(waitHandles, _connectionTimeoutMs);

            IsActive = MemoryMarshal.Read<int>(Data.Slice(DataFileHeader.FieldOffsets.Status, sizeof(int))) != 0;

            if (waitIndex == WaitHandle.WaitTimeout)
            {
                // Cancel any pending data-ready waiters
                _cancellationTokenSource.Cancel();

                SetState(SimulatorConnectionState.Closed);
                Closed?.Invoke(this, EventArgs.Empty);

                break;
            }

            if (waitIndex != 0)
            {
                // Canceled
                break;
            }

            _dataReadySignal.Set(autoReset: true);
        }

        // Do not set Closed state or fire any events here, as that should only happen on timeout. Those actions occur during calls to
        // Close() or Dispose(). Otherwise an event handler could block and prevent timely joining of the thread.
    }

    private void SetState(SimulatorConnectionState state)
    {
        var oldState = (SimulatorConnectionState)Interlocked.Exchange(ref _connectionStateValue, (int)state);

        if (state != oldState)
        {
            StateChanged?.Invoke(this, new SimulatorConnectionStateChangedEventArgs(state, oldState));
        }
    }

    private void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                Interlocked.Exchange(ref _connectionStateValue, (int)SimulatorConnectionState.Closed);

                _cancellationTokenSource.Cancel();

                if (_thread.ThreadState != ThreadState.Unstarted)
                {
                    _thread.Join();
                }

                _cancellationTokenSource.Dispose();

                _dataFile.Dispose();
                _dataReadySignal.Dispose();
            }

            _isDisposed = true;
        }
    }

    /// <summary>
    /// Releases resources used by <see cref="SimulatorConnection"/>.
    /// </summary>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
