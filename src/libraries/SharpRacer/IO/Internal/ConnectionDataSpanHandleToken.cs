using System.Diagnostics.CodeAnalysis;

namespace SharpRacer.IO.Internal;
internal readonly struct ConnectionDataSpanHandleToken : IEquatable<ConnectionDataSpanHandleToken>
{
    private readonly ulong _id;

    public ConnectionDataSpanHandleToken(ulong id)
    {
        _id = id;
    }

    internal readonly ulong Id => _id;

    internal static ConnectionDataSpanHandleToken Empty { get; } = new ConnectionDataSpanHandleToken(0);

    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is ConnectionDataSpanHandleToken other && Equals(other);
    }

    public readonly bool Equals(ConnectionDataSpanHandleToken other)
    {
        return _id == other._id;
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(_id);
    }

    public static bool operator ==(ConnectionDataSpanHandleToken left, ConnectionDataSpanHandleToken right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ConnectionDataSpanHandleToken left, ConnectionDataSpanHandleToken right)
    {
        return !(left == right);
    }
}
