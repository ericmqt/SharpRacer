namespace SharpRacer.Extensions.Xunit.TestObjects;

public readonly struct EqualityStruct : IEquatable<EqualityStruct>
{
    public EqualityStruct(int x)
    {
        X = x;
    }

    public readonly int X { get; }

    public override bool Equals(object? obj)
    {
        return obj is EqualityStruct harness && Equals(harness);
    }

    public bool Equals(EqualityStruct other)
    {
        return X == other.X;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X);
    }

    public static bool operator ==(EqualityStruct left, EqualityStruct right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(EqualityStruct left, EqualityStruct right)
    {
        return !(left == right);
    }
}
