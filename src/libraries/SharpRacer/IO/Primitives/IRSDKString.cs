﻿using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SharpRacer.IO.Primitives;

/// <summary>
/// A fixed 32-byte array that represents the iRacing SDK native character arrays of length IRSDK_MAX_STRING.
/// </summary>
[InlineArray(Size)]
public struct IRSDKString
{
    /// <summary>
    /// The length, in bytes, of an instance of <see cref="IRSDKString"/>.
    /// </summary>
    public const int Size = 32;

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Field is required for inline arrays.")]
    [SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Inline arrays do not allow readonly fields.")]
    private byte _first;

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
