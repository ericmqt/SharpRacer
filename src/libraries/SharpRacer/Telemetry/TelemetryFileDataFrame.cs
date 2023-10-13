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
    /// <param name="tick"></param>
    internal TelemetryFileDataFrame(byte[] data, int tick)
    {
        _data = data;

        Tick = tick;
    }

    /// <inheritdoc />
    public ReadOnlySpan<byte> Data => _data;

    /// <inheritdoc />
    public int Tick { get; }
}
