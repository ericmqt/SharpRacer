namespace SharpRacer;

/// <summary>
/// Describes values representing any active engine warnings.
/// </summary>
/// <remarks>See: irsdk_EngineWarnings</remarks>
[Flags]
public enum EngineWarnings : uint
{
    /// <summary>
    /// No active engine warnings.
    /// </summary>
    None = 0,

    /// <summary>
    /// Water temperature warning.
    /// </summary>
    WaterTemperature = 0x01,

    /// <summary>
    /// Fuel pressure warning.
    /// </summary>
    FuelPressure = 0x02,

    /// <summary>
    /// Oil pressure warning.
    /// </summary>
    OilPressure = 0x04,

    /// <summary>
    /// The engine has stalled.
    /// </summary>
    EngineStalled = 0x08,

    /// <summary>
    /// The pit speed limiter is active.
    /// </summary>
    PitSpeedLimiter = 0x10,

    /// <summary>
    /// The rev limiter is active.
    /// </summary>
    RevLimiterActive = 0x20,

    /// <summary>
    /// Oil temperature warning.
    /// </summary>
    OilTemperature = 0x40
}
