namespace SharpRacer;

/// <summary>
/// Defines session state types.
/// </summary>
/// <remarks>
/// See irsdk_SessionState in the iRacing SDK.
/// </remarks>
public enum SessionState : uint
{
    /// <summary>
    /// Invalid session state.
    /// </summary>
    Invalid = 0,

    /// <summary>
    /// Waiting for drivers to enter their cars.
    /// </summary>
    GetInCar = 1,

    /// <summary>
    /// Warmup period.
    /// </summary>
    Warmup = 2,

    /// <summary>
    /// Parade laps.
    /// </summary>
    ParadeLaps = 3,

    /// <summary>
    /// Racing.
    /// </summary>
    Racing = 4,

    /// <summary>
    /// Checkered flag is flying.
    /// </summary>
    Checkered = 5,

    /// <summary>
    /// Post-race cool-down period.
    /// </summary>
    CoolDown = 6
}
