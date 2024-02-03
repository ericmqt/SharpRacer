namespace SharpRacer.Internal;

internal class EmptySimulatorDataFile : ISimulatorDataFile
{
    private readonly byte[] _data;

    public EmptySimulatorDataFile()
    {
        _data = [];
    }

    public EmptySimulatorDataFile(int size)
    {
        _data = new byte[size];
    }

    public ReadOnlySpan<byte> Span => _data;

    public void Dispose()
    {

    }
}
