namespace SharpRacer;

/// <summary>
/// Defines services that may be selected to perform during a pit service stop.
/// </summary>
/// <remarks>
/// See irsdk_PitSvFlags in the iRacing SDK.
/// </remarks>
[Flags]
public enum PitServiceOptions : uint
{
    /// <summary>
    /// No pit services selected.
    /// </summary>
    /// <remarks>
    /// This value is not defined in the irsdk_PitSvFlags enumeration in the iRacing SDK.
    /// </remarks>
    None = 0,

    /// <summary>
    /// Change the left-front tire.
    /// </summary>
    TireChangeLF = 0x0001,

    /// <summary>
    /// Change the right-front tire.
    /// </summary>
    TireChangeRF = 0x0002,

    /// <summary>
    /// Change the left-rear tire.
    /// </summary>
    TireChangeLR = 0x0004,

    /// <summary>
    /// Change the right-rear tire.
    /// </summary>
    TireChangeRR = 0x0008,

    /// <summary>
    /// Fill fuel.
    /// </summary>
    FuelFill = 0x0010,

    /// <summary>
    /// Windshield tear-off.
    /// </summary>
    WindshieldTearoff = 0x0020,

    /// <summary>
    /// Perform a fast repair.
    /// </summary>
    FastRepair = 0x0040
}
