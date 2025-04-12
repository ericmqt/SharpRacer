namespace SharpRacer.IO.Internal;

internal interface IDataFileMemoryPool : IDisposable
{
    void Close();
    IDataFileMemoryOwner Rent();
    void Return(IDataFileMemoryOwner memoryOwner);
}
