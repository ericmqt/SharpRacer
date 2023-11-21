namespace SharpRacer.Simulator;

/// <summary>
/// Represents a connection to a simulator session.
/// </summary>
public interface ISimulatorConnection : IDisposable
{
    /// <summary>
    /// Gets or sets the amount of time to elapse without receiving a signal before the simulator signal is considered timed out.
    /// </summary>
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
    /// Raised when the connection has closed.
    /// </summary>
    event EventHandler<EventArgs>? ConnectionClosed;

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
