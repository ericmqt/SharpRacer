namespace SharpRacer;

/// <summary>
/// Represents a single combination of <see cref="IncidentType"/> and <see cref="IncidentPenalty"/>.
/// </summary>
/// <remarks>
/// The first byte (0x00FF) is the <see cref="IncidentType"/> value and the second byte (0xFF00) is the
/// <see cref="IncidentPenalty"/> value.
/// 
/// See irsdk_IncidentFlags in the iRacing SDK.
/// </remarks>
[Flags]
public enum IncidentFlags : ushort
{
    /// <summary>
    /// Indicates no incident or no penalty applied to a reported incident.
    /// </summary>
    None = IncidentType.None,

    /// <summary>
    /// Out-of-control.
    /// </summary>
    OutOfControl = IncidentType.OutOfControl,

    /// <summary>
    /// Off-track.
    /// </summary>
    OffTrack = IncidentType.OffTrack,

    /// <summary>
    /// Off-track (ongoing).
    /// </summary>
    OffTrackOngoing = IncidentType.OffTrackOngoing,

    /// <summary>
    /// Contact with the world.
    /// </summary>
    ContactWithWorld = IncidentType.ContactWithWorld,

    /// <summary>
    /// Collision with the world.
    /// </summary>
    CollisionWithWorld = IncidentType.CollisionWithWorld,

    /// <summary>
    /// Collision with the world (ongoing).
    /// </summary>
    CollisionWithWorldOngoing = IncidentType.CollisionWithWorldOngoing,

    /// <summary>
    /// Contact with another car.
    /// </summary>
    ContactWithCar = IncidentType.ContactWithCar,

    /// <summary>
    /// Collision with another car.
    /// </summary>
    CollisionWithCar = IncidentType.CollisionWithCar,

    /// <summary>
    /// The incident incurred a 0x penalty.
    /// </summary>
    Penalty0x = IncidentPenalty.ZeroX,

    /// <summary>
    /// The incident incurred a 1x penalty.
    /// </summary>
    Penalty1x = IncidentPenalty.OneX,

    /// <summary>
    /// The incident incurred a 2x penalty.
    /// </summary>
    Penalty2x = IncidentPenalty.TwoX,

    /// <summary>
    /// The incident incurred a 4x penalty.
    /// </summary>
    Penalty4x = IncidentPenalty.FourX
}
