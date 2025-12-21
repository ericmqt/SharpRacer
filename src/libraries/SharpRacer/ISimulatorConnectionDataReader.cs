using SharpRacer.Interop;

namespace SharpRacer;

/// <summary>
/// Reads data from a simulator connection.
/// </summary>
public interface ISimulatorConnectionDataReader : IDisposable
{
    /// <summary>
    /// Gets the length, in bytes, of the telemetry data buffers.
    /// </summary>
    /// <returns>The length, in bytes, of the telemetry data buffers.</returns>
    /// <exception cref="ObjectDisposedException">The instance is disposed.</exception>
    int GetDataBufferLength();

    /// <summary>
    /// Gets a read-only reference to the <see cref="DataFileHeader"/> structure.
    /// </summary>
    /// <returns>The read-only reference to the <see cref="DataFileHeader"/> structure.</returns>
    /// <example>
    /// This method must be called with the <c>ref</c> modifier in order to receive a reference to the structure and not a by-value copy.
    /// <code>
    ///     ref readonly var dataFileHeader = ref connectionDataReader.GetHeaderRef();
    /// </code>
    /// </example>
    /// <exception cref="ObjectDisposedException">The instance is disposed.</exception>
    ref readonly DataFileHeader GetHeaderRef();

    /// <summary>
    /// Gets a read-only span of bytes representing the contents of the session information string.
    /// </summary>
    /// <returns>A read-only span of bytes representing the contents of the session information string.</returns>
    /// <exception cref="ObjectDisposedException">The instance is disposed.</exception>
    ReadOnlySpan<byte> GetSessionInfoStringSpan();

    /// <summary>
    /// Returns the active telemetry data buffer as a byte array.
    /// </summary>
    /// <returns>A byte array containing the contents of the active telemetry data buffer.</returns>
    /// <exception cref="ObjectDisposedException">The instance is disposed.</exception>
    byte[] ReadActiveTelemetryBuffer();

    /// <summary>
    /// Returns the active telemetry data buffer as a byte array.
    /// </summary>
    /// <param name="tickCount">The tick value of the data buffer.</param>
    /// <returns>A byte array containing the contents of the active telemetry data buffer.</returns>
    /// <exception cref="ObjectDisposedException">The instance is disposed.</exception>
    byte[] ReadActiveTelemetryBuffer(out int tickCount);

    /// <summary>
    /// Copies the active telemetry data buffer into the specified span.
    /// </summary>
    /// <param name="destination">A span of bytes into which the buffer data will be copied.</param>
    /// <exception cref="ObjectDisposedException">The instance is disposed.</exception>
    void ReadActiveTelemetryBuffer(Span<byte> destination);

    /// <summary>
    /// Copies the active telemetry data buffer into the specified span.
    /// </summary>
    /// <param name="destination">A span of bytes into which the buffer data will be copied.</param>
    /// <param name="tickCount">The tick value of the data buffer.</param>
    /// <exception cref="ObjectDisposedException">The instance is disposed.</exception>
    void ReadActiveTelemetryBuffer(Span<byte> destination, out int tickCount);

    /// <summary>
    /// Returns an array of <see cref="DataVariableHeader"/> structures read from the data file.
    /// </summary>
    /// <returns>An array of <see cref="DataVariableHeader"/> structures.</returns>
    /// <exception cref="ObjectDisposedException">The instance is disposed.</exception>
    DataVariableHeader[] ReadTelemetryVariableHeaders();

    /// <summary>
    /// Reads the <see cref="DataFileHeader"/> structure from the data file.
    /// </summary>
    /// <returns>The <see cref="DataFileHeader"/> structure read from the data file.</returns>
    /// <exception cref="ObjectDisposedException">The instance is disposed.</exception>
    DataFileHeader ReadHeader();

    /// <summary>
    /// Reads the session information string.
    /// </summary>
    /// <returns>The session information string.</returns>
    /// <exception cref="ObjectDisposedException">The instance is disposed.</exception>
    string ReadSessionInfoString();

    /// <summary>
    /// Reads the session information string.
    /// </summary>
    /// <param name="sessionInfoVersion">The session information version number.</param>
    /// <returns>The session information string.</returns>
    /// <exception cref="ObjectDisposedException">The instance is disposed.</exception>
    string ReadSessionInfoString(out int sessionInfoVersion);
}
