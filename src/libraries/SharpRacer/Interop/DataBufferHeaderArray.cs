using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpRacer.Interop;

/// <summary>
/// Inline <see cref="DataBufferHeader"/> array of <see cref="DataFileConstants.MaxDataVariableBuffers"/> elements.
/// </summary>
[InlineArray(DataFileConstants.MaxDataVariableBuffers)]
public struct DataBufferHeaderArray
{
    /// <summary>
    /// The length, in bytes, of an instance of <see cref="DataBufferHeaderArray"/>.
    /// </summary>
    public const int Size = DataBufferHeader.Size * DataFileConstants.MaxDataVariableBuffers;

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Field is required for InlineArray")]
    [SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Inline arrays do not allow readonly fields.")]
    private DataBufferHeader _first;

    internal static DataBufferHeaderArray FromArray(DataBufferHeader[] headerArray)
    {
        if (headerArray.Length != 4)
        {
            throw new ArgumentException($"Value '{nameof(headerArray)}' must have a length of 4.", nameof(headerArray));
        }

        var headerBytes = MemoryMarshal.AsBytes<DataBufferHeader>(headerArray);

        return MemoryMarshal.Read<DataBufferHeaderArray>(headerBytes);
    }
}
