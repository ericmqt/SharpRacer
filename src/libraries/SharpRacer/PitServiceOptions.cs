namespace SharpRacer;

/// <summary>
/// Describes the services to perform during a pit stop.
/// </summary>
[Flags]
public enum PitServiceOptions : uint
{
    None = 0,
    TireChangeLF = 0x0001,
    TireChangeRF = 0x0002,
    TireChangeLR = 0x0004,
    TireChangeRR = 0x0008,
    FuelFill = 0x0010,
    WindshieldTearoff = 0x0020,
    FastRepair = 0x0040
}
