namespace SharpRacer.Enums;

/// <summary>
/// Defines penalties applied to an incident.
/// </summary>
public enum IncidentPenalty : ushort
{
    /// <summary>
    /// No penalty.
    /// </summary>
    NoPenalty = 0x0000,

    /// <summary>
    /// A penalty of 0x.
    /// </summary>
    ZeroX = 0x0100,

    /// <summary>
    /// A penalty of 1x.
    /// </summary>
    OneX = 0x0200,

    /// <summary>
    /// A penalty of 2x.
    /// </summary>
    TwoX = 0x0300,

    /// <summary>
    /// A penalty of 4x.
    /// </summary>
    FourX = 0x0400
}
