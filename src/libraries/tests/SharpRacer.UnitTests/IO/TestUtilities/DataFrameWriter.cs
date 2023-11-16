using System.Runtime.InteropServices;
using SharpRacer.Interop;
using SharpRacer.Telemetry;
using SharpRacer.Telemetry.Variables;

namespace SharpRacer.IO.TestUtilities;
internal class DataFrameWriter
{
    private readonly Memory<byte> _dataFrame;

    public DataFrameWriter(Memory<byte> dataFrame)
    {
        _dataFrame = dataFrame;
    }

    public void Write<T>(in DataVariableHeader header, T value)
        where T : unmanaged
    {
        if (header.Count != 1)
        {
            throw new ArgumentException("Header count must be equal to 1", nameof(header));
        }

        MemoryMarshal.Write<T>(_dataFrame.Slice(header.Offset).Span, value);
    }

    public void WriteArray<T>(in DataVariableHeader header, T[] values)
        where T : unmanaged
    {
        if (header.Count <= 1)
        {
            throw new ArgumentException("Header count must be greater than 1", nameof(header));
        }

        if (header.Count != values.Length)
        {
            throw new ArgumentException($"'{nameof(header)}' has count {header.Count} but value array has length {values.Length}.", nameof(values));
        }

        var frameSlice = _dataFrame.Slice(header.Offset, ((DataVariableValueType)header.Type).GetSize() * header.Count).Span;

        MemoryMarshal.AsBytes<T>(values).CopyTo(frameSlice);
    }
}