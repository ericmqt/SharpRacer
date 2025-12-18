using System.Diagnostics.CodeAnalysis;
using SharpRacer.IO.Internal;

namespace SharpRacer.IO;

public readonly ref struct DataFileSpanOwner
{
    internal DataFileSpanOwner(
        IDataFileSpanPool? pool,
        DataFileSpanOwnerToken ownerToken,
        ReadOnlySpan<byte> span)
    {
        Pool = pool;
        Span = span;
        Token = ownerToken;
    }

    [MemberNotNullWhen(true, nameof(Pool))]
    internal readonly bool IsOwned => Pool != null;

    internal readonly IDataFileSpanPool? Pool { get; }

    public readonly ReadOnlySpan<byte> Span { get; }

    internal readonly DataFileSpanOwnerToken Token { get; }

    internal static DataFileSpanOwner Ownerless(ReadOnlySpan<byte> span)
    {
        return new DataFileSpanOwner(null, DataFileSpanOwnerToken.Empty, span);
    }

    /// <summary>
    /// Ends the ownership lifetime.
    /// </summary>
    public void Dispose()
    {
        Pool?.Return(in this);
    }

    public static implicit operator ReadOnlySpan<byte>(DataFileSpanOwner spanOwner)
    {
        return spanOwner.Span;
    }
}
