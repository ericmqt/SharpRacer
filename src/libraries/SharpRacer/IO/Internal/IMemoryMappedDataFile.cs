using DotNext.IO.MemoryMappedFiles;

namespace SharpRacer.IO.Internal;

internal interface IMemoryMappedDataFile : IDisposable
{
    ReadOnlyMemory<byte> Data { get; }

    IMappedMemory CreateMemoryAccessor();
    IMemoryMappedFileSpanFactory CreateSpanAccessor();
}
