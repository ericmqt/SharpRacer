using System.Runtime.Versioning;
using SharpRacer.Interop;

namespace SharpRacer.Commands;

[SupportedOSPlatform("windows5.1.2600")]
public static class ReplayCommands
{
    public static void EraseTape()
    {
        BroadcastMessage.Send(SimulatorCommandId.ReplaySetState, 0);
    }

    public static void Search(ReplaySearchMode searchMode)
    {
        BroadcastMessage.Send(SimulatorCommandId.ReplaySearch, (ushort)searchMode);
    }

    public static void SearchSessionTime(int sessionNumber, int sessionTimeMs)
    {
        BroadcastMessage.Send(SimulatorCommandId.ReplaySearchSessionTime, sessionNumber, sessionTimeMs);
    }

    public static void SetPlayPosition(ReplaySeekMode seekMode, int frameNumber)
    {
        BroadcastMessage.Send(SimulatorCommandId.ReplaySetPlayPosition, (ushort)seekMode, frameNumber);
    }

    public static void SetPlaySpeed(int playSpeed, bool isSlowMotion)
    {
        BroadcastMessage.Send(SimulatorCommandId.ReplaySetPlaySpeed, playSpeed, isSlowMotion ? 1 : 0);
    }
}
