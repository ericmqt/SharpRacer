namespace SharpRacer.IO;
public interface IDataFileMemoryOwner : IDisposable
{
    ReadOnlyMemory<byte> Memory { get; }
}
