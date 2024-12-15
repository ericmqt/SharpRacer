using System.Runtime.Versioning;
using SharpRacer.Interop;

namespace SharpRacer.Commands;

/// <summary>
/// Provides static methods for invoking custom car texture reload commands.
/// </summary>
/// <remarks>
/// The simulator must be running for these commands to have any effect.
/// </remarks>
[SupportedOSPlatform("windows5.1.2600")]
public static class TextureReloadCommands
{
    /// <summary>
    /// Reloads custom textures for all cars.
    /// </summary>
    public static void ReloadAll()
    {
        BroadcastMessage.Send(SimulatorCommandId.ReloadTextures);
    }

    /// <summary>
    /// Reloads the custom texture for the car at the specified index.
    /// </summary>
    /// <param name="carIdx">The car index.</param>
    public static void ReloadCar(int carIdx)
    {
        BroadcastMessage.Send(SimulatorCommandId.ReloadTextures, carIdx);
    }
}
