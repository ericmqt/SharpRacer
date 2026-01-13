using System.Runtime.Versioning;
using SharpRacer.Commands.Interop;
using Windows.Win32;

namespace SharpRacer.Commands.VideoCapture;

/// <summary>
/// Provides methods for sending video capture commands to the simulator.
/// </summary>
public sealed class VideoCaptureCommandClient : CommandClientBase, IVideoCaptureCommandClient
{
    /// <summary>
    /// Initializes a new <see cref="VideoCaptureCommandClient"/> instance.
    /// </summary>
    [SupportedOSPlatform(PInvoke.MinOSPlatform)]
    public VideoCaptureCommandClient()
        : this(SimulatorCommandSink.Instance)
    {

    }

    /// <summary>
    /// Initializes a new <see cref="VideoCaptureCommandClient"/> instance with the specified command sink.
    /// </summary>
    /// <param name="commandSink">The <see cref="ISimulatorCommandSink"/> to use for sending commands to the simulator.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="commandSink"/> is <see langword="null"/>.
    /// </exception>
    public VideoCaptureCommandClient(ISimulatorCommandSink commandSink)
        : base(commandSink)
    {
    }

    /// <inheritdoc />
    public void HideTimer()
    {
        Send(new VideoCaptureCommand(VideoCaptureCommandType.HideVideoTimer));
    }

    /// <inheritdoc />
    public void Screenshot()
    {
        Send(new VideoCaptureCommand(VideoCaptureCommandType.CaptureScreenshot));
    }

    /// <inheritdoc />
    public void ShowTimer()
    {
        Send(new VideoCaptureCommand(VideoCaptureCommandType.ShowVideoTimer));
    }

    /// <inheritdoc />
    public void Start()
    {
        Send(new VideoCaptureCommand(VideoCaptureCommandType.BeginVideoCapture));
    }

    /// <inheritdoc />
    public void Stop()
    {
        Send(new VideoCaptureCommand(VideoCaptureCommandType.EndVideoCapture));
    }

    /// <inheritdoc />
    public void ToggleVideoCapture()
    {
        Send(new VideoCaptureCommand(VideoCaptureCommandType.ToggleVideoCapture));
    }
}
