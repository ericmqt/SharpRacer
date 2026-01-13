namespace SharpRacer.Commands.PitService;

/// <summary>
/// Defines pit service command types.
/// </summary>
/// <remarks>
/// See irsdk_PitCommandMode in the iRacing SDK.
/// </remarks>
public enum PitCommandType : ushort
{
    /// <summary>
    /// Reset all pit service options by clearing the checkboxes.
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
    /// Change the left-front tire.
    /// </summary>
    TireChangeLeftFront = 3,

    /// <summary>
    /// Change the right-front tire.
    /// </summary>
    TireChangeRightFront = 4,

    /// <summary>
    /// Change the left-rear tire.
    /// </summary>
    TireChangeLeftRear = 5,

    /// <summary>
    /// Change the right-rear tire.
    /// </summary>
    TireChangeRightRear = 6,

    /// <summary>
    /// Reset tire change options by clearing the checkbox.
    /// </summary>
    ResetTireChange = 7,

    /// <summary>
    /// Request a fast repair.
    /// </summary>
    FastRepair = 8,

    /// <summary>
    /// Reset windscreen tear-off pit service option by clearing the checkbox.
    /// </summary>
    ResetWindscreenTearOff = 9,

    /// <summary>
    /// Reset fast repair pit service option by clearing the checkbox.
    /// </summary>
    ResetFastRepair = 10,

    /// <summary>
    /// Reset fuel pit service option by clearing the checkbox.
    /// </summary>
    ResetFuel = 11
}
