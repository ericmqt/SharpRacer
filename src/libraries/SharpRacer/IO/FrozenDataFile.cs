namespace SharpRacer.IO;
internal class FrozenDataFile : ISimulatorDataFile
{
    private readonly Memory<byte> _data;

    public FrozenDataFile(byte[] data)
    {
        _data = data;
    }

    public FrozenDataFile(ISimulatorDataFile dataFile)
    {
        ArgumentNullException.ThrowIfNull(dataFile);

        var frozenBlob = new byte[dataFile.Span.Length];
        dataFile.Span.CopyTo(frozenBlob);

        _data = frozenBlob;
    }

    public ReadOnlySpan<byte> Span => _data.Span;

    public ISimulatorDataFile Freeze()
    {
        return new FrozenDataFile(this);
    }

    public void Dispose()
    {

    }
}
