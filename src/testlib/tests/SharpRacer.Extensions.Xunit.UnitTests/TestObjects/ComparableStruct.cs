using System.Diagnostics.CodeAnalysis;

namespace SharpRacer.Extensions.Xunit.TestObjects;

public readonly struct ComparableStruct : IComparable<ComparableStruct>, IEquatable<ComparableStruct>
{
    public ComparableStruct(int x)
    {
        X = x;
    }

    public readonly int X { get; }

    public int CompareTo(ComparableStruct other)
    {
        return X.CompareTo(other.X);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is ComparableStruct other && Equals(other);
    }

    public bool Equals(ComparableStruct other)
    {
        return X == other.X;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X);
    }

    public static bool operator ==(ComparableStruct left, ComparableStruct right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ComparableStruct left, ComparableStruct right)
    {
        return !(left == right);
    }

    public static bool operator <(ComparableStruct left, ComparableStruct right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(ComparableStruct left, ComparableStruct right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(ComparableStruct left, ComparableStruct right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(ComparableStruct left, ComparableStruct right)
    {
        return left.CompareTo(right) >= 0;
    }
}
