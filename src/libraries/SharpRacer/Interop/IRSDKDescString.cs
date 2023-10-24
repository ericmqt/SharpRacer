using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace SharpRacer.Interop;

/// <summary>
/// A fixed 64-byte array that represents the iRacing SDK native character arrays of length IRSDK_MAX_DESC.
/// </summary>
[InlineArray(Size)]
public struct IRSDKDescString
{
    /// <summary>
    /// The length, in bytes, of an instance of <see cref="IRSDKDescString"/>.
    /// </summary>
    public const int Size = 64;

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Field is required for InlineArray")]
    [SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "InlineArray does not allow readonly fields")]
    private byte _first;

    internal static IRSDKDescString FromString(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            var emptyBytes = new byte[Size];

            return MemoryMarshal.Read<IRSDKDescString>(emptyBytes);
        }

        if (value.Length > Size)
        {
            throw new ArgumentException($"'{nameof(value)}' cannot be longer than {Size} characters.");
        }

        var strBytes = new byte[Size];
        Encoding.UTF8.GetBytes(value, strBytes);

        return MemoryMarshal.Read<IRSDKDescString>(strBytes);
    }

    /// <summary>
    /// Gets the length, in characters, of the string, excluding the null terminator.
    /// </summary>
    /// <returns>The length of the string in the underlying byte array, excluding the null terminator.</returns>
    public readonly int GetLength()
    {
        for (int i = 0; i < Size; i++)
        {
            if (this[i] == '\0')
            {
                return i;
            }
        }

        return Size;
    }

    /// <summary>
    /// Creates a <see cref="string"/> from the underlying fixed byte array, up to and excluding the null-terminator character if present.  
    /// </summary>
    /// <returns>A <see cref="string"/> representing the underlying fixed byte array up to the null terminator character if present.</returns>
    public override readonly string ToString()
    {
        return string.Create(GetLength(), this,
            (chars, source) =>
            {
                for (int i = 0; i < chars.Length; i++)
                {
                    chars[i] = (char)source[i];
                }
            });
    }
}
