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
    WaterTemperature = 0x0001,

    /// <summary>
    /// Fuel pressure warning.
    /// </summary>
    FuelPressure = 0x0002,

    /// <summary>
    /// Oil pressure warning.
    /// </summary>
    OilPressure = 0x0004,

    /// <summary>
    /// The engine has stalled.
    /// </summary>
    EngineStalled = 0x0008,

    /// <summary>
    /// The pit speed limiter is active.
    /// </summary>
    PitSpeedLimiter = 0x0010,

    /// <summary>
    /// The rev limiter is active.
    /// </summary>
    RevLimiterActive = 0x0020,

    /// <summary>
    /// Oil temperature warning.
    /// </summary>
    OilTemperature = 0x0040,

    /// <summary>
    /// Repairs are required.
    /// </summary>
    RepairRequired = 0x0080,

    /// <summary>
    /// Optional repairs are available.
    /// </summary>
    RepairAvailable = 0x0100
}
