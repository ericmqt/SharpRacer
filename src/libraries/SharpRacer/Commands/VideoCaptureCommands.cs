using System.Runtime.Versioning;
using SharpRacer.Interop;

namespace SharpRacer.Commands;

[SupportedOSPlatform("windows5.1.2600")]
public static class VideoCaptureCommands
{
    /// <summary>
    /// Begin capturing video.
    /// </summary>
    public static void BeginVideoCapture()
    {
        BroadcastMessage.Send(SimulatorCommandId.VideoCapture, (ushort)VideoCaptureCommandType.BeginVideoCapture);
    }

    /// <summary>
    /// Save a screenshot to disk.
    /// </summary>
    public static void CaptureScreenshot()
    {
        BroadcastMessage.Send(SimulatorCommandId.VideoCapture, (ushort)VideoCaptureCommandType.CaptureScreenshot);
    }

    /// <summary>
    /// End video capture.
    /// </summary>
    public static void EndVideoCapture()
    {
        BroadcastMessage.Send(SimulatorCommandId.VideoCapture, (ushort)VideoCaptureCommandType.EndVideoCapture);
    }

    /// <summary>
    /// Hide the video timer.
    /// </summary>
    public static void HideVideoTimer()
    {
        BroadcastMessage.Send(SimulatorCommandId.VideoCapture, (ushort)VideoCaptureCommandType.HideVideoTimer);
    }

    /// <summary>
    /// Show the video timer.
    /// </summary>
    public static void ShowVideoTimer()
    {
        BroadcastMessage.Send(SimulatorCommandId.VideoCapture, (ushort)VideoCaptureCommandType.ShowVideoTimer);
    }

    /// <summary>
    /// Toggle video capture on and off.
    /// </summary>
    public static void ToggleVideoCapture()
    {
        BroadcastMessage.Send(SimulatorCommandId.VideoCapture, (ushort)VideoCaptureCommandType.ToggleVideoCapture);
    }
}
