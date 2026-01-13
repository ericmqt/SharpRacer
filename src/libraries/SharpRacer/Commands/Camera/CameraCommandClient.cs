using System.Runtime.Versioning;
using SharpRacer.Commands.Interop;
using Windows.Win32;

namespace SharpRacer.Commands.Camera;

/// <summary>
/// Provides methods for sending camera commands to the simulator.
/// </summary>
public class CameraCommandClient : CommandClientBase, ICameraCommandClient
{
    /// <summary>
    /// Initializes a new <see cref="CameraCommandClient"/> instance.
    /// </summary>
    [SupportedOSPlatform(PInvoke.MinOSPlatform)]
    public CameraCommandClient()
        : this(SimulatorCommandSink.Instance)
    {

    }

    /// <summary>
    /// Initializes a new <see cref="CameraCommandClient"/> with the specified command sink.
    /// </summary>
    /// <param name="commandSink">The <see cref="ISimulatorCommandSink"/> to use for sending commands to the simulator.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="commandSink"/> is <see langword="null"/>.
    /// </exception>
    public CameraCommandClient(ISimulatorCommandSink commandSink)
        : base(commandSink)
    {

    }

    /// <inheritdoc />
    public void SetState(CameraState cameraState)
    {
        Send(new SetCameraStateCommand(cameraState));
    }

    /// <inheritdoc />
    public void TargetDriver(int driverNumber, int cameraGroup, int cameraIndex)
    {
        Send(new TargetDriverCommand(driverNumber, cameraGroup, cameraIndex));
    }

    /// <inheritdoc />
    public void TargetDriver(CameraTargetMode targetMode, int cameraGroup, int cameraIndex)
    {
        Send(new TargetDriverCommand(targetMode, cameraGroup, cameraIndex));
    }

    /// <inheritdoc />
    public void TargetRacePosition(int position, int cameraGroup, int cameraIndex)
    {
        Send(new TargetRacePositionCommand(position, cameraGroup, cameraIndex));
    }

    /// <inheritdoc />
    public void TargetRacePosition(CameraTargetMode targetMode, int cameraGroup, int cameraIndex)
    {
        Send(new TargetRacePositionCommand(targetMode, cameraGroup, cameraIndex));
    }
}
