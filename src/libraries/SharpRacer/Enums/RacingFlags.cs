namespace SharpRacer;

/// <summary>
/// Defines racing flag types.
/// </summary>
/// <remarks>
/// Not all values correspond to flags shown to drivers in the game.
/// 
/// See irsdk_Flags in the iRacing SDK.
/// </remarks>
[Flags]
public enum RacingFlags : uint
{
    /// <summary>
    /// No flags.
    /// </summary>
    /// <remarks>
    /// This value is not defined in the irsdk_Flags enumeration in the iRacing SDK.
    /// </remarks>
    None = 0x00000000,

    /// <summary>
    /// Checkered flag.
    /// </summary>
    Checkered = 0x00000001,

    /// <summary>
    /// White flag.
    /// </summary>
    White = 0x00000002,

    /// <summary>
    /// Green flag.
    /// </summary>
    Green = 0x00000004,

    /// <summary>
    /// Yellow flag.
    /// </summary>
    Yellow = 0x00000008,

    /// <summary>
    /// Red flag.
    /// </summary>
    Red = 0x00000010,

    /// <summary>
    /// Blue flag.
    /// </summary>
    Blue = 0x00000020,

    /// <summary>
    /// Debris flag.
    /// </summary>
    Debris = 0x00000040,

    /// <summary>
    /// Crossed flag.
    /// </summary>
    Crossed = 0x00000080,

    /// <summary>
    /// Yellow waving flag.
    /// </summary>
    YellowWaving = 0x00000100,

    /// <summary>
    /// Flag indicating one lap remains until a green flag.
    /// </summary>
    OneLapToGreen = 0x00000200,

    /// <summary>
    /// Held green flag. 
    /// </summary>
    GreenHeld = 0x00000400,

    /// <summary>
    /// Flag indicating that ten laps remain.
    /// </summary>
    TenToGo = 0x00000800,

    /// <summary>
    /// Flag indicating that five laps remain.
    /// </summary>
    FiveToGo = 0x00001000,

    /// <summary>
    /// Random waving flag.
    /// </summary>
    RandomWaving = 0x00002000,

    /// <summary>
    /// Caution flag.
    /// </summary>
    Caution = 0x00004000,

    /// <summary>
    /// Waving caution flag.
    /// </summary>
    CautionWaving = 0x00008000,

    /// <summary>
    /// Black flag.
    /// </summary>
    Black = 0x00010000,

    /// <summary>
    /// Flag indicating disqualification.
    /// </summary>
    Disqualify = 0x00020000,

    /// <summary>
    /// Indicates car is allowed service. Per iRacing SDK, this value is "not a flag".
    /// </summary>
    Servicible = 0x00040000,

    /// <summary>
    /// Furled flag.
    /// </summary>
    Furled = 0x00080000,

    /// <summary>
    /// Indicates repair is required.
    /// </summary>
    Repair = 0x00100000,

    /// <summary>
    /// Starting lights are hidden.
    /// </summary>
    StartHidden = 0x10000000,

    /// <summary>
    /// Starting lights are in the "ready" phase.
    /// </summary>
    StartReady = 0x20000000,

    /// <summary>
    /// Starting lights are in the "set" phase.
    /// </summary>
    StartSet = 0x40000000,

    /// <summary>
    /// Starting lights are in the "go" phase.
    /// </summary>
    StartGo = 0x80000000,
}
