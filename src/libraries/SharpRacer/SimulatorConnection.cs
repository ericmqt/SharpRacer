using System.Runtime.Versioning;
using SharpRacer.Internal;
using SharpRacer.IO;
using SharpRacer.Telemetry;

namespace SharpRacer;

/// <summary>
/// Represents a connection to a simulator session.
/// </summary>
[SupportedOSPlatform("windows5.1.2600")]
public sealed class SimulatorConnection : ISimulatorConnection, IDisposable
{
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly IConnectionPool _connectionPool;
    private int _connectionStateValue;
    private readonly SemaphoreSlim _connectionTransitionSemaphore;
    private ISimulatorInternalConnection _internalConnection;
    private readonly object _internalConnectionLock = new();
    private bool _isDisposed;
    private readonly SemaphoreSlim _openSemaphore;
    private readonly ConnectionDataVariableInfoProvider _variableInfoProvider;

    /// <summary>
    /// Initializes an instance of <see cref="SimulatorConnection"/>.
    /// </summary>
    public SimulatorConnection()
        : this(ConnectionPool.Default)
    {

    }

    internal SimulatorConnection(IConnectionPool connectionPool)
    {
        _connectionPool = connectionPool ?? throw new ArgumentNullException(nameof(connectionPool));

        _cancellationTokenSource = new CancellationTokenSource();
        _connectionStateValue = (int)SimulatorConnectionState.None;
        _internalConnection = new InactiveInternalConnection(SimulatorConnectionState.None);

        _connectionTransitionSemaphore = new SemaphoreSlim(1, 1);
        _openSemaphore = new SemaphoreSlim(1, 1);

        _variableInfoProvider = new ConnectionDataVariableInfoProvider();
    }

    /// <inheritdoc />
    public ReadOnlySpan<byte> Data => _internalConnection.Data;

    /// <inheritdoc />
    public IEnumerable<DataVariableInfo> DataVariables => _variableInfoProvider.DataVariables;

    /// <inheritdoc />
    public SimulatorConnectionState State => (SimulatorConnectionState)_connectionStateValue;

    /// <inheritdoc />
    public event EventHandler<EventArgs>? Closed;

    /// <inheritdoc />
    public event EventHandler<SimulatorConnectionStateChangedEventArgs>? StateChanged;

    /// <inheritdoc />
    public void Close()
    {
        _connectionTransitionSemaphore.Wait();

        try
        {
            _cancellationTokenSource.Cancel();

            _connectionPool.ReleaseOuterConnection(this);

            // Replace the data file with an empty one, because only consumers should be calling Close(), not the internal connection, so there
            // should be no need to preserve a copy of the data file to prevent uncoordinated reads from getting garbage data.

            Interlocked.Exchange(ref _internalConnection, new InactiveInternalConnection(new FrozenDataFile([]), SimulatorConnectionState.Closed));
            SetState(SimulatorConnectionState.Closed);
            Closed?.Invoke(this, EventArgs.Empty);
        }
        finally
        {
            _connectionTransitionSemaphore.Release();
        }
    }

    public void Dispose()
    {
        if (!_isDisposed)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();

            _connectionPool.ReleaseOuterConnection(this);
            // Internal connection is disposed by the pool so there is no need to dispose of it here

            _openSemaphore.Dispose();
            _connectionTransitionSemaphore.Dispose();

            _isDisposed = true;
        }

        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public void NotifyDataVariableActivated(string variableName, Action<DataVariableInfo> callback)
    {
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

                _connectionPool.Connect(this, timeout);
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

            using var linkedCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(
                _cancellationTokenSource.Token,
                cancellationToken);

            try
            {
                await _connectionPool.ConnectAsync(this, timeout, linkedCancellationSource.Token).ConfigureAwait(false);
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
        return _internalConnection.WaitForDataReady(_cancellationTokenSource.Token);
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
            return await _internalConnection.WaitForDataReadyAsync(_cancellationTokenSource.Token).ConfigureAwait(false);
        }

        // Combine our cancellation token with the provided one to ensure waiters are returned false as soon as either this instance or the
        // underlying connection is closed or disposed.

        using var linkedCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);

        return await _internalConnection.WaitForDataReadyAsync(linkedCancellationSource.Token).ConfigureAwait(false);
    }

    internal void SetClosedInternalConnection(ISimulatorInternalConnection internalConnection)
    {
        _connectionTransitionSemaphore.Wait();

        try
        {
            Console.WriteLine($"[SetClosedInternalConnection] {State} -> {internalConnection.State}, ConnectionId: {internalConnection.ConnectionId}");

            // Cancel pending waiters
            _cancellationTokenSource.Cancel();

            Interlocked.Exchange(ref _internalConnection, internalConnection);

            SetState(SimulatorConnectionState.Closed);
        }
        finally
        {
            _connectionTransitionSemaphore.Release();
        }
    }

    internal void SetOpenInternalConnection(ISimulatorInternalConnection internalConnection)
    {
        // This should only be called when transitioning from Connecting -> Open

        _connectionTransitionSemaphore.Wait();

        try
        {
            if (State != SimulatorConnectionState.Connecting)
            {
                return;
            }

            Console.WriteLine($"[SetOpenInternalConnection] {State} -> {internalConnection.State}, ConnectionId: {internalConnection.ConnectionId}");

            Interlocked.Exchange(ref _internalConnection, internalConnection);

            _variableInfoProvider.InitializeVariables(this);

            SetState(SimulatorConnectionState.Open);
        }
        finally
        {
            _connectionTransitionSemaphore.Release();
        }
    }

    private void SetState(SimulatorConnectionState state)
    {
        var oldState = (SimulatorConnectionState)Interlocked.Exchange(ref _connectionStateValue, (int)state);

        if (state != oldState)
        {
            StateChanged?.Invoke(this, new SimulatorConnectionStateChangedEventArgs(state, oldState));
        }
    }
}
