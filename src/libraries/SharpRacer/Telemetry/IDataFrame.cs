namespace SharpRacer.Telemetry;

/// <summary>
/// Provides a single frame of telemetry data.
/// </summary>
public interface IDataFrame
{
    /// <summary>
    /// Gets a read-only span of bytes over the frame data.
    /// </summary>
    ReadOnlySpan<byte> Data { get; }

    /// <summary>
    /// The simulator tick associated with the data frame.
    /// </summary>
    int Tick { get; }
}
