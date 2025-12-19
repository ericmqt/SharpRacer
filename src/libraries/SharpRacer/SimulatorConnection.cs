using System.Runtime.Versioning;
using SharpRacer.Internal;
using SharpRacer.Internal.Connections;
using SharpRacer.IO;
using SharpRacer.Telemetry;

namespace SharpRacer;

/// <summary>
/// Represents a connection to a simulator session.
/// </summary>
[SupportedOSPlatform("windows5.1.2600")]
public sealed class SimulatorConnection : ISimulatorConnection, IOuterConnection, IDisposable
{
    private readonly IConnectionCancellationTokenSource _cancellationTokenSource;
    private readonly IConnectionManager _connectionManager;
    private int _connectionStateValue;
    private readonly SemaphoreSlim _connectionTransitionSemaphore;
    private IInnerConnection _innerConnection;
    private bool _isDisposed;
    private readonly SemaphoreSlim _openSemaphore;
    private readonly IConnectionDataVariableInfoProvider _variableInfoProvider;

    /// <summary>
    /// Initializes an instance of <see cref="SimulatorConnection"/>.
    /// </summary>
    public SimulatorConnection()
        : this(ConnectionManager.Default, new ConnectionDataVariableInfoProvider(), new ConnectionCancellationTokenSource())
    {

    }

    internal SimulatorConnection(
        IConnectionManager connectionManager,
        IConnectionDataVariableInfoProvider variableInfoProvider,
        IConnectionCancellationTokenSource cancellationTokenSource)
    {
        _connectionManager = connectionManager ?? throw new ArgumentNullException(nameof(connectionManager));
        _variableInfoProvider = variableInfoProvider ?? throw new ArgumentNullException(nameof(variableInfoProvider));
        _cancellationTokenSource = cancellationTokenSource ?? throw new ArgumentNullException(nameof(cancellationTokenSource));

        _connectionStateValue = (int)SimulatorConnectionState.None;
        _innerConnection = new IdleInnerConnection();

        _connectionTransitionSemaphore = new SemaphoreSlim(1, 1);
        _openSemaphore = new SemaphoreSlim(1, 1);
    }

    /// <inheritdoc />
    public bool CanRead
    {
        get => State == SimulatorConnectionState.Open;
    }

    /// <inheritdoc />
    public IEnumerable<DataVariableInfo> DataVariables => _variableInfoProvider.DataVariables;

    /// <inheritdoc />
    public SimulatorConnectionState State => (SimulatorConnectionState)_connectionStateValue;

    /// <inheritdoc />
    public event EventHandler<EventArgs>? Closed;

    /// <inheritdoc />
    public event EventHandler<EventArgs>? Opened;

    /// <inheritdoc />
    public event EventHandler<SimulatorConnectionStateChangedEventArgs>? StateChanged;

    /// <inheritdoc />
    public IConnectionDataHandle AcquireDataHandle()
    {
        return _innerConnection.AcquireDataHandle();
    }

    /// <inheritdoc />
    public ConnectionDataSpanHandle AcquireDataSpanHandle()
    {
        return _innerConnection.AcquireDataSpanHandle();
    }

    /// <inheritdoc />
    public void Close()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (State == SimulatorConnectionState.None || State == SimulatorConnectionState.Connecting)
        {
            throw new InvalidOperationException(
                $"{nameof(Close)} cannot be called when {nameof(State)} is {SimulatorConnectionState.None} or {SimulatorConnectionState.Connecting}.");
        }

        if (State == SimulatorConnectionState.Closed)
        {
            return;
        }

