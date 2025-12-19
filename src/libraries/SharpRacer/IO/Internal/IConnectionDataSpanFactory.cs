namespace SharpRacer.IO.Internal;
internal interface IConnectionDataSpanFactory : IDisposable
{
    ReadOnlySpan<byte> Create();
}
