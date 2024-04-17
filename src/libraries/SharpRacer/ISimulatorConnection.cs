using SharpRacer.Telemetry;

namespace SharpRacer;

/// <summary>
/// Represents a connection to a simulator session.
/// </summary>
public interface ISimulatorConnection : IDataVariableInfoProvider, IDisposable
{
    /// <summary>
    /// Gets a read-only span of bytes over the simulator data file.
    /// </summary>
    ReadOnlySpan<byte> Data { get; }

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
    /// Closes the connection to the simulator. Once closed, the connection may not be reused.
    /// </summary>
    void Close();

    /// <summary>
    /// Opens a connection to the simulator.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// The connection is closed. Closed connections cannot be reopened.
    /// 
    /// -OR-
    /// 
    /// A call to Open or OpenAsync is already in progress.
    /// </exception>
    /// <exception cref="SimulatorConnectionException">An exception was thrown while connecting to the simulator.</exception>
    void Open();

    /// <summary>
    /// Opens a connection to the simulator.
    /// </summary>
    /// <param name="timeout">
    /// The length of time to wait for the connection to be established.
    /// 
    /// If the value is <see cref="Timeout.InfiniteTimeSpan"/>, the method does not return until a connection is established, a connection
    /// exception is thrown, or the <see cref="ISimulatorConnection"/> instance is disposed.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// The connection is closed. Closed connections cannot be reopened.
    /// 
    /// -OR-
    /// 
    /// A call to Open or OpenAsync is already in progress.
    /// </exception>
    /// <exception cref="SimulatorConnectionException">
    /// An exception was thrown while connecting to the simulator.
    /// 
    /// -OR-
    /// 
    /// Value for parameter <paramref name="timeout"/> was <see cref="TimeSpan.Zero"/> and a connection was not immediately obtained.
    /// </exception>
    /// <exception cref="TimeoutException">The timeout period elapsed before a connection was established.</exception>
    void Open(TimeSpan timeout);

    /// <summary>
    /// Asynchronously opens a connection to the simulator.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for canceling the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">
    /// The connection is closed. Closed connections cannot be reopened.
    /// 
    /// -OR-
    /// 
    /// A call to Open or OpenAsync is already in progress.
    /// </exception>
    /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
    /// <exception cref="SimulatorConnectionException">An exception was thrown while connecting to the simulator.</exception>
    Task OpenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously opens a connection to the simulator.
    /// </summary>
    /// <param name="timeout">
    /// The length of time to wait for the connection to be established.
    /// 
    /// If the value is <see cref="Timeout.InfiniteTimeSpan"/>, the <see cref="Task"/> is not completed until a connection is established,
    /// a connection exception is thrown, or the <see cref="SimulatorConnection"/> instance is disposed.
    /// </param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for canceling the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">
    /// The connection is closed. Closed connections cannot be reopened.
    /// 
    /// -OR-
    /// 
    /// A call to Open or OpenAsync is already in progress.
    /// </exception>
    /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
    /// <exception cref="SimulatorConnectionException">
    /// An exception was thrown while connecting to the simulator.
    /// 
    /// -OR-
    /// 
    /// Value for parameter <paramref name="timeout"/> was <see cref="TimeSpan.Zero"/> and a connection was not immediately obtained.
    /// </exception>
    /// <exception cref="TimeoutException">The timeout period elapsed before a connection was established.</exception>
    Task OpenAsync(TimeSpan timeout, CancellationToken cancellationToken = default);

    /// <summary>
    /// Waits for the simulator to raise the data-ready event. Returns <see langword="true"/> if the data-ready event was raised,
    /// otherwise <see langword="false"/>.
    /// </summary>
    /// <returns><see langword="true"/> if the data-ready event was raised, otherwise <see langword="false"/>.</returns>
    /// /// <exception cref="ObjectDisposedException">The <see cref="ISimulatorConnection"/> instance has been disposed.</exception>
    bool WaitForDataReady();

    /// <summary>
    /// Asynchronously waits for the simulator to raise the data-ready event. Returns <see langword="true"/> if the data-ready event was
    /// raised, otherwise <see langword="false"/>.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for canceling the wait operation.</param>
    /// <returns><see langword="true"/> if the data-ready event was raised, otherwise <see langword="false"/>.</returns>
    /// <exception cref="ObjectDisposedException">The <see cref="ISimulatorConnection"/> instance has been disposed.</exception>
    ValueTask<bool> WaitForDataReadyAsync(CancellationToken cancellationToken = default);
}
