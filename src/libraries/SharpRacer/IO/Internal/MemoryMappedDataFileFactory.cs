using System.IO.MemoryMappedFiles;
using System.Runtime.Versioning;

namespace SharpRacer.IO.Internal;
internal sealed class MemoryMappedDataFileFactory : IMemoryMappedDataFileFactory
{
    private const string _MemoryMappedFileName = "Local\\IRSDKMemMapFileName";

    public IMemoryMappedDataFile CreateEmpty()
    {
        throw new NotImplementedException();
    }

    public IMemoryMappedDataFile CreateFrozen(IMemoryMappedDataFile openDataFile)
    {
        throw new NotImplementedException();
    }

    [SupportedOSPlatform("windows")]
    public IMemoryMappedDataFile OpenNew()
    {
        return new MemoryMappedDataFile(MemoryMappedFile.OpenExisting(_MemoryMappedFileName, MemoryMappedFileRights.Read));
    }
}
