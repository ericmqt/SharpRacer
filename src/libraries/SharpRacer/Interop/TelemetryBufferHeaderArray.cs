using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpRacer.Interop;

/// <summary>
/// Inline <see cref="TelemetryBufferHeader"/> array of <see cref="DataFileConstants.MaxTelemetryBuffers"/> elements.
/// </summary>
[InlineArray(DataFileConstants.MaxTelemetryBuffers)]
public struct TelemetryBufferHeaderArray : IEquatable<TelemetryBufferHeaderArray>
{
    /// <summary>
    /// The length, in bytes, of an instance of <see cref="TelemetryBufferHeaderArray"/>.
    /// </summary>
    public const int Size = TelemetryBufferHeader.Size * DataFileConstants.MaxTelemetryBuffers;

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Field is required for InlineArray")]
    [SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Inline arrays do not allow readonly fields.")]
    private TelemetryBufferHeader _first;

    internal static TelemetryBufferHeaderArray FromArray(TelemetryBufferHeader[] headerArray)
    {
        if (headerArray.Length != 4)
        {
            throw new ArgumentException($"Value '{nameof(headerArray)}' must have a length of 4.", nameof(headerArray));
        }

        var headerBytes = MemoryMarshal.AsBytes<TelemetryBufferHeader>(headerArray);

        return MemoryMarshal.Read<TelemetryBufferHeaderArray>(headerBytes);
    }

    /// <inheritdoc />
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is TelemetryBufferHeaderArray other && Equals(other);
    }

    /// <inheritdoc />
    public readonly bool Equals(TelemetryBufferHeaderArray other)
    {
        for (int i = 0; i < DataFileConstants.MaxTelemetryBuffers; i++)
        {
            if (this[i] != other[i])
            {
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc />
    public override readonly int GetHashCode()
    {
        var hc = new HashCode();

        for (int i = 0; i < DataFileConstants.MaxTelemetryBuffers; i++)
        {
            hc.Add(this[i]);
        }

        return hc.ToHashCode();
    }

    /// <inheritdoc />
    public static bool operator ==(TelemetryBufferHeaderArray left, TelemetryBufferHeaderArray right)
    {
        return left.Equals(right);
    }

    /// <inheritdoc />
    public static bool operator !=(TelemetryBufferHeaderArray left, TelemetryBufferHeaderArray right)
    {
        return !(left == right);
    }
}
