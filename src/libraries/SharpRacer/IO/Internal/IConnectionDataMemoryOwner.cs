namespace SharpRacer.IO.Internal;

internal interface IConnectionDataMemoryOwner : IDisposable
{
    ReadOnlyMemory<byte> Memory { get; }

    void Close();
    IConnectionDataHandle Rent();
    void Return(IConnectionDataHandle memoryOwner);
}
