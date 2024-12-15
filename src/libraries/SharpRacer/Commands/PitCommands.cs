using System.Runtime.Versioning;
using SharpRacer.Interop;

namespace SharpRacer.Commands;

/// <summary>
/// Provides static methods for invoking pit service commands.
/// </summary>
/// <remarks>
/// The simulator must be running for these commands to have any effect.
/// </remarks>
[SupportedOSPlatform("windows5.1.2600")]
public static class PitCommands
{
    /// <summary>
    /// Clear all pit service options by clearing the checkboxes.
    /// </summary>
    public static void ClearAll()
    {
        SendCommand(PitCommandType.Reset);
    }

    /// <summary>
    /// Clear the fast repair checkbox.
    /// </summary>
    public static void ClearFastRepair()
    {
        SendCommand(PitCommandType.ResetFastRepair);
    }

    /// <summary>
    /// Clear the fuel checkbox.
    /// </summary>
    public static void ClearFuel()
    {
        SendCommand(PitCommandType.ResetFuel);
    }

    /// <summary>
    /// Clear the tire change checkboxes.
    /// </summary>
    public static void ClearTireChange()
    {
        SendCommand(PitCommandType.ResetTireChange);
    }

    /// <summary>
    /// Clear the windscreen tear-off checkbox.
    /// </summary>
    public static void ClearWindscreenTearOff()
    {
        SendCommand(PitCommandType.ResetWindscreenTearOff);
    }

    /// <summary>
    /// Use a fast repair, if available.
    /// </summary>
    public static void FastRepair()
    {
        SendCommand(PitCommandType.FastRepair);
    }

    /// <summary>
    /// Add fuel without changing the configured fuel amount.
    /// </summary>
    public static void Fuel()
    {
        Fuel(liters: 0);
    }

    /// <summary>
    /// Add a specified number of liters of fuel.
    /// </summary>
    /// <param name="liters">The amount of fuel to add in liters.</param>
    public static void Fuel(int liters)
    {
        SendCommand(PitCommandType.AddFuel, liters);
    }

    /// <summary>
    /// Change the left-front tire without changing the configured tire pressure.
    /// </summary>
    public static void TireChangeLeftFront()
    {
        TireChangeLeftFront(pressureKPa: 0);
    }

    /// <summary>
    /// Change the left-front tire and set the tire pressure.
    /// </summary>
    /// <param name="pressureKPa">The tire pressure in kilopascals (kPa).</param>
    public static void TireChangeLeftFront(int pressureKPa)
    {
        SendCommand(PitCommandType.TireChangeLeftFront, pressureKPa);
    }

    /// <summary>
    /// Change the left-rear tire without changing the configured tire pressure.
    /// </summary>
    public static void TireChangeLeftRear()
    {
        TireChangeLeftRear(pressureKPa: 0);
    }

    /// <summary>
    /// Change the left-rear tire and set the tire pressure.
    /// </summary>
    /// <param name="pressureKPa">The tire pressure in kilopascals (kPa).</param>
    public static void TireChangeLeftRear(int pressureKPa)
    {
        SendCommand(PitCommandType.TireChangeLeftRear, pressureKPa);
    }

    /// <summary>
    /// Change the right-front tire without changing the configured tire pressure.
    /// </summary>
    public static void TireChangeRightFront()
    {
        TireChangeRightFront(pressureKPa: 0);
    }

    /// <summary>
    /// Change the right-front tire and set the tire pressure.
    /// </summary>
    /// <param name="pressureKPa">The tire pressure in kilopascals (kPa).</param>
    public static void TireChangeRightFront(int pressureKPa)
    {
        SendCommand(PitCommandType.TireChangeRightFront, pressureKPa);
    }

    /// <summary>
    /// Change the right-rear tire without changing the configured tire pressure.
    /// </summary>
    public static void TireChangeRightRear()
    {
        TireChangeRightRear(pressureKPa: 0);
    }

    /// <summary>
    /// Change the right-rear tire and set the tire pressure.
    /// </summary>
    /// <param name="pressureKPa">The tire pressure in kilopascals (kPa).</param>
    public static void TireChangeRightRear(int pressureKPa)
    {
        SendCommand(PitCommandType.TireChangeRightRear, pressureKPa);
    }

    /// <summary>
    /// Clean the windscreen, consuming one tear-off.
    /// </summary>
    public static void WindscreenTearOff()
    {
        SendCommand(PitCommandType.WindscreenTearOff);
    }

    private static void SendCommand(PitCommandType pitCommand)
    {
        BroadcastMessage.Send(SimulatorCommandId.PitCommand, (ushort)pitCommand);
    }

    private static void SendCommand(PitCommandType pitCommand, int arg1)
    {
        BroadcastMessage.Send(SimulatorCommandId.PitCommand, (ushort)pitCommand, arg1);
    }
}
