namespace SharpRacer.Commands.PitService;

/// <summary>
/// Defines the pit services that can be cleared or reset, equivalent to unchecking the relevant checkboxes in the pit services black box.
/// </summary>
public enum PitServiceResetType : ushort
{
    /// <summary>
    /// Reset all pit service options.
    /// </summary>
    All = PitCommandType.Reset,

    /// <summary>
    /// Reset tire service options.
    /// </summary>
    TireChange = PitCommandType.ResetTireChange,

    /// <summary>
    /// Reset windscreen tear-off service.
    /// </summary>
    WindscreenTearOff = PitCommandType.ResetWindscreenTearOff,

    /// <summary>
    /// Reset fast repair service.
    /// </summary>
    FastRepair = PitCommandType.ResetFastRepair,

    /// <summary>
    /// Reset fuel service.
    /// </summary>
    Fuel = PitCommandType.ResetFuel
}
