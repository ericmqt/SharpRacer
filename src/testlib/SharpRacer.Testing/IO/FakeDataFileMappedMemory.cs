using System.Runtime.InteropServices;
using DotNext.IO.MemoryMappedFiles;
using DotNext.Runtime.InteropServices;

namespace SharpRacer.IO;

internal sealed class FakeDataFileMappedMemory : IMappedMemory
{
    private readonly Memory<byte> _memory;

    public FakeDataFileMappedMemory(Memory<byte> memory)
    {
        _memory = memory;

        // Unused but initialization required
        Buffer = null!;
    }

    public SafeBuffer Buffer { get; }
    public int Length => _memory.Length;
    public Pointer<byte> Pointer { get; }
    public Span<byte> Span => _memory.Span;
    public Memory<byte> Memory => _memory;

    public void Dispose()
    {

    }

    public void Flush()
    {

    }
}
