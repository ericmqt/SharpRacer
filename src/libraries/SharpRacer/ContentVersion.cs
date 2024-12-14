using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace SharpRacer;

/// <summary>
/// Represents the version number for simulator content (e.g. tracks, cars).
/// </summary>
public readonly struct ContentVersion
    : IComparable<ContentVersion>,
    IComparisonOperators<ContentVersion, ContentVersion, bool>,
    IEquatable<ContentVersion>,
    IEqualityOperators<ContentVersion, ContentVersion, bool>,
    IParsable<ContentVersion>
{
    /// <summary>
    /// Initializes a new <see cref="ContentVersion"/> value from the specified version number parts.
    /// </summary>
    /// <param name="major">The major version number.</param>
    /// <param name="minor">The minor version number.</param>
    /// <param name="build">The build number.</param>
    /// <param name="patch">The patch number.</param>
    public ContentVersion(int major, int minor, int build, int patch)
    {
        Major = major;
        Minor = minor;
        Build = build;
        Patch = patch;
    }

    /// <summary>
    /// Gets the major version number.
    /// </summary>
    public readonly int Major { get; }

    /// <summary>
    /// Gets the minor version number.
    /// </summary>
    public readonly int Minor { get; }

    /// <summary>
    /// Gets the build number.
    /// </summary>
    public readonly int Build { get; }

    /// <summary>
    /// Gets the patch number.
    /// </summary>
    public readonly int Patch { get; }

    /// <summary>
    /// Compares the current instance to another <see cref="ContentVersion"/> value and returns an integer that indicates whether the
    /// current value precedes, follows, or occurs in the same position in the sort order as the other value.
    /// </summary>
    /// <param name="other">The <see cref="ContentVersion"/> value to compare against.</param>
    /// <returns>
    /// An integer that indicates the relative order of the values being compared, having the following significance:
    /// <para>Negative: This instance precedes other in the sort order.</para>
    /// <para>Zero: This instance occurs in the same position in the sort order as other.</para>
    /// <para>Positive: This instance follows other in the sort order.</para>
    /// </returns>
    public readonly int CompareTo(ContentVersion other)
    {
        if (Major != other.Major)
        {
            return Major.CompareTo(other.Major);
        }

        if (Minor != other.Minor)
        {
            return Minor.CompareTo(other.Minor);
        }

        if (Build != other.Build)
        {
            return Build.CompareTo(other.Build);
        }

        return Patch.CompareTo(other.Patch);
    }

    /// <summary>
    /// Evaluates whether the current instance is equivalent to the specified object.
    /// </summary>
    /// <param name="obj">The object to compare against the current instance for equality.</param>
    /// <returns>
    /// <see langword="true"/> if the current instance is equivalent to the specified object, otherwise <see langword="false"/>.
    /// </returns>
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is ContentVersion other && Equals(other);
    }

    /// <summary>
    /// Evaluates whether the current instance is equivalent to the specified <see cref="ContentVersion"/> value.
    /// </summary>
    /// <param name="other">The <see cref="ContentVersion"/> value to compare against the current instance for equality.</param>
    /// <returns>
    /// <see langword="true"/> if the current instance is equivalent to the specified <see cref="ContentVersion"/> value, otherwise
    /// <see langword="false"/>.
    /// </returns>
    public readonly bool Equals(ContentVersion other)
    {
        return Major == other.Major && Minor == other.Minor && Build == other.Build && Patch == other.Patch;
    }

    /// <summary>
    /// Computes the hash code for the current instance.
    /// </summary>
    /// <returns></returns>
    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Major, Minor, Build, Patch);
    }

    /// <summary>
    /// Returns a string that represents the current <see cref="ContentVersion"/> value.
    /// </summary>
    /// <returns></returns>
    public override readonly string ToString()
    {
        return string.Format("{0}.{1:D2}.{2:D2}.{3:D2}", Major, Minor, Build, Patch);
    }

    /// <summary>
    /// Parses a string into a <see cref="ContentVersion"/> value.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="provider"></param>
    /// <returns>A <see cref="ContentVersion"/> value parsed from the string.</returns>
    /// <exception cref="FormatException">The input string is not in a valid format.</exception>
    public static ContentVersion Parse(string s, IFormatProvider? provider = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(s, nameof(s));

        var parts = s.Split('.');

        if (parts.Length != 4)
        {
            throw new FormatException($"Value '{s}' is not in a valid format.");
        }

        if (!int.TryParse(parts[0], out var major))
        {
            throw new FormatException($"Unable to parse '{parts[0]}' as major version number.");
        }

        if (!int.TryParse(parts[1], out var minor))
        {
            throw new FormatException($"Unable to parse '{parts[0]}' as minor version number.");
        }

        if (!int.TryParse(parts[2], out var build))
        {
            throw new FormatException($"Unable to parse '{parts[0]}' as build version number.");
        }

        if (!int.TryParse(parts[3], out var patch))
        {
            throw new FormatException($"Unable to parse '{parts[0]}' as patch version number.");
        }

        return new ContentVersion(major, minor, build, patch);
    }

    /// <summary>
    /// Attempts to parse the specified string into a <see cref="ContentVersion"/> value.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="provider"></param>
    /// <param name="result">
    /// The <see cref="ContentVersion"/> value parsed from the input string if parsing succeeded.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if parsing succeeded, otherwise <see langword="false"/>. When the return value is <see langword="true"/>,
    /// the <paramref name="result"/> parameter contains the parsed value.
    /// </returns>
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out ContentVersion result)
    {
        result = default;

        if (string.IsNullOrEmpty(s))
        {
            return false;
        }

        var parts = s.Split('.');

        if (parts.Length != 4)
        {
            return false;
        }

        if (!int.TryParse(parts[0], out var major))
        {
            return false;
        }

        if (!int.TryParse(parts[1], out var minor))
        {
            return false;
        }

        if (!int.TryParse(parts[2], out var build))
        {
            return false;
        }

        if (!int.TryParse(parts[3], out var patch))
        {
            return false;
        }

        result = new ContentVersion(major, minor, build, patch);
        return true;
    }

    /// <summary>
    /// Compares two <see cref="ContentVersion"/> values to determine equality.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>
    /// <see langword="true"/> if the operands are equivalent, otherwise <see langword="false"/>.
    /// </returns>
    public static bool operator ==(ContentVersion left, ContentVersion right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Compares two <see cref="ContentVersion"/> values to determine inequality.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>
    /// <see langword="true"/> if the operands are not equivalent, otherwise <see langword="false"/>.
    /// </returns>
    public static bool operator !=(ContentVersion left, ContentVersion right)
    {
        return !left.Equals(right);
    }

    /// <summary>
    /// Evaluates whether the left operand is less than the right operand.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>
    /// <see langword="true"/> if the left operand is less than the right operand, otherwise <see langword="false"/>.
    /// </returns>
    public static bool operator <(ContentVersion left, ContentVersion right)
    {
        return left.CompareTo(right) < 0;
    }

    /// <summary>
    /// Evaluates whether the left operand is less than or equal to the right operand.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>
    /// <see langword="true"/> if the left operand is less than or equal to the right operand, otherwise <see langword="false"/>.
    /// </returns>
    public static bool operator <=(ContentVersion left, ContentVersion right)
    {
        return left.CompareTo(right) <= 0;
    }

    /// <summary>
    /// Evaluates whether the left operand is greater than the right operand.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>
    /// <see langword="true"/> if the left operand is greater than the right operand, otherwise <see langword="false"/>.
    /// </returns>
    public static bool operator >(ContentVersion left, ContentVersion right)
    {
        return left.CompareTo(right) > 0;
    }

    /// <summary>
    /// Evaluates whether the left operand is greater than or equal to the right operand.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>
    /// <see langword="true"/> if the left operand is greater than or equal to the right operand, otherwise <see langword="false"/>.
    /// </returns>
    public static bool operator >=(ContentVersion left, ContentVersion right)
    {
        return left.CompareTo(right) >= 0;
    }
}
