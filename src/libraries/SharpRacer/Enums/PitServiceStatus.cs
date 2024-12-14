namespace SharpRacer;

/// <summary>
/// Defines status and error types for pit services.
/// </summary>
/// <remarks>
/// See irsdk_PitSvStatus in the iRacing SDK.
/// </remarks>
public enum PitServiceStatus : uint
{
    /// <summary>
    /// None.
    /// </summary>
    None = 0,

    /// <summary>
    /// Pit service in progress.
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Pit service complete.
    /// </summary>
    Complete = 2,

    /// <summary>
    /// Driver is too far to the left in the pit box.
    /// </summary>
    ErrorExceededPitBoxLeft = 100,

    /// <summary>
    /// Driver is too far to the right in the pit box.
    /// </summary>
    ErrorExceededPitBoxRight = 101,

    /// <summary>
    /// Driver is too far forward in the pit box.
    /// </summary>
    ErrorExceededPitBoxForward = 102,

    /// <summary>
    /// Driver is too far back in the pit box.
    /// </summary>
    ErrorExceededPitBoxBack = 103,

    /// <summary>
    /// Driver is at an incorrect angle in the pit box.
    /// </summary>
    ErrorExceededPitBoxAngle = 104,

    /// <summary>
    /// Pit service is not possible because the car is unrepairable.
    /// </summary>
    ErrorUnrepairable = 105
}
