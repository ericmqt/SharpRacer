using SharpRacer.Telemetry.Variables;

namespace SharpRacer;

/// <summary>
/// Represents a connection to a simulator session.
/// </summary>
public interface ISimulatorConnection : IDataVariableInfoProvider, IDisposable
{
    /// <summary>
    /// Gets or sets the amount of time to elapse without receiving a signal before the simulator signal is considered timed out.
    /// </summary>
    /// <remarks>
    /// Avoid using a sufficiently large timeout value that would allow the simulator to close and relaunch within the timeout period.
    /// Objects that depend upon session-specific data, such as telemetry variable contexts, could enter an invalid state.
    /// </remarks>
    TimeSpan ConnectionTimeout { get; set; }

    /// <summary>
    /// Gets a read-only span of bytes over the simulator data file.
    /// </summary>
    ReadOnlySpan<byte> Data { get; }

    /// <summary>
    /// Gets a value indicating whether the simulator has an active status.
    /// </summary>
    bool IsActive { get; }

    /// <summary>
    /// Gets a value indicating if the connection is open.
    /// </summary>
    bool IsOpen { get; }

    /// <summary>
    /// Gets a value indicating the current state of the connection.
    /// </summary>
    SimulatorConnectionState State { get; }

    /// <summary>
    /// Raised when the connection has closed.
    /// </summary>
    event EventHandler<EventArgs>? Closed;

    /// <summary>
    /// Raised when the state of the connection has changed.
    /// </summary>
    event EventHandler<SimulatorConnectionStateChangedEventArgs>? StateChanged;

    /// <summary>
    /// Closes the connection to the simulator. Once closed, a connection may not be reused.
    /// </summary>
    void Close();

    /// <summary>
    /// Asynchronously opens a connection to the simulator. If the simulator is not already active, the operation will wait until a
    /// connection is established or cancellation is requested. If the operation fails, this method may be called again.
    /// </summary>
    /// <remarks>
    /// Concurrent calls to any overload of this method should be avoided, but are allowed and will return a cached <see cref="Task"/>
    /// representing the connection operation already in progress. Only the timeout period and <see cref="CancellationToken"/> of the
    /// originating call will be used for termination.
    /// </remarks>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to use for cancelling the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">The connection is closed. Closed connections cannot be reopened.</exception>
    /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
    /// <exception cref="FileNotFoundException">
    /// Simulator data file acquisition failed after receiving a data-ready signal from the simulator. This could occur in the unlikely
    /// event that this method was called just prior to the simulator closing.
    /// </exception>
    Task OpenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously opens a connection to the simulator. If the simulator is not already active, the operation will wait until a
    /// connection is established, the timeout period elapses, or cancellation is requested. If the operation fails, this method may be
    /// called again.
    /// </summary>
    /// <remarks>
    /// Concurrent calls to any overload of this method should be avoided, but are allowed and will return a cached <see cref="Task"/>
    /// representing the connection operation already in progress. Only the timeout period and <see cref="CancellationToken"/> of the
    /// originating call will be used for termination.
    /// </remarks>
    /// <param name="timeout">The amount of time to wait for a connection before throwing <see cref="TimeoutException"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to use for cancelling the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">The connection is closed. Closed connections cannot be reopened.</exception>
    /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
    /// <exception cref="TimeoutException">The specified timeout period elapsed before a connection was established.</exception>
    /// <exception cref="FileNotFoundException">
    /// Simulator data file acquisition failed after receiving a data-ready signal from the simulator. This could occur in the unlikely
    /// event that this method was called just prior to the simulator closing.
    /// </exception>
    Task OpenAsync(TimeSpan timeout, CancellationToken cancellationToken = default);

    /// <summary>
    /// Waits for the simulator to fire the data-ready event by blocking the current thread until the event is signaled or the connection is
    /// closed. Returns <see langword="true"/> if the data-ready event was fired, otherwise <see langword="false"/>.
    /// </summary>
    /// <returns><see langword="true"/> if the data-ready event was fired, otherwise <see langword="false"/>.</returns>
    /// /// <exception cref="ObjectDisposedException">The <see cref="ISimulatorConnection"/> instance has been disposed.</exception>
    bool WaitForDataReady();

    /// <summary>
    /// Waits for the simulator to fire the data-ready event by blocking the current thread until the event is signaled or the connection
    /// is closed. Returns <see langword="true"/> if the data-ready event was fired, otherwise <see langword="false"/>.
    /// </summary>
    /// <returns><see langword="true"/> if the data-ready event was fired, otherwise <see langword="false"/>.</returns>
    /// <exception cref="ObjectDisposedException">The <see cref="ISimulatorConnection"/> instance has been disposed.</exception>
    ValueTask<bool> WaitForDataReadyAsync();

    /// <summary>
    /// Waits for the simulator to fire the data-ready event by blocking the current thread until the event is signaled, cancellation is
    /// requested, or the connection is closed. Returns <see langword="true"/> if the data-ready event was fired, otherwise <see langword="false"/>.
    /// </summary>
    /// <param name="cancellationToken">
    /// The <see cref="CancellationToken"/> to monitor for canceling the wait operation.</param>
    /// <returns><see langword="true"/> if the data-ready event was fired, otherwise <see langword="false"/>.</returns>
    /// <exception cref="ObjectDisposedException">The <see cref="ISimulatorConnection"/> instance has been disposed.</exception>
    ValueTask<bool> WaitForDataReadyAsync(CancellationToken cancellationToken);
}
