namespace SharpRacer.Telemetry;

/// <summary>
/// Provides a single frame of telemetry data sourced from a telemetry file (*.IBT).
/// </summary>
public class TelemetryFileDataFrame : IDataFrame
{
    private readonly byte[] _data;

    /// <summary>
    /// Initializes a new <see cref="TelemetryFileDataFrame"/>.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="frameIndex"></param>
    internal TelemetryFileDataFrame(byte[] data, int frameIndex)
    {
        _data = data;
        FrameIndex = frameIndex;
    }

    /// <inheritdoc />
    public ReadOnlySpan<byte> Data => _data;

    /// <summary>
    /// Gets the index of the frame in the telemetry file.
    /// </summary>
    public int FrameIndex { get; }
}
