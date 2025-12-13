using System.Diagnostics.CodeAnalysis;

namespace SharpRacer.IO.Internal;
internal readonly struct DataFileSpanOwnerToken : IEquatable<DataFileSpanOwnerToken>
{
    private readonly ulong _id;

    public DataFileSpanOwnerToken(ulong id)
    {
        _id = id;
    }

    internal readonly ulong Id => _id;

    internal static DataFileSpanOwnerToken Empty { get; } = new DataFileSpanOwnerToken(0);

    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is DataFileSpanOwnerToken other && Equals(other);
    }

    public readonly bool Equals(DataFileSpanOwnerToken other)
    {
        return _id == other._id;
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(_id);
    }

    public static bool operator ==(DataFileSpanOwnerToken left, DataFileSpanOwnerToken right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(DataFileSpanOwnerToken left, DataFileSpanOwnerToken right)
    {
        return !(left == right);
    }
}
