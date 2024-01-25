using System.Runtime.Versioning;
using SharpRacer.Interop;

namespace SharpRacer.Commands;

[SupportedOSPlatform("windows5.1.2600")]
public static class TextureReloadCommands
{
    public static void ReloadAll()
    {
        BroadcastMessage.Send(SimulatorCommandId.ReloadTextures);
    }

    public static void ReloadCar(int carIdx)
    {
        BroadcastMessage.Send(SimulatorCommandId.ReloadTextures, carIdx);
    }
}
