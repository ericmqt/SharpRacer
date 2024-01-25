using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using SharpRacer.Commands;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace SharpRacer.Interop;

/// <summary>
/// Sends Win32 messages to the simulator using HWND_BROADCAST.
/// </summary>
[SupportedOSPlatform("windows5.1.2600")]
internal static class BroadcastMessage
{
    private static readonly HWND HWND_BROADCAST = new(0xFFFF);
    private static readonly uint IRSDK_BROADCASTMSG = PInvoke.RegisterWindowMessage("IRSDK_BROADCASTMSG");

    internal static void Send(SimulatorCommandId commandId)
    {
        SendMessage(CreateWParam(commandId, 0), 0);
    }

    internal static void Send(SimulatorCommandId commandId, ushort arg1)
    {
        SendMessage(CreateWParam(commandId, arg1), 0);
    }

    internal static void Send(SimulatorCommandId commandId, int arg1)
    {
        SendMessage(CreateWParam(commandId, arg1), 0);
    }

    internal static void Send(SimulatorCommandId commandId, int arg1, int arg2)
    {
        SendMessage(CreateWParam(commandId, arg1), arg2);
    }

    internal static void Send(SimulatorCommandId commandId, int arg1, float arg2)
    {
        int real = (int)(arg2 * 65536.0f);

        SendMessage(CreateWParam(commandId, arg1), real);
    }

    internal static void Send(SimulatorCommandId commandId, int arg1, int arg2, int arg3)
    {
        var lParam = (int)((ushort)arg2 | (uint)arg3 << 16);

        SendMessage(CreateWParam(commandId, arg1), lParam);
    }

    /// <summary>
    /// Sends a message using HWND_BROADCAST.
    /// </summary>
    /// <param name="wParam"></param>
    /// <param name="lParam"></param>
    /// <exception cref="Win32Exception">Thrown when SendNotifyMessage returns zero.</exception>
    private static void SendMessage(WPARAM wParam, LPARAM lParam)
    {
        if (!PInvoke.SendNotifyMessage(HWND_BROADCAST, IRSDK_BROADCASTMSG, wParam, lParam))
        {
            throw new Win32Exception(Marshal.GetLastPInvokeError(), Marshal.GetLastPInvokeErrorMessage());
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static WPARAM CreateWParam(ushort messageId, int arg1)
    {
        return messageId | (uint)arg1 << 16;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static WPARAM CreateWParam(SimulatorCommandId commandId, int arg1)
    {
        return (ushort)commandId | (uint)arg1 << 16;
    }
}
