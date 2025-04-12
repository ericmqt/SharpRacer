namespace SharpRacer.IO.Internal;
internal sealed class ConnectionDataFileFactory : IConnectionDataFileFactory
{
    private readonly IMemoryMappedDataFileFactory _memoryMappedDataFileFactory;

    public ConnectionDataFileFactory()
        : this(new MemoryMappedDataFileFactory())
    {

    }

    public ConnectionDataFileFactory(IMemoryMappedDataFileFactory memoryMappedDataFileFactory)
    {
        _memoryMappedDataFileFactory = memoryMappedDataFileFactory;
    }

    public IConnectionDataFile Create()
    {
        var mmf = _memoryMappedDataFileFactory.OpenNew();

        return new ConnectionDataFile(mmf);
    }
}