        // The inner connection will call SetClosedInnerConnection for us
        _innerConnection.CloseOuterConnection(this);
    }

    ///<inheritdoc />
    public ISimulatorConnectionDataReader CreateDataReader()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (State != SimulatorConnectionState.Open)
        {
            throw new InvalidOperationException("The connection is not open.");
        }

        return new SimulatorConnectionDataReader(this);
    }

    /// <summary>
    /// Releases resources owned by this instance.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    /// <exception cref="ObjectDisposedException">The connection is disposed.</exception>
    public void NotifyDataVariableActivated(string variableName, Action<DataVariableInfo> callback)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        ArgumentException.ThrowIfNullOrEmpty(variableName);
        ArgumentNullException.ThrowIfNull(callback);

        _variableInfoProvider.NotifyDataVariableActivated(variableName, callback);
    }

    /// <inheritdoc />
    public void Open()
    {
        Open(Timeout.InfiniteTimeSpan);
    }

    /// <inheritdoc />
    public void Open(TimeSpan timeout)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (State == SimulatorConnectionState.Open)
        {
            return;
        }

        if (State == SimulatorConnectionState.Closed)
        {
            throw new InvalidOperationException("The connection is closed and may not be reused.");
        }

        if (!_openSemaphore.Wait(0))
        {
            throw new InvalidOperationException($"A call to {nameof(Open)} or {nameof(OpenAsync)} is already in progress.");
        }

        try
        {
            try
            {
                SetState(SimulatorConnectionState.Connecting);

                _connectionManager.Connect(this, timeout);
            }
            catch (Exception)
            {
                SetState(SimulatorConnectionState.None);

                throw;
            }
        }
        finally
        {
            _openSemaphore.Release();
        }
    }

    /// <inheritdoc />
    public Task OpenAsync(CancellationToken cancellationToken = default)
    {
        return OpenAsync(Timeout.InfiniteTimeSpan, cancellationToken);
    }

    /// <inheritdoc />
    public async Task OpenAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (State == SimulatorConnectionState.Open)
        {
            return;
        }

        if (State == SimulatorConnectionState.Closed)
        {
            throw new InvalidOperationException("The connection is closed and may not be reused.");
        }

        if (!_openSemaphore.Wait(0, CancellationToken.None))
        {
            throw new InvalidOperationException($"A call to {nameof(Open)} or {nameof(OpenAsync)} is already in progress.");
        }

        try
        {
            SetState(SimulatorConnectionState.Connecting);

            using var linkedCancellationSource = _cancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            try
            {
                await _connectionManager.ConnectAsync(this, timeout, linkedCancellationSource.Token);
            }
            catch (Exception)
            {
                SetState(SimulatorConnectionState.None);

                throw;
            }
        }
        finally
        {
            _openSemaphore.Release();
        }
    }

    /// <inheritdoc />
    public bool WaitForDataReady()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (State != SimulatorConnectionState.Open || _cancellationTokenSource.Token.IsCancellationRequested)
        {
            return false;
        }

        // Ensure calls to this method will return in case the connection dies while waiting
        return _innerConnection.WaitForDataReady(_cancellationTokenSource.Token);
    }

    /// <inheritdoc />
    public async ValueTask<bool> WaitForDataReadyAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (State != SimulatorConnectionState.Open || _cancellationTokenSource.Token.IsCancellationRequested)
        {
            return false;
        }

        if (cancellationToken == default)
        {
            return await _innerConnection.WaitForDataReadyAsync(_cancellationTokenSource.Token).ConfigureAwait(false);
        }

        // Combine our cancellation token with the provided one to ensure waiters are returned false as soon as either this instance or the
        // underlying connection is closed or disposed.

        using var linkedCancellationSource = _cancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        return await _innerConnection.WaitForDataReadyAsync(linkedCancellationSource.Token).ConfigureAwait(false);
    }

    private void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            _isDisposed = true;

            if (disposing)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();

                _innerConnection.Detach(this);

                _openSemaphore.Dispose();
                _connectionTransitionSemaphore.Dispose();
            }
        }
    }

    private void SetState(SimulatorConnectionState state)
    {
        var oldState = (SimulatorConnectionState)Interlocked.Exchange(ref _connectionStateValue, (int)state);

        if (state != oldState)
        {
            StateChanged?.Invoke(this, new SimulatorConnectionStateChangedEventArgs(state, oldState));

            if (state == SimulatorConnectionState.Closed)
            {
                Closed?.Invoke(this, EventArgs.Empty);
            }
            else if (state == SimulatorConnectionState.Open)
            {
                Opened?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    void IOuterConnection.SetClosedInnerConnection(IInnerConnection internalConnection)
    {
        // This should only be called by the pool when either the internal connection has closed or when the last SimulatorConnection
        // sharing the internal connection closes.

        if (_isDisposed)
        {
            return;
        }

        _connectionTransitionSemaphore.Wait();

        try
        {
            // Cancel pending waiters
            _cancellationTokenSource.Cancel();

            Interlocked.Exchange(ref _innerConnection, internalConnection);

            SetState(SimulatorConnectionState.Closed);
        }
        finally
        {
            _connectionTransitionSemaphore.Release();
        }
    }

    void IOuterConnection.SetOpenInnerConnection(IOpenInnerConnection openInnerConnection)
    {
        // This should only be called when transitioning from Connecting -> Open

        if (_isDisposed)
        {
            return;
        }

        _connectionTransitionSemaphore.Wait();

        try
        {
            if (State != SimulatorConnectionState.Connecting)
            {
                return;
            }

            Interlocked.Exchange(ref _innerConnection, openInnerConnection);

            _variableInfoProvider.OnDataVariablesActivated(this);

            SetState(SimulatorConnectionState.Open);
        }
        finally
        {
            _connectionTransitionSemaphore.Release();
        }
    }
}
