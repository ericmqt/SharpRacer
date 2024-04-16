namespace SharpRacer;

/// <summary>
/// Describes spotter callouts for cars alongside the driver.
/// </summary>
/// <remarks>See: irsdk_CarLeftRight</remarks>
public enum CarLeftRight : uint
{
    /// <summary>
    /// No cars are alongside the driver.
    /// </summary>
    None = 0,

    /// <summary>
    /// The driver car has transitioned from having one or more cars alongside to none.
    /// </summary>
    Clear = 1,

    /// <summary>
    /// There is a car to the driver's left.
    /// </summary>
    CarLeft = 2,

    /// <summary>
    /// There is a car to the driver's right.
    /// </summary>
    CarRight = 3,

    /// <summary>
    /// There is a car on both sides of the driver.
    /// </summary>
    CarLeftRight = 4,

    /// <summary>
    /// There are two cars to the left of the driver.
    /// </summary>
    TwoCarsLeft = 5,

    /// <summary>
    /// There are two cars to the right of the driver.
    /// </summary>
    TwoCarsRight = 6
}
