using System.Runtime.InteropServices;
using SharpRacer.Interop;
using SharpRacer.IO;

namespace SharpRacer;
internal class SimulatorConnectionDataReader : ISimulatorConnectionDataReader
{
    private readonly ISimulatorConnection _connection;
    private readonly IConnectionDataHandle _memoryHandle;
    private bool _isDisposed;

    public SimulatorConnectionDataReader(ISimulatorConnection connection)
    {
        if (connection.State != SimulatorConnectionState.Open)
        {
            throw new ArgumentException("The connection is not open.", nameof(connection));
        }

        _connection = connection;
        _memoryHandle = _connection.AcquireDataHandle();
    }

    protected ReadOnlyMemory<byte> Memory => _memoryHandle.Memory;
    protected ReadOnlySpan<byte> Span => _memoryHandle.Memory.Span;

    /// <inheritdoc />
    public int GetDataBufferLength()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        ref readonly var header = ref GetHeaderRef();

        return header.DataBufferElementLength;
    }

    /// <inheritdoc />
    public ref readonly DataFileHeader GetHeaderRef()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return ref DataFileHeader.AsRef(Span);
    }

    /// <inheritdoc />
    public ReadOnlySpan<byte> GetSessionInfoStringSpan()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        ref readonly var header = ref GetHeaderRef();

        return Span.Slice(header.SessionInfoOffset, header.SessionInfoLength);
    }

    /// <inheritdoc />
    public byte[] ReadActiveDataBuffer()
    {
        return ReadActiveDataBuffer(out _);
    }

    /// <inheritdoc />
    public byte[] ReadActiveDataBuffer(out int tickCount)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        using var reader = new DataBufferReader(Span);

        var buffer = new byte[reader.BufferLength];
        bool copied = false;

        do
        {
            copied = reader.TryCopyActiveBuffer(buffer, out tickCount);
        }
        while (!copied);

        return buffer;
    }

    /// <inheritdoc />
    public void ReadActiveDataBuffer(Span<byte> destination)
    {
        ReadActiveDataBuffer(destination, out _);
    }

    /// <inheritdoc />
    public void ReadActiveDataBuffer(Span<byte> destination, out int tickCount)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        using var reader = new DataBufferReader(Span);

        // TODO: Destination length check?

        bool copied = false;

        do
        {
            copied = reader.TryCopyActiveBuffer(destination, out tickCount);
        }
        while (!copied);
    }

    /// <inheritdoc />
    public DataVariableHeader[] ReadDataVariableHeaders()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        ref readonly var header = ref GetHeaderRef();

        var variableHeaders = new DataVariableHeader[header.VariableCount];

        var variableHeaderBytes = Span.Slice(
            header.VariableHeaderOffset,
            header.VariableCount * DataVariableHeader.Size);

        variableHeaderBytes.CopyTo(MemoryMarshal.AsBytes<DataVariableHeader>(variableHeaders));

        return variableHeaders;
    }

    /// <inheritdoc />
    public DataFileHeader ReadHeader()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return DataFileHeader.Read(Span);
    }

    /// <inheritdoc />
    public string ReadSessionInfoString()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return ReadSessionInfoString(out _);
    }

    /// <inheritdoc />
    public string ReadSessionInfoString(out int sessionInfoVersion)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        ref readonly var dataFileHeader = ref DataFileHeader.AsRef(Span);

        string sessionInfoStr;

        // Read the session info string, checking that the version we started with matches the version in the header after reading
        do
        {
            sessionInfoVersion = dataFileHeader.SessionInfoVersion;

            var span = Span.Slice(dataFileHeader.SessionInfoOffset, dataFileHeader.SessionInfoLength);

            sessionInfoStr = SessionInfoString.Encoding.GetString(span);
        }
        while (sessionInfoVersion != dataFileHeader.SessionInfoVersion);

        return sessionInfoStr;
    }

    #region IDisposable Implementation

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                _memoryHandle.Dispose();
            }

            _isDisposed = true;
        }
    }

    #endregion IDisposable Implementation
}
