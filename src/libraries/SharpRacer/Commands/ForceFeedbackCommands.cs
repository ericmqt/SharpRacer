using System.Runtime.Versioning;
using SharpRacer.Interop;

namespace SharpRacer.Commands;

/// <summary>
/// Provides static methods for force feedback commands.
/// </summary>
/// <remarks>
/// The simulator must be running for these commands to have any effect.
/// </remarks>
[SupportedOSPlatform("windows5.1.2600")]
public static class ForceFeedbackCommands
{
    /// <summary>
    /// Sets the maximum force when mapping steering wheel torque to direct input units.
    /// </summary>
    /// <param name="forceNm">The maximum force in Newton-meters.</param>
    public static void SetMaxForce(float forceNm)
    {
        BroadcastMessage.Send(SimulatorCommandId.ForceFeedbackCommand, 0, forceNm);
    }
}
