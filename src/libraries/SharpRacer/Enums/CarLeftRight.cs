namespace SharpRacer;

/// <summary>
/// Defines spotter callouts for cars alongside the driver.
/// </summary>
/// <remarks>See irsdk_CarLeftRight in the iRacing SDK.</remarks>
public enum CarLeftRight : uint
{
    /// <summary>
    /// No cars are alongside the driver.
    /// </summary>
    None = 0,

    /// <summary>
    /// Driver car no longer has any cars alongside.
    /// </summary>
    Clear = 1,

    /// <summary>
    /// Car to the driver's left.
    /// </summary>
    CarLeft = 2,

    /// <summary>
    /// Car to the driver's right.
    /// </summary>
    CarRight = 3,

    /// <summary>
    /// Car on both sides of the driver.
    /// </summary>
    CarLeftRight = 4,

    /// <summary>
    /// Two cars to the left of the driver.
    /// </summary>
    TwoCarsLeft = 5,

    /// <summary>
    /// Two cars to the right of the driver.
    /// </summary>
    TwoCarsRight = 6
}
