namespace SharpRacer.Commands.PitService;

/// <summary>
/// Defines the tire targeted by a pit service command.
/// </summary>
public enum TireChangeTarget : byte
{
    /// <summary>
    /// The left-front tire.
    /// </summary>
    LeftFront = 0,

    /// <summary>
    /// The right-front tire.
    /// </summary>
    RightFront = 1,

    /// <summary>
    /// The left-rear tire.
    /// </summary>
    LeftRear = 2,

    /// <summary>
    /// The right-rear tire.
    /// </summary>
    RightRear = 3
}
