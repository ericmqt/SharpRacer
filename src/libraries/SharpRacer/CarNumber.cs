using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SharpRacer;

/// <summary>
/// Represents a car number.
/// </summary>
/// <remarks>
/// The car number is encoded as an integer that accounts for leading zeroes, allowing car numbers such as "7" and "07" to be easily
/// distinguished.
/// 
/// See irsdk_padCarNum in the iRacing SDK.
/// </remarks>
public readonly struct CarNumber : IEquatable<CarNumber>, IParsable<CarNumber>, ISpanParsable<CarNumber>
{
    internal const int LengthFactor = 1000;

    /// <summary>
    /// Initializes a new <see cref="CarNumber"/> value from its encoded integer representation.
    /// </summary>
    /// <param name="rawValue">The encoded integer representation of the car number.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="rawValue"/> is less than zero or greater than 3999.
    /// </exception>
    public CarNumber(int rawValue)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(rawValue);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(rawValue, 3999);

        Base = rawValue - ((rawValue / LengthFactor) * LengthFactor);
        var nBaseDigits = GetNonZeroDigitCount(Base);
        var nLeadingZeroes = Math.Max((rawValue / LengthFactor) - nBaseDigits, 0);

        Length = nLeadingZeroes + nBaseDigits;
        Value = nLeadingZeroes > 0 ? Base + (Length * LengthFactor) : Base;

        HasValue = true;
    }

    /// <summary>
    /// Gets a <see cref="CarNumber"/> value that indicates a value was not specified.
    /// </summary>
    public static CarNumber None { get; } = default;

    /// <summary>
    /// Gets the integer representation of the car number.
    /// </summary>
    internal readonly int Base { get; }

    /// <summary>
    /// Gets a value indicating if this instance was initialized.
    /// </summary>
    internal readonly bool HasValue { get; }

    /// <summary>
    /// Gets the length of the car number when formatted as a string, including any leading zeroes.
    /// </summary>
    internal readonly int Length { get; }

    /// <summary>
    /// Gets an encoded integer representation of the car number, accounting for any leading zeroes.
    /// </summary>
    public readonly int Value { get; }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is CarNumber number && Equals(number);
    }

    /// <inheritdoc />
    public bool Equals(CarNumber other)
    {
        return Value == other.Value && HasValue == other.HasValue;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(HasValue, Value);
    }

    /// <summary>
    /// Returns the string representation of the car number, including any leading zeroes.
    /// </summary>
    /// <returns>The car number as a string.</returns>
    public override string ToString()
    {
        if (!HasValue)
        {
            return string.Empty;
        }

        return Base.ToString($"D{Length}");
    }

    /// <summary>
    /// Parses a string into a <see cref="CarNumber"/> value.
    /// </summary>
    /// <param name="value">The string to parse.</param>
    /// <param name="provider">An object that provides culture-specific formatting information about <paramref name="value"/>.</param>
    /// <returns>The result of parsing <paramref name="value"/>.</returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="value"/> is an empty string or <see langword="null"/>.
    /// 
    /// -OR-
    /// 
    /// The length of <paramref name="value"/> is greater than three.
    /// </exception>
    /// <exception cref="FormatException">
    /// <paramref name="value"/> is not in the correct format.
    /// </exception>
    public static CarNumber Parse(string value, IFormatProvider? provider = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(value);

        if (value.Length > 3)
        {
            throw new ArgumentException($"'{nameof(value)}' must have a length less than or equal to 3.", nameof(value));
        }

        return Parse(value.AsSpan(), provider);
    }

    /// <summary>
    /// Parses a span of characters into a <see cref="CarNumber"/> value.
    /// </summary>
    /// <param name="valueSpan">The span of characters to parse.</param>
    /// <param name="provider">An object that provides culture-specific formatting information about <paramref name="valueSpan"/>.</param>
    /// <returns>The result of parsing <paramref name="valueSpan"/>.</returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="valueSpan"/> is empty.
    /// 
    /// -OR-
    /// 
    /// The length of <paramref name="valueSpan"/> is greater than three.
    /// </exception>
    /// <exception cref="FormatException">
    /// The string represented by <paramref name="valueSpan"/> is not in the correct format.
    /// </exception>
    public static CarNumber Parse(ReadOnlySpan<char> valueSpan, IFormatProvider? provider = null)
    {
        if (valueSpan.Length == 0)
        {
            throw new ArgumentException($"'{nameof(valueSpan)}' cannot be empty.", nameof(valueSpan));
        }

        if (valueSpan.Length > 3)
        {
            throw new ArgumentException($"'{nameof(valueSpan)}' must have a length less than or equal to 3.", nameof(valueSpan));
        }

        int leadingZeroCount = GetLeadingZeroCount(valueSpan);

        if (leadingZeroCount == valueSpan.Length)
        {
            return new CarNumber(leadingZeroCount * LengthFactor);
        }

        var carNumberBaseSpan = valueSpan[leadingZeroCount..];

        if (!int.TryParse(carNumberBaseSpan, out var carNumberBase))
        {
            throw new FormatException($"Unable to parse non-leading-zero part '{carNumberBaseSpan}' as an integer.");
        }

        return new CarNumber(carNumberBase + (valueSpan.Length * LengthFactor));
    }

    /// <summary>
    /// Attempts to parse a string into a <see cref="CarNumber"/> value.
    /// </summary>
    /// <param name="value">The string to parse.</param>
    /// <param name="result">
    /// When this method returns, contains the result of successfully parsing <paramref name="value"/> or the default value on failure.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="value"/> was successfully parsed, otherwise <see langword="false"/>.
    /// </returns>
    public static bool TryParse([NotNullWhen(true)] string? value, [MaybeNullWhen(false)] out CarNumber result)
    {
        return TryParse(value.AsSpan(), null, out result);
    }

    /// <summary>
    /// Attempts to parse a string into a <see cref="CarNumber"/> value.
    /// </summary>
    /// <param name="value">The string to parse.</param>
    /// <param name="provider">An object that provides culture-specific formatting information about <paramref name="value"/>.</param>
    /// <param name="result">
    /// When this method returns, contains the result of successfully parsing <paramref name="value"/> or the default value on failure.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="value"/> was successfully parsed, otherwise <see langword="false"/>.
    /// </returns>
    public static bool TryParse([NotNullWhen(true)] string? value, IFormatProvider? provider, [MaybeNullWhen(false)] out CarNumber result)
    {
        return TryParse(value.AsSpan(), provider, out result);
    }

    /// <summary>
    /// Attempts to parse a span of characters into a <see cref="CarNumber"/> value.
    /// </summary>
    /// <param name="valueSpan">The span of characters to parse.</param>
    /// <param name="result">
    /// When this method returns, contains the result of successfully parsing <paramref name="valueSpan"/> or the default value on failure.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="valueSpan"/> was successfully parsed, otherwise <see langword="false"/>.
    /// </returns>
    public static bool TryParse(ReadOnlySpan<char> valueSpan, [MaybeNullWhen(false)] out CarNumber result)
    {
        return TryParse(valueSpan, null, out result);
    }

    /// <summary>
    /// Attempts to parse a span of characters into a <see cref="CarNumber"/> value.
    /// </summary>
    /// <param name="valueSpan">The span of characters to parse.</param>
    /// <param name="provider">An object that provides culture-specific formatting information about <paramref name="valueSpan"/>.</param>
    /// <param name="result">
    /// When this method returns, contains the result of successfully parsing <paramref name="valueSpan"/> or the default value on failure.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="valueSpan"/> was successfully parsed, otherwise <see langword="false"/>.
    /// </returns>
    public static bool TryParse(ReadOnlySpan<char> valueSpan, IFormatProvider? provider, [MaybeNullWhen(false)] out CarNumber result)
    {
        result = default;

        if (valueSpan.Length == 0 || valueSpan.Length > 3)
        {
            return false;
        }

        int leadingZeroCount = GetLeadingZeroCount(valueSpan);

        if (leadingZeroCount == valueSpan.Length)
        {
            result = new CarNumber(leadingZeroCount * LengthFactor);
            return true;
        }

        var carNumberBaseSpan = valueSpan[leadingZeroCount..];

        if (!int.TryParse(carNumberBaseSpan, out var carNumberBase))
        {
            return false;
        }

        result = new CarNumber(carNumberBase + (valueSpan.Length * LengthFactor));
        return true;
    }

    internal static int GetLeadingZeroCount(ReadOnlySpan<char> span)
    {
        int leadingZeroCount = 0;

        for (int i = 0; i < span.Length; i++)
        {
            if (span[i] == '0')
            {
                leadingZeroCount++;
            }
            else
            {
                break;
            }
        }

        return leadingZeroCount;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int GetNonZeroDigitCount(int baseCarNumber)
    {
        return baseCarNumber switch
        {
            > 99 => 3,
            > 9 => 2,
            _ => 1
        };
    }

    /// <inheritdoc />
    public static bool operator ==(CarNumber left, CarNumber right)
    {
        return left.Equals(right);
    }

    /// <inheritdoc />
    public static bool operator !=(CarNumber left, CarNumber right)
    {
        return !(left == right);
    }
}
