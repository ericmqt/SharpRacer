using SharpRacer.IO;

namespace SharpRacer.Telemetry;

/// <summary>
/// Reads telemetry data buffers from a simulator connection.
/// </summary>
public class TelemetryDataReader
{
    private int _bufferLength;
    private readonly ISimulatorConnection _connection;

    /// <summary>
    /// Initializes an instance of <see cref="TelemetryDataReader"/> for reading from a connection.
    /// </summary>
    /// <param name="connection"></param>
    public TelemetryDataReader(ISimulatorConnection connection)
    {
        _connection = connection;
        _bufferLength = ReadBufferLength();
    }

    /// <summary>
    /// Gets the length, in bytes, of the buffers containing telemetry data. If the connection is in a state that does not support reading,
    /// this property returns zero.
    /// </summary>
    public int BufferLength
    {
        get
        {
            if (_bufferLength <= 0)
            {
                _bufferLength = ReadBufferLength();
            }

            return _bufferLength;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the underlying connection is in a state that supports reading.
    /// </summary>
    public bool CanRead => _connection.CanRead;

    /// <summary>
    /// Gets the index of the data buffer that contains the most recent telemetry data.
    /// </summary>
    /// <returns>The index of the active data buffer.</returns>
    /// <exception cref="InvalidOperationException">The underlying connection is not in a state that supports reading.</exception>
    public int GetActiveBufferIndex()
    {
        VerifyCanRead();

        var reader = new DataBufferReader(_connection.Data);

        return reader.GetActiveBufferIndex();
    }

    /// <summary>
    /// Reads the contents of the active telemetry data buffer from the connection and returns it as a byte array.
    /// </summary>
    /// <returns>A byte array of length <see cref="BufferLength"/> containing the contents of the telemetry data buffer.</returns>
    /// <exception cref="InvalidOperationException">The underlying connection is not in a state that supports reading.</exception>
    public byte[] Read()
    {
        return Read(out _);
    }

    /// <summary>
    /// Reads the contents of the active telemetry data buffer from the connection and returns it as a byte array.
    /// </summary>
    /// <param name="tick">The tick value of the data buffer that was read.</param>
    /// <returns>A byte array of length <see cref="BufferLength"/> containing the contents of the telemetry data buffer.</returns>
    /// <exception cref="InvalidOperationException">The underlying connection is not in a state that supports reading.</exception>
    public byte[] Read(out int tick)
    {
        VerifyCanRead();

        var reader = new DataBufferReader(_connection.Data);

        var data = new byte[BufferLength];

        reader.CopyActiveBuffer(data, out tick);

        return data;
    }

    /// <summary>
    /// Reads the contents of the active telemetry data buffer from the connection into the specified destination span.
    /// </summary>
    /// <param name="destination">A span of bytes into which the buffer data will be copied.</param>
    /// <exception cref="InvalidOperationException">The underlying connection is not in a state that supports reading.</exception>
    public void Read(Span<byte> destination)
    {
        Read(destination, out _);
    }

    /// <summary>
    /// Reads the contents of the active telemetry data buffer from the connection into the specified destination span.
    /// </summary>
    /// <param name="destination">A span of bytes into which the buffer data will be copied.</param>
    /// <param name="tick">The tick value of the data buffer that was read.</param>
    /// <exception cref="InvalidOperationException">The underlying connection is not in a state that supports reading.</exception>
    public void Read(Span<byte> destination, out int tick)
    {
        VerifyCanRead();

        var reader = new DataBufferReader(_connection.Data);

        reader.CopyActiveBuffer(destination, out tick);
    }

    private int ReadBufferLength()
    {
        if (!_connection.CanRead)
        {
            return 0;
        }

        var reader = new DataBufferReader(_connection.Data);

        return reader.BufferLength;
    }

    private void VerifyCanRead()
    {
        if (!_connection.CanRead)
        {
            throw new InvalidOperationException("The underlying connection is not in a state that supports reading.");
        }
    }
}
