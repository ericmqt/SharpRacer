namespace SharpRacer.Simulator;

/// <summary>
/// Provides access to the simulator data file for a simulator session.
/// </summary>
public interface ISimulatorDataFile : IDisposable
{
    /// <summary>
    /// Gets a read-only span of bytes over the contents of the simulator data file.
    /// </summary>
    ReadOnlySpan<byte> Span { get; }
}
