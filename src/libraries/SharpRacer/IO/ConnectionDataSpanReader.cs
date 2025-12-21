using System.Runtime.InteropServices;
using SharpRacer.Interop;

namespace SharpRacer.IO;

/// <summary>
/// Provides a high-performance API for reading data from a simulator connection.
/// </summary>
public readonly ref struct ConnectionDataSpanReader : IDisposable
{
    private readonly ref readonly DataFileHeader _fileHeader;
    private readonly ConnectionDataSpanHandle _spanOwner;
    private readonly bool _isSpanOwnedByReader;

    /// <summary>
    /// Initializes an instance of the <see cref="ConnectionDataSpanReader"/> structure using the specified connection as its data source.
    /// </summary>
    /// <param name="connection">The <see cref="ISimulatorConnection"/> instance from which data will be read.</param>
    /// <exception cref="ArgumentException">
    /// <paramref name="connection"/> has not been opened.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="connection"/> is <see langword="null"/>.
    /// </exception>
    public ConnectionDataSpanReader(ISimulatorConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        // Allow reading from Closed connection since data file will be frozen and readable until all of the outer connections have closed.
        if (connection.State < SimulatorConnectionState.Open)
        {
            throw new ArgumentException("Connection has not been opened.", nameof(connection));
        }

        _spanOwner = connection.AcquireDataSpanHandle();
        _isSpanOwnedByReader = true;

        _fileHeader = ref DataFileHeader.AsRef(_spanOwner.Span);
    }

    /// <summary>
    /// Initializes an instance of the <see cref="ConnectionDataSpanReader"/> structure using the specified span as its data source.
    /// </summary>
    /// <param name="dataFileSpan">The read-only span of bytes from which data will be read.</param>
    /// <exception cref="ArgumentException">
    /// <paramref name="dataFileSpan"/> has a length less than the minimum length of <see cref="DataFileHeader.Size"/>.
    /// </exception>
    public ConnectionDataSpanReader(ReadOnlySpan<byte> dataFileSpan)
        : this(ConnectionDataSpanHandle.Ownerless(dataFileSpan), isOwnedByReader: false)
    {

    }

    /// <summary>
    /// Initializes an instance of the <see cref="ConnectionDataSpanReader"/> structure using the specified data span handle, optionally
    /// releasing the handle when the reader is disposed.
    /// </summary>
    /// <param name="spanHandle">The <see cref="ConnectionDataSpanHandle"/> acquired from the connection.</param>
    /// <param name="isOwnedByReader">
    /// If <see langword="true"/>, disposing the <see cref="ConnectionDataSpanReader"/> will cause <paramref name="spanHandle"/> to be
    /// disposed as well. Use this when you do not need to retain a reference to <paramref name="spanHandle"/> after initializing the
    /// <see cref="ConnectionDataSpanReader"/> structure.
    /// </param>
    /// <exception cref="ArgumentException">
    /// The span pointed to by <paramref name="spanHandle"/> has a length less than the minimum length of <see cref="DataFileHeader.Size"/>.
    /// </exception>
    public ConnectionDataSpanReader(ConnectionDataSpanHandle spanHandle, bool isOwnedByReader)
    {
        if (spanHandle.Span.Length < DataFileHeader.Size)
        {
            throw new ArgumentException(
                $"The data file span has length {spanHandle.Span.Length}, which is less than the minimum length {DataFileHeader.Size}",
                nameof(spanHandle));
        }

        _spanOwner = spanHandle;
        _isSpanOwnedByReader = isOwnedByReader;

        _fileHeader = ref DataFileHeader.AsRef(_spanOwner.Span);
    }

    /// <summary>
    /// Releases the underlying data file span handle.
    /// </summary>
    public readonly void Dispose()
    {
        if (_isSpanOwnedByReader)
        {
            _spanOwner.Dispose();
        }
    }

    /// <summary>
    /// Gets the length, in bytes, of the telemetry data buffers in the data file.
    /// </summary>
    /// <returns>The length, in bytes, of the telemetry data buffers.</returns>
    public readonly int GetDataBufferLength()
    {
        return _fileHeader.DataBufferElementLength;
    }

    /// <summary>
    /// Gets a read-only reference to the <see cref="DataFileHeader"/> structure in the data file.
    /// </summary>
    /// <returns>The read-only reference to the <see cref="DataFileHeader"/> structure.</returns>
    /// <example>
    /// This method must be called with the <c>ref</c> modifier in order to receive a reference to the structure and not a by-value copy.
    /// <code>
    ///     ref readonly var dataFileHeader = ref dataFileSpanReader.GetHeaderRef();
    /// </code>
    /// </example>
    public readonly ref readonly DataFileHeader GetHeaderRef()
    {
        return ref _fileHeader;
    }

    /// <summary>
    /// Gets a read-only span of bytes representing the contents of the session information string.
    /// </summary>
    /// <returns>A read-only span of bytes representing the contents of the session information string.</returns>
    public readonly ReadOnlySpan<byte> GetSessionInfoStringSpan()
    {
        return _spanOwner.Span.Slice(_fileHeader.SessionInfoOffset, _fileHeader.SessionInfoLength);
    }

    /// <summary>
    /// Reads the active telemetry data buffer from the data file and returns it as a byte array.
    /// </summary>
    /// <returns>A byte array containing the contents of the active telemetry data buffer.</returns>
    public readonly byte[] ReadActiveDataBuffer()
    {
        return ReadActiveDataBuffer(out _);
    }

    /// <summary>
    /// Reads the active telemetry data buffer from the data file and returns it as a byte array.
    /// </summary>
    /// <param name="tickCount">The tick value of the data buffer.</param>
    /// <returns>A byte array containing the contents of the active telemetry data buffer.</returns>
    public readonly byte[] ReadActiveDataBuffer(out int tickCount)
    {
        using var reader = new TelemetryBufferReader(_spanOwner.Span);

        var buffer = new byte[reader.BufferLength];
        bool copied = false;

        do
        {
            copied = reader.TryCopyActiveBuffer(buffer, out tickCount);
        }
        while (!copied);

        return buffer;
    }

    /// <summary>
    /// Copies the active telemetry data buffer from the data file into the specified span.
    /// </summary>
    /// <param name="destination">A span of bytes into which the buffer data will be copied.</param>
    public readonly void ReadActiveDataBuffer(Span<byte> destination)
    {
        ReadActiveDataBuffer(destination, out _);
    }

    /// <summary>
    /// Copies the active telemetry data buffer from the data file into the specified span.
    /// </summary>
    /// <param name="destination">A span of bytes into which the buffer data will be copied.</param>
    /// <param name="tickCount">The tick value of the data buffer.</param>
    public readonly void ReadActiveDataBuffer(Span<byte> destination, out int tickCount)
    {
        using var reader = new TelemetryBufferReader(_spanOwner.Span);

        // TODO: Destination length check?

        bool copied = false;

        do
        {
            copied = reader.TryCopyActiveBuffer(destination, out tickCount);
        }
        while (!copied);
    }

    /// <summary>
    /// Reads the array of <see cref="TelemetryVariableHeader"/> structures from the data file.
    /// </summary>
    /// <returns>An array of <see cref="TelemetryVariableHeader"/> structures.</returns>
    public readonly TelemetryVariableHeader[] ReadTelemetryVariableHeaders()
    {
        var variableHeaders = new TelemetryVariableHeader[_fileHeader.VariableCount];

        var variableHeaderBytes = _spanOwner.Span.Slice(
            _fileHeader.VariableHeaderOffset,
            _fileHeader.VariableCount * TelemetryVariableHeader.Size);

        variableHeaderBytes.CopyTo(MemoryMarshal.AsBytes((Span<TelemetryVariableHeader>)variableHeaders));

        return variableHeaders;
    }

    /// <summary>
    /// Reads the <see cref="DataFileHeader"/> structure from the data file.
    /// </summary>
    /// <returns>The <see cref="DataFileHeader"/> structure read from the data file.</returns>
    public readonly DataFileHeader ReadHeader()
    {
        return DataFileHeader.Read(_spanOwner.Span);
    }

    /// <summary>
    /// Reads the session information string from the data file.
    /// </summary>
    /// <returns>The session information string.</returns>
    public readonly string ReadSessionInfoString()
    {
        return ReadSessionInfoString(out _);
    }

    /// <summary>
    /// Reads the session information string from the data file.
    /// </summary>
    /// <param name="sessionInfoVersion">The session information version number.</param>
    /// <returns>The session information string.</returns>
    public readonly string ReadSessionInfoString(out int sessionInfoVersion)
    {
        string sessionInfoStr;

        // Read the session info string, checking that the version we started with matches the version in the header after reading
        do
        {
            sessionInfoVersion = _fileHeader.SessionInfoVersion;

            var span = _spanOwner.Span.Slice(_fileHeader.SessionInfoOffset, _fileHeader.SessionInfoLength);

            sessionInfoStr = SessionInfoString.Encoding.GetString(span);
        }
        while (sessionInfoVersion != _fileHeader.SessionInfoVersion);

        return sessionInfoStr;
    }
}
