namespace SharpRacer.IO.Internal;

internal sealed class FakeConnectionDataSpanFactory : IConnectionDataSpanFactory
{
    private readonly Memory<byte> _memory;

    public FakeConnectionDataSpanFactory(Memory<byte> memory)
    {
        _memory = memory;
    }

    public bool IsDisposed { get; private set; }

    public ReadOnlySpan<byte> Create()
    {
        return _memory.Span;
    }

    public void Dispose()
    {
        IsDisposed = true;
    }
}
