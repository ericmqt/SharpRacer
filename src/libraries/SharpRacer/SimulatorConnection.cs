using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Win32.SafeHandles;
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
    private readonly AutoResetEvent _dataReadyEvent;
    private readonly SafeWaitHandle _dataReadyEventHandle;
    private readonly DataReadySignal _dataReadySignal;
    private bool _isDisposed;
    private readonly Thread _thread;
    private readonly CancellationTokenSource _threadCancellationTokenSource;
    private readonly WaitHandle[] _waitHandles;

    /// <summary>
    /// Creates a new <see cref="SimulatorConnection"/>.
    /// </summary>
    public SimulatorConnection()
    {
        ConnectionTimeout = TimeSpan.FromSeconds(5);
        _connectionStateValue = (int)SimulatorConnectionState.Closed;

        _dataFile = new EmptySimulatorDataFile();
        _dataReadySignal = new DataReadySignal();
        _threadCancellationTokenSource = new CancellationTokenSource();

        _dataReadyEventHandle = DataReadyEventHandle.CreateSafeWaitHandle();
        _dataReadyEvent = new AutoResetEvent(false) { SafeWaitHandle = _dataReadyEventHandle };

        _waitHandles = [_dataReadyEvent, _threadCancellationTokenSource.Token.WaitHandle];

        _thread = new Thread(Thread_Run);
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
    public bool IsOpen => State == SimulatorConnectionState.Open;

    /// <inheritdoc />
    public int SessionInfoVersion { get; private set; }

    /// <inheritdoc />
    public SimulatorConnectionState State => (SimulatorConnectionState)_connectionStateValue;

    #endregion Properties

    /// <inheritdoc />
    public event EventHandler<EventArgs>? ConnectionClosed;

    /// <inheritdoc />
    public event EventHandler<EventArgs>? ConnectionOpened;

    /// <inheritdoc />
    public event EventHandler<SimulatorConnectionStateChangedEventArgs>? StateChanged;

    /// <summary>
    /// Gets a collection of <see cref="DataVariableInfo"/> objects representing the telemetry variables exposed by the simulator, or
    /// throws an exception if the connection is not open.
    /// </summary>
    /// <returns>A collection of <see cref="DataVariableInfo"/> objects from the simulator.</returns>
    /// <exception cref="InvalidOperationException">The connection is not open.</exception>
    public IEnumerable<DataVariableInfo> GetDataVariables()
    {
        if (!IsOpen)
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
    public bool WaitForDataReady()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (State == SimulatorConnectionState.Closed)
        {
            return false;
        }

        // Use the thread CancellationToken to ensure calls to this method will return in case the connection dies while waiting
        return _dataReadySignal.Wait(_threadCancellationTokenSource.Token);
    }

    /// <inheritdoc />
    public ValueTask<bool> WaitForDataReadyAsync()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (State == SimulatorConnectionState.Closed)
        {
            return ValueTask.FromResult(false);
        }

        return _dataReadySignal.WaitAsync(_threadCancellationTokenSource.Token);
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

        using var linkedCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(_threadCancellationTokenSource.Token, cancellationToken);

        return await _dataReadySignal.WaitAsync(linkedCancellationSource.Token).ConfigureAwait(false);
    }

    private void Thread_DataReadyLoop()
    {
        while (!_threadCancellationTokenSource.Token.IsCancellationRequested)
        {
            // Wait for data-ready signal, cancellation, or timeout
            var waitIndex = WaitHandle.WaitAny(_waitHandles, _connectionTimeoutMs);

            // Toggle IsActive based on current header value
            IsActive = MemoryMarshal.Read<int>(Data.Slice(DataFileHeader.FieldOffsets.Status, sizeof(int))) == 1;

            if (waitIndex != 0)
            {
                // Canceled or timed out
                break;
            }

            SessionInfoVersion = MemoryMarshal.Read<int>(Data.Slice(DataFileHeader.FieldOffsets.SessionInfoVersion, sizeof(int)));

            // Signal to waiters
            _dataReadySignal.Set(autoReset: true);
        }
    }

    private void Thread_Run()
    {
        Thread.CurrentThread.Name = $"{nameof(SimulatorConnection)}_Thread";

        var cancellationToken = _threadCancellationTokenSource.Token;

        try
        {
            SetState(SimulatorConnectionState.Connecting);

            // Wait for the first data-ready signal, then acquire the data file
            if (!Thread_InitializeConnection())
            {
                // Canceled or data file acquisition failed
                Thread_Stop();

                return;
            }

            SetState(SimulatorConnectionState.Open);
            ConnectionOpened?.Invoke(this, EventArgs.Empty);

            Thread_DataReadyLoop();

            Thread_Stop();
        }
        catch
        {
            // TODO: Probably want some kind of ConnectionThreadException event
            Thread_Stop();
        }
    }

    private bool Thread_InitializeConnection()
    {
        if (WaitHandle.WaitAny(_waitHandles) != 0)
        {
            // Canceled while waiting
            return false;
        }

        // Acquire the data file
        ISimulatorDataFile dataFile;

        try { dataFile = SimulatorDataFile.Open(); }
        catch
        {
            // Fail-safe catch handler for the unlikely scenario where the connection was opened on the very last signal sent by the
            // simulator before closing and the data file closed before we could acquire it.

            return false;
        }

        // Atomically swap data file references
        var oldDataFile = Interlocked.Exchange(ref _dataFile, dataFile);

        // Dispose the placeholder data file
        oldDataFile.Dispose();

        return true;
    }

    private void Thread_Stop()
    {
        IsActive = false;

        _threadCancellationTokenSource.Cancel();

        SetState(SimulatorConnectionState.Closed);

        ConnectionClosed?.Invoke(this, EventArgs.Empty);
    }

    private void OnStateChanged(SimulatorConnectionState newState, SimulatorConnectionState oldState)
    {
        StateChanged?.Invoke(this, new SimulatorConnectionStateChangedEventArgs(newState, oldState));
    }

    private void SetState(SimulatorConnectionState state)
    {
        var oldState = (SimulatorConnectionState)Interlocked.Exchange(ref _connectionStateValue, (int)state);

        if (state != oldState)
        {
            OnStateChanged(state, oldState);
        }
    }

    #region IDisposable Implementation

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                IsActive = false;

                _threadCancellationTokenSource.Cancel();
                _thread.Join();

                _threadCancellationTokenSource.Dispose();

                _dataFile.Dispose();
                _dataReadySignal.Dispose();

                _dataReadyEvent.Dispose();
                _dataReadyEventHandle.Dispose();
            }

            _isDisposed = true;
        }
    }

    #endregion IDisposable Implementation
}
