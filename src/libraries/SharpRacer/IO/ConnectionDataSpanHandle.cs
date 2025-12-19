using System.Diagnostics.CodeAnalysis;
using SharpRacer.IO.Internal;

namespace SharpRacer.IO;

/// <summary>
/// Provides a handle for a read-only span of bytes over the simulator connection data file.
/// </summary>
/// <remarks>
/// Because <see cref="ConnectionDataSpanHandle"/> implements the <see cref="IDisposable"/> interface, its
/// <see cref="IDisposable.Dispose">Dispose</see> method should be called when it is no longer needed.
/// 
/// Once disposed, it is no longer safe to access the <see cref="ConnectionDataSpanHandle.Span">Span</see> property.
/// </remarks>
public readonly ref struct ConnectionDataSpanHandle : IDisposable
{
    internal ConnectionDataSpanHandle(
        IConnectionDataSpanOwner? owner,
        ConnectionDataSpanHandleToken ownerToken,
        ReadOnlySpan<byte> span)
    {
        Owner = owner;
        Span = span;
        Token = ownerToken;
    }

    [MemberNotNullWhen(true, nameof(Owner))]
    internal readonly bool IsOwned => Owner != null;

    internal readonly IConnectionDataSpanOwner? Owner { get; }

    /// <summary>
    /// Gets the read-only span of bytes owned by the handle.
    /// </summary>
    public readonly ReadOnlySpan<byte> Span { get; }

    internal readonly ConnectionDataSpanHandleToken Token { get; }

    internal static ConnectionDataSpanHandle Ownerless(ReadOnlySpan<byte> span)
    {
        return new ConnectionDataSpanHandle(null, ConnectionDataSpanHandleToken.Zero, span);
    }

    /// <summary>
    /// Releases the handle.
    /// </summary>
    /// <remarks>
    /// This method should not be called while a reference to the span owned by the handle is available. Once disposed, it is no longer
    /// safe to access the <see cref="Span"/> property.
    /// </remarks>
    public void Dispose()
    {
        Owner?.Return(in this);
    }

    public static implicit operator ReadOnlySpan<byte>(ConnectionDataSpanHandle spanOwner)
    {
        return spanOwner.Span;
    }
}
