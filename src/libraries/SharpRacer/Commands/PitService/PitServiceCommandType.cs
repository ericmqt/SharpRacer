namespace SharpRacer.Commands.PitService;

/// <summary>
/// Defines subcommands for the <see cref="SimulatorCommandId.PitService"/> simulator command.
/// </summary>
/// <seealso cref="SimulatorCommandId"/>
public enum PitServiceCommandType : ushort
{
    /// <summary>
    /// Reset all pit service options.
    /// </summary>
    Reset = 0,

    /// <summary>
    /// Clean the windscreen, using one tear-off.
    /// </summary>
    WindscreenTearOff = 1,

    /// <summary>
    /// Add fuel.
    /// </summary>
    AddFuel = 2,

    /// <summary>
    /// Change a tire and optionally set the tire pressure.
    /// </summary>
    TireChange = 3,

    /// <summary>
    /// Reset all tire change options.
    /// </summary>
    ResetTireChange = 7,

    /// <summary>
    /// Request a fast repair.
    /// </summary>
    FastRepair = 8,

    /// <summary>
    /// Reset the windscreen tear-off pit service option.
    /// </summary>
    ResetWindscreenTearOff = 9,

    /// <summary>
    /// Reset the fast repair pit service option.
    /// </summary>
    ResetFastRepair = 10,

    /// <summary>
    /// Reset fuel pit service options.
    /// </summary>
    ResetFuel = 11
}
