using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using SharpRacer.Commands;
using Windows.Win32;

namespace SharpRacer.Interop;

/// <summary>
/// Facilitates sending messages to the simulator via the SendNotifyMessage Win32 API.
/// </summary>
[SupportedOSPlatform(PInvoke.MinOSPlatform)]
internal sealed class SimulatorNotifyMessageSink : ISimulatorNotifyMessageSink
{
    private static readonly uint IRSDK_BROADCASTMSG = PInvoke.RegisterWindowMessage(CommandMessageConstants.WindowMessageName);

    internal SimulatorNotifyMessageSink()
    {

    }

    /// <summary>
    /// Gets the default <see cref="SimulatorNotifyMessageSink"/> instance.
    /// </summary>
    public static ISimulatorNotifyMessageSink Instance { get; } = new SimulatorNotifyMessageSink();

    /// <inheritdoc />
    public void Send(SimulatorNotifyMessageData message)
    {
        if (!PInvoke.SendNotifyMessage(
            PInvoke.HWND_BROADCAST, IRSDK_BROADCASTMSG, message.WParam, message.LParam))
        {
            throw new Win32Exception(Marshal.GetLastPInvokeError(), Marshal.GetLastPInvokeErrorMessage());
        }
    }
}
