using SharpRacer.Interop;

namespace SharpRacer.IO;

/// <summary>
/// Provides a high-performance API for reading telemetry data buffers from a simulator connection.
/// </summary>
public readonly ref struct TelemetryBufferReader : IDisposable
{
    private readonly ref readonly DataFileHeader _fileHeader;
    private readonly ConnectionDataSpanHandle _spanHandle;

    /// <summary>
    /// Initializes an instance of the <see cref="TelemetryBufferReader"/> structure using the specified connection as its data source.
    /// </summary>
    /// <param name="connection">The <see cref="ISimulatorConnection"/> instance from which data will be read.</param>
    /// <exception cref="ArgumentException">
    /// <paramref name="connection"/> has not been opened.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="connection"/> is <see langword="null"/>.
    /// </exception>
    public TelemetryBufferReader(ISimulatorConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        if (connection.State != SimulatorConnectionState.Open)
        {
            throw new ArgumentException("The connection is not open.", nameof(connection));
        }

        _spanHandle = connection.AcquireDataSpanHandle();
        IsHandleOwnedByReader = true;

        _fileHeader = ref DataFileHeader.AsRef(_spanHandle.Span);
    }

    /// <summary>
    /// Initializes an instance of the <see cref="TelemetryBufferReader"/> structure.
    /// </summary>
    /// <param name="data">A read-only span of bytes representing the connection data.</param>
    internal TelemetryBufferReader(ReadOnlySpan<byte> data)
        : this(ConnectionDataSpanHandle.Ownerless(data), isHandleOwnedByReader: false)
    {

    }

    internal TelemetryBufferReader(ConnectionDataSpanHandle spanHandle, bool isHandleOwnedByReader)
    {
        if (spanHandle.Span.Length < DataFileHeader.Size)
        {
            throw new ArgumentException(
                $"The data span handle has length {spanHandle.Span.Length}, which is less than the minimum length {DataFileHeader.Size}",
                nameof(spanHandle));
        }

        _spanHandle = spanHandle;
        IsHandleOwnedByReader = isHandleOwnedByReader;

        _fileHeader = ref DataFileHeader.AsRef(_spanHandle.Span);
    }

    /// <summary>
    /// Gets the length, in bytes, of the buffers used for storing telemetry data.
    /// </summary>
    public readonly int BufferLength => _fileHeader.TelemetryBufferElementLength;

    internal bool IsHandleOwnedByReader { get; }

    /// <summary>
    /// Copies the active data buffer into the specified span.
    /// </summary>
    /// <param name="destination">A span of bytes into which the buffer data will be copied.</param>
    /// <param name="tickCount">The tick value of the data buffer.</param>
    public readonly void CopyActiveBuffer(Span<byte> destination, out int tickCount)
    {
        ref readonly var bufferHeader = ref GetActiveBufferHeaderRef();

        while (!TryCopyBuffer(in bufferHeader, destination, out tickCount))
        {
            bufferHeader = ref GetActiveBufferHeaderRef();
        }
    }

    /// <summary>
    /// Releases the underlying data file span handle.
    /// </summary>
    public void Dispose()
    {
        if (IsHandleOwnedByReader)
        {
            _spanHandle.Dispose();
        }
    }

    /// <summary>
    /// Gets a read-only reference to the <see cref="TelemetryBufferHeader"/> for the active data buffer.
    /// </summary>
    /// <returns>A read-only reference to the <see cref="TelemetryBufferHeader"/> in the file header with the highest tick value.</returns>
    public readonly ref readonly TelemetryBufferHeader GetActiveBufferHeaderRef()
    {
        return ref _fileHeader.TelemetryBufferHeaders[GetActiveBufferIndex()];
    }

    /// <summary>
    /// Gets the index of the active data buffer, which is the buffer with the highest tick value.
    /// </summary>
    /// <returns>The index of the active data buffer.</returns>
    public readonly int GetActiveBufferIndex()
    {
        int activeBufferIndex = 0;
        int activeBufferTickCount = _fileHeader.TelemetryBufferHeaders[0].TickCount;

        for (int i = 1; i < _fileHeader.TelemetryBufferCount; i++)
        {
            int tickCount = _fileHeader.TelemetryBufferHeaders[i].TickCount;

            if (Math.Max(tickCount, activeBufferTickCount) != activeBufferTickCount)
            {
                activeBufferIndex = i;
                activeBufferTickCount = tickCount;
            }
        }

        return activeBufferIndex;
    }

    /// <summary>
    /// Gets a read-only reference to the <see cref="TelemetryBufferHeader"/> at the specified index.
    /// </summary>
    /// <param name="index">The index of the data buffer.</param>
    /// <returns>A read-only reference to the <see cref="TelemetryBufferHeader"/> at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="index"/> is less than zero.
    /// 
    /// -OR-
    /// 
    /// <paramref name="index"/> is greater than or equal to the <see cref="DataFileConstants.MaxTelemetryBuffers"/>.
    /// </exception>
    public readonly ref readonly TelemetryBufferHeader GetBufferHeaderRef(int index)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(index, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, DataFileConstants.MaxTelemetryBuffers);

        return ref _fileHeader.TelemetryBufferHeaders[index];
    }

    /// <summary>
    /// Attempts to copy the specified telemetry data buffer into the destination span, ensuring that the buffer contents were not
    /// overwritten by the simulator during the operation.
    /// </summary>
    /// <param name="telemetryBufferHeader">
    /// The read-only reference to the <see cref="TelemetryBufferHeader"/> representing the buffer to copy into the destination span.
    /// </param>
    /// <param name="destination">A span of bytes into which the buffer data will be copied.</param>
    /// <param name="tickCount">The tick value of the data buffer.</param>
    /// <returns>
    /// <see langword="true"/> if the buffer was not overwritten by the simulator during the operation, otherwise <see langword="false"/>.
    /// </returns>
    public readonly bool TryCopyBuffer(ref readonly TelemetryBufferHeader telemetryBufferHeader, Span<byte> destination, out int tickCount)
    {
        // Store tick count before copying so we can check to see if it changed afterwards, which would indicate the buffer was overwritten
        // and the operation fails.
        tickCount = telemetryBufferHeader.TickCount;

        _spanHandle.Span.Slice(telemetryBufferHeader.BufferOffset, _fileHeader.TelemetryBufferElementLength).CopyTo(destination);

        return tickCount == telemetryBufferHeader.TickCount;
    }

    /// <summary>
    /// Attempts to copy the active telemetry data buffer into the destination span, ensuring that the buffer contents were not overwritten
    /// by the simulator during the operation.
    /// </summary>
    /// <param name="destination">A span of bytes into which the buffer data will be copied.</param>
    /// <param name="tickCount">The tick value of the data buffer.</param>
    /// <returns>
    /// <see langword="true"/> if the buffer was not overwritten by the simulator during the operation, otherwise <see langword="false"/>.
    /// </returns>
    public readonly bool TryCopyActiveBuffer(Span<byte> destination, out int tickCount)
    {
        return TryCopyBuffer(in GetActiveBufferHeaderRef(), destination, out tickCount);
    }
}
