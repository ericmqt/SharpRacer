using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace SharpRacer.Simulator;

[SupportedOSPlatform("windows5.1.2600")]
internal static class BroadcastMessage
{
    private static readonly HWND HWND_BROADCAST = new(0xFFFF);

    private static readonly uint IRSDK_BROADCASTMSG = PInvoke.RegisterWindowMessage("IRSDK_BROADCASTMSG");

    public static void Send(SimulatorCommandId commandId, int param1, int param2)
    {
        PInvoke.SendNotifyMessage(
            HWND_BROADCAST,
            IRSDK_BROADCASTMSG,
            CreateWParam(commandId, param1),
            param2);
    }

    public static void Send(SimulatorCommandId commandId, int param1, float param2)
    {
        throw new NotImplementedException();
    }

    public static void Send(SimulatorCommandId commandId, int param1, int param2, int param3)
    {
        PInvoke.SendNotifyMessage(
            HWND_BROADCAST,
            IRSDK_BROADCASTMSG,
            CreateWParam(commandId, param1),
            CreateLParam(param2, param3));
    }

    private static void SendCommand(WPARAM wParam, LPARAM lParam)
    {
        PInvoke.SendNotifyMessage(HWND_BROADCAST, IRSDK_BROADCASTMSG, wParam, lParam);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static LPARAM CreateLParam(int low, int high)
    {
        return (int)(((ushort)low) | (((uint)high) << 16));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static WPARAM CreateWParam(SimulatorCommandId commandId, int high)
    {
        return ((ushort)commandId) | (((uint)high) << 16);
    }
}