namespace SharpRacer.Telemetry;

public enum SessionState : uint
{
    Invalid = 0,
    GetInCar = 1,
    Warmup = 2,
    ParadeLaps = 3,
    Racing = 4,
    Checkered = 5,
    CoolDown = 6
}