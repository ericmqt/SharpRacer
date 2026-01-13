using System.Runtime.CompilerServices;

namespace SharpRacer.Commands;

/// <summary>
/// Provides methods for reading argument values from a <see cref="CommandMessage"/> value.
/// </summary>
public readonly ref struct CommandMessageReader
{
    private readonly ref readonly CommandMessage _message;

    /// <summary>
    /// Initializes a new <see cref="CommandMessageReader"/> instance from the specified <see cref="CommandMessage"/>.
    /// </summary>
    /// <param name="message">The <see cref="CommandMessage"/> to read.</param>
    public CommandMessageReader(ref readonly CommandMessage message)
    {
        _message = ref message;
    }

    /// <summary>
    /// Reads a <see cref="ushort"/> from the first argument of the <see cref="CommandMessage"/>.
    /// </summary>
    /// <returns>The first argument of the <see cref="CommandMessage"/> as a 16-bit unsigned integer.</returns>
    public readonly ushort ReadArg1()
    {
        return _message.Arg1;
    }

    /// <summary>
    /// Reads a <typeparamref name="TEnum"/> value from the first argument of the <see cref="CommandMessage"/>.
    /// </summary>
    /// <typeparam name="TEnum">The type of enumeration to read.</typeparam>
    /// <returns>The first argument of the <see cref="CommandMessage"/> as a <typeparamref name="TEnum"/> value.</returns>
    /// <exception cref="CommandMessageReaderException">
    /// The value read from the <see cref="CommandMessage"/> is not defined in <typeparamref name="TEnum"/>.
    /// </exception>
    public readonly TEnum ReadArg1<TEnum>()
        where TEnum : unmanaged, Enum
    {
        var arg1 = _message.Arg1;
        var value = Unsafe.As<ushort, TEnum>(ref arg1);

        if (!Enum.IsDefined(typeof(TEnum), value))
        {
            throw CommandMessageReaderException.ReadUndefinedEnumerationValue<TEnum>(arg1);
        }

        return value;
    }

    /// <summary>
    /// Reads a <typeparamref name="TEnum"/> value from the first argument of the <see cref="CommandMessage"/> without checking if the
    /// value is defined by the enumeration.
    /// </summary>
    /// <typeparam name="TEnum">The type of enumeration to read.</typeparam>
    /// <returns>The first argument of the <see cref="CommandMessage"/> as a <typeparamref name="TEnum"/> value.</returns>
    public readonly TEnum ReadArg1Flags<TEnum>()
        where TEnum : unmanaged, Enum
    {
        var arg1 = _message.Arg1;
        return Unsafe.As<ushort, TEnum>(ref arg1);
    }

    /// <summary>
    /// Reads a <see cref="ushort"/> from the second argument of the <see cref="CommandMessage"/>.
    /// </summary>
    /// <returns>The second argument of the <see cref="CommandMessage"/> as a <see cref="ushort"/>.</returns>
    public readonly ushort ReadArg2()
    {
        return _message.Arg2;
    }

    /// <summary>
    /// Reads a <see cref="bool"/> from the second argument of the <see cref="CommandMessage"/>.
    /// </summary>
    /// <returns>The second argument of the <see cref="CommandMessage"/> as a <see cref="bool"/>.</returns>
    public readonly bool ReadArg2Bool()
    {
        return _message.Arg2 != 0;
    }

    /// <summary>
    /// Reads a <see cref="float"/> from the second argument of the <see cref="CommandMessage"/>.
    /// </summary>
    /// <returns>The second argument of the <see cref="CommandMessage"/> as a <see cref="float"/>.</returns>
    public readonly float ReadArg2Float()
    {
        var real = (_message.Arg2 << 16) | _message.Arg3;

        return real / CommandMessageConstants.FloatArgument.ScaleFactor;
    }

    /// <summary>
    /// Reads an <see cref="int"/> from the second argument of the <see cref="CommandMessage"/>.
    /// </summary>
    /// <returns>The second argument of the <see cref="CommandMessage"/> as an <see cref="int"/>.</returns>
    public readonly int ReadArg2Int()
    {
        return (_message.Arg2 << 16) | _message.Arg3;
    }

    /// <summary>
    /// Reads a <see cref="ushort"/> from the third argument of the <see cref="CommandMessage"/>.
    /// </summary>
    /// <returns>The third argument of the <see cref="CommandMessage"/> as a <see cref="ushort"/>.</returns>
    public readonly ushort ReadArg3()
    {
        return _message.Arg3;
    }

    /// <summary>
    /// Attempts to read a <typeparamref name="TEnum"/> value from the first argument of the <see cref="CommandMessage"/> and returns a
    /// value that indicates whether the operation was successful.
    /// </summary>
    /// <typeparam name="TEnum">The type of enumeration to read.</typeparam>
    /// <param name="value">
    /// The <typeparamref name="TEnum"/> value read from the first argument of the <see cref="CommandMessage"/>.
    /// </param>
    /// <returns><see langword="true"/> if the operation was successful, otherwise <see langword="false"/>.</returns>
    public readonly bool TryReadArg1<TEnum>(out TEnum value)
        where TEnum : unmanaged, Enum
    {
        var arg1 = _message.Arg1;

        value = Unsafe.As<ushort, TEnum>(ref arg1);

        if (!Enum.IsDefined(typeof(TEnum), value))
        {
            value = default;
            return false;
        }

        return true;
    }

    /// <summary>
    /// Attempts to read a <typeparamref name="TEnum"/> value from the first argument of the <see cref="CommandMessage"/> without checking
    /// that the value is defined by the enumeration and returns a value that indicates whether the operation was successful.
    /// </summary>
    /// <typeparam name="TEnum">The type of enumeration to read.</typeparam>
    /// <param name="value">
    /// The <typeparamref name="TEnum"/> value read from the first argument of the <see cref="CommandMessage"/>.
    /// </param>
    /// <returns><see langword="true"/> if the operation was successful, otherwise <see langword="false"/>.</returns>
    public readonly bool TryReadArg1Flags<TEnum>(out TEnum value)
        where TEnum : unmanaged, Enum
    {
        var arg1 = _message.Arg1;

        value = Unsafe.As<ushort, TEnum>(ref arg1);
        return true;
    }
}
