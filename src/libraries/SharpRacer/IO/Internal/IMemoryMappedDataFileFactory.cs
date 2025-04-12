namespace SharpRacer.IO.Internal;
internal interface IMemoryMappedDataFileFactory
{
    IMemoryMappedDataFile CreateEmpty();
    IMemoryMappedDataFile CreateFrozen(IMemoryMappedDataFile openDataFile);
    IMemoryMappedDataFile OpenNew();
}
