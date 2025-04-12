namespace SharpRacer.IO.Internal;
internal interface IMemoryMappedFileSpanFactory : IDisposable
{
    ReadOnlySpan<byte> CreateReadOnlySpan();
}
