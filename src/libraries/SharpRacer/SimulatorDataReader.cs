using System.Runtime.InteropServices;
using System.Text;
using SharpRacer.Interop;

namespace SharpRacer;
public readonly ref struct SimulatorDataReader
{
    private readonly ReadOnlySpan<byte> _data;

    public SimulatorDataReader(ISimulatorConnection connection)
    {
        if (connection is null)
        {
            throw new ArgumentNullException(nameof(connection));
        }

        _data = connection.Data;
    }

    public SimulatorDataReader(ReadOnlySpan<byte> data)
    {
        _data = data;
    }

    public readonly ReadOnlySpan<byte> Data => _data;

    public DataVariableHeader[] ReadDataVariableHeaders()
    {
        var count = MemoryMarshal.Read<int>(_data.Slice(DataFileHeader.FieldOffsets.VariableCount, sizeof(int)));
        var offset = MemoryMarshal.Read<int>(_data.Slice(DataFileHeader.FieldOffsets.VariableHeaderOffset, sizeof(int)));

        var headers = new DataVariableHeader[count];

        var headersBlob = _data.Slice(offset, count * DataVariableHeader.Size);

        // Copy the source data into our header array reinterpreted as a span of bytes
        headersBlob.CopyTo(MemoryMarshal.AsBytes<DataVariableHeader>(headers));

        return headers;
    }

    public DataFileHeader ReadHeader()
    {
        return MemoryMarshal.Read<DataFileHeader>(_data[..DataFileHeader.Size]);
    }

    public string ReadSessionInfo()
    {
        var offset = MemoryMarshal.Read<int>(_data.Slice(DataFileHeader.FieldOffsets.SessionInfoOffset, sizeof(int)));
        var length = MemoryMarshal.Read<int>(_data.Slice(DataFileHeader.FieldOffsets.SessionInfoLength, sizeof(int)));
        var span = _data.Slice(offset, length);

        // SessionInfo encoding is ISO-8859-1, not UTF8. This only really matters for tracks with non-ASCII characters in their name.
        return Encoding.Latin1.GetString(span);
    }

    public int ReadSessionInfoVersion()
    {
        var slice = _data.Slice(DataFileHeader.FieldOffsets.SessionInfoVersion, sizeof(int));

        return MemoryMarshal.Read<int>(slice);
    }
}
