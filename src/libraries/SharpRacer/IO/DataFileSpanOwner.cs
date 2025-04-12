using SharpRacer.IO.Internal;

namespace SharpRacer.IO;

public readonly ref struct DataFileSpanOwner
{
    private readonly IDataFileSpanPool _pool;

    internal DataFileSpanOwner(
        IDataFileSpanPool pool,
        DataFileSpanOwnerToken ownerToken,
        ReadOnlySpan<byte> span)
    {
        _pool = pool;
        Span = span;
        Token = ownerToken;
    }

    public readonly ReadOnlySpan<byte> Span { get; }
    internal readonly DataFileSpanOwnerToken Token { get; }

    public void Dispose()
    {
        _pool.Return(in this);
    }
}
