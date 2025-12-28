using DotNext.IO.MemoryMappedFiles;
using SharpRacer.IO.Internal;

namespace SharpRacer.IO;

internal sealed class FakeMemoryMappedDataFile : IMemoryMappedDataFile
{
    private readonly Memory<byte> _memory;

    public FakeMemoryMappedDataFile(Memory<byte> memory)
    {
        _memory = memory;
    }

    public ReadOnlyMemory<byte> Data => _memory;

    public IMappedMemory CreateMemoryAccessor()
    {
        return new FakeDataFileMappedMemory(_memory);
    }

    public IConnectionDataSpanFactory CreateSpanFactory()
    {
        return new FakeConnectionDataSpanFactory(_memory);
    }

    public void Dispose()
    {

    }
}
