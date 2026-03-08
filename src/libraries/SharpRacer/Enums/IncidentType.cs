namespace SharpRacer.Enums;

/// <summary>
/// Defines incident types.
/// </summary>
public enum IncidentType : ushort
{
    /// <summary>
    /// Indicates no incident was reported.
    /// </summary>
    None = 0x0000,

    /// <summary>
    /// Out-of-control.
    /// </summary>
    OutOfControl = 0x0001,

    /// <summary>
    /// Off-track.
    /// </summary>
    OffTrack = 0x0002,

    /// <summary>
    /// Off-track (ongoing).
    /// </summary>
    OffTrackOngoing = 0x0003,

    /// <summary>
    /// Contact with the world.
    /// </summary>
    ContactWithWorld = 0x0004,

    /// <summary>
    /// Collision with the world.
    /// </summary>
    CollisionWithWorld = 0x0005,

    /// <summary>
    /// Collision with the world (ongoing).
    /// </summary>
    CollisionWithWorldOngoing = 0x0006,

    /// <summary>
    /// Contact with another car.
    /// </summary>
    ContactWithCar = 0x0007,

    /// <summary>
    /// Collsion with another car.
    /// </summary>
    CollisionWithCar = 0x0008
}
