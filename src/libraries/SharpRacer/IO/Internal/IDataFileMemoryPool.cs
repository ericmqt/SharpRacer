namespace SharpRacer.IO.Internal;

internal interface IDataFileMemoryPool : IDisposable
{
    void Close();
    IConnectionDataHandle Rent();
    void Return(IConnectionDataHandle memoryOwner);
}
