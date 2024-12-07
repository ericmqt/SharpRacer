using SharpRacer.Interop;

namespace SharpRacer.IO;

/// <summary>
/// Provides a high-performance API for reading telemetry data buffers from a simulator connection.
/// </summary>
public readonly ref struct DataBufferReader
{
    private readonly ref readonly DataFileHeader _fileHeader;
    private readonly ReadOnlySpan<byte> _data;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataBufferReader"/> structure.
    /// </summary>
    /// <param name="connection">The <see cref="ISimulatorConnection"/> instance from which data will be read.</param>
    public DataBufferReader(ISimulatorConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        // TODO: Validate connection is open or leave it up to callers?

        _data = connection.Data;
        _fileHeader = ref DataFileHeader.AsRef(_data);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataBufferReader"/> structure.
    /// </summary>
    /// <param name="data">A read-only span of bytes representing the connection data.</param>
    public DataBufferReader(ReadOnlySpan<byte> data)
    {
        _data = data;
        _fileHeader = ref DataFileHeader.AsRef(_data);
    }

    /// <summary>
    /// Gets the length, in bytes, of the buffers used for storing telemetry data.
    /// </summary>
    public readonly int BufferLength => _fileHeader.DataBufferElementLength;

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
    /// Gets a read-only reference to the <see cref="DataBufferHeader"/> for the active data buffer.
    /// </summary>
    /// <returns>A read-only reference to the <see cref="DataBufferHeader"/> in the file header with the highest tick value.</returns>
    public readonly ref readonly DataBufferHeader GetActiveBufferHeaderRef()
    {
        return ref _fileHeader.DataBufferHeaders[GetActiveBufferIndex()];
    }

    /// <summary>
    /// Gets the index of the active data buffer, which is the buffer with the highest tick value.
    /// </summary>
    /// <returns>The index of the active data buffer.</returns>
    public readonly int GetActiveBufferIndex()
    {
        int activeBufferIndex = -1;
        int activeBufferTickCount = -1;

        // Check each header and return the index with the highest tick count
        for (int i = 0; i < _fileHeader.DataBufferCount; i++)
        {
            ref readonly var bufferHeader = ref _fileHeader.DataBufferHeaders[i];

            if (bufferHeader.TickCount > activeBufferTickCount)
            {
                activeBufferIndex = i;
                activeBufferTickCount = bufferHeader.TickCount;
            }
        }

        return activeBufferIndex;
    }

    /// <summary>
    /// Gets a read-only reference to the <see cref="DataBufferHeader"/> at the specified index.
    /// </summary>
    /// <param name="index">The index of the data buffer.</param>
    /// <returns>A read-only reference to the <see cref="DataBufferHeader"/> at the specified index.</returns>
    public readonly ref readonly DataBufferHeader GetBufferHeaderRef(int index)
    {
        return ref _fileHeader.DataBufferHeaders[index];
    }

    /// <summary>
    /// Attempts to copy the specified data buffer into the destination span, ensuring that the buffer contents were not overwritten by the
    /// simulator during the operation.
    /// </summary>
    /// <param name="dataBufferHeader">
    /// The read-only reference to the <see cref="DataBufferHeader"/> representing the buffer to copy into the destination span.
    /// </param>
    /// <param name="destination">A span of bytes into which the buffer data will be copied.</param>
    /// <param name="tickCount">The tick value of the data buffer.</param>
    /// <returns>
    /// <see langword="true"/> if the buffer was not overwritten by the simulator during the operation, otherwise <see langword="false"/>.
    /// </returns>
    public readonly bool TryCopyBuffer(ref readonly DataBufferHeader dataBufferHeader, Span<byte> destination, out int tickCount)
    {
        // Store tick count before copying so we can check to see if it changed afterwards, which would indicate the buffer was overwritten
        // and the operation fails.
        tickCount = dataBufferHeader.TickCount;

        _data.Slice(dataBufferHeader.BufferOffset, _fileHeader.DataBufferElementLength).CopyTo(destination);

        return tickCount == dataBufferHeader.TickCount;
    }

    /// <summary>
    /// Attempts to copy the active data buffer into the destination span, ensuring that the buffer contents were not overwritten by the
    /// simulator during the operation.
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
