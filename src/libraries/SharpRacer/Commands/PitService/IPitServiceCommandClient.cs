namespace SharpRacer.Commands.PitService;

/// <summary>
/// Defines methods for sending pit services commands to the simulator.
/// </summary>
public interface IPitServiceCommandClient
{
    /// <summary>
    /// Clears the specified pit service options.
    /// </summary>
    /// <param name="pitService">The pit service options to clear.</param>
    void Clear(PitServiceResetType pitService);

    /// <summary>
    /// Resets all pit service options.
    /// </summary>
    void ClearAll();

    /// <summary>
    /// Request a fast repair, if one is available.
    /// </summary>
    void RequestFastRepair();

    /// <summary>
    /// Add fuel.
    /// </summary>
    void RequestFuel();

    /// <summary>
    /// Add the specified number of liters of fuel.
    /// </summary>
    /// <param name="liters"></param>
    void RequestFuel(int liters);

    /// <summary>
    /// Request a tire change for the specified tire using the default tire pressure.
    /// </summary>
    /// <param name="tire"></param>
    void RequestTireChange(TireChangeTarget tire);

    /// <summary>
    /// Request a tire change for the specified tire and set the tire pressure.
    /// </summary>
    /// <param name="tire">The tire to change.</param>
    /// <param name="pressureKPa">The tire pressure in kilopascals (kPa).</param>
    void RequestTireChange(TireChangeTarget tire, int pressureKPa);

    /// <summary>
    /// Clean the windscreen, consuming one tear-off.
    /// </summary>
    void RequestWindscreenTearOff();
}
