namespace SharpRacer.Telemetry;

public enum PitServiceStatus : uint
{
    None = 0,
    InProgress = 1,
    Complete = 2,

    ErrorExceededPitBoxLeft = 100,
    ErrorExceededPitBoxRight = 101,
    ErrorExceededPitBoxForward = 102,
    ErrorExceededPitBoxBack = 103,
    ErrorExceededPitBoxAngle = 104,
    ErrorUnrepairable = 105
}