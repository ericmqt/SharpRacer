using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace SharpRacer;
public readonly struct ContentVersion
    : IComparable<ContentVersion>,
    IComparisonOperators<ContentVersion, ContentVersion, bool>,
    IEquatable<ContentVersion>,
    IEqualityOperators<ContentVersion, ContentVersion, bool>,
    IParsable<ContentVersion>
{
    public ContentVersion(int major, int minor, int build, int patch)
    {
        Major = major;
        Minor = minor;
        Build = build;
        Patch = patch;
    }

    public readonly int Major { get; }
    public readonly int Minor { get; }
    public readonly int Build { get; }
    public readonly int Patch { get; }

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

    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is ContentVersion other && Equals(other);
    }

    public readonly bool Equals(ContentVersion other)
    {
        return Major == other.Major && Minor == other.Minor && Build == other.Build && Patch == other.Patch;
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Major, Minor, Build, Patch);
    }

    public override readonly string ToString()
    {
        return string.Format("{0}.{1:D2}.{2:D2}.{3:D2}", Major, Minor, Build, Patch);
    }

    /// <summary>
    /// Parses a string into a <see cref="ContentVersion"/> value.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="provider"></param>
    /// <returns></returns>
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

    public static bool operator ==(ContentVersion left, ContentVersion right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ContentVersion left, ContentVersion right)
    {
        return !left.Equals(right);
    }

    /// <summary>
    /// Evaluates whether the left operand is less than the right operand.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
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
    /// <param name="left"></param>
    /// <param name="right"></param>
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
    /// <param name="left"></param>
    /// <param name="right"></param>
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
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns>
    /// <see langword="true"/> if the left operand is greater than or equal to the right operand, otherwise <see langword="false"/>.
    /// </returns>
    public static bool operator >=(ContentVersion left, ContentVersion right)
    {
        return left.CompareTo(right) >= 0;
    }
}
