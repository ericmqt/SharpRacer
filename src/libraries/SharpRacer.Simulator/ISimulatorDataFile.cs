namespace SharpRacer.Simulator;
public interface ISimulatorDataFile : IDisposable
{
    ReadOnlySpan<byte> Span { get; }
}
