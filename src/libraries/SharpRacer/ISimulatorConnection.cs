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
    event EventHandler<EventArgs>? ConnectionClosed;

    /// <summary>
    /// Raised when the connection to the simulator is established.
    /// </summary>
    event EventHandler<EventArgs>? ConnectionOpened;

    /// <summary>
    /// Raised when the state of the connection has changed.
    /// </summary>
    event EventHandler<SimulatorConnectionStateChangedEventArgs>? StateChanged;

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
