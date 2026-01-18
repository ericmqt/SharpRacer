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
    public void TargetCar(CarNumber carNumber, int cameraGroup, int cameraIndex)
    {
        if (carNumber == default)
        {
            throw new ArgumentException(
                $"'{nameof(carNumber)}' cannot be equal to {nameof(CarNumber)}.{nameof(CarNumber.None)}.",
                nameof(carNumber));
        }

        Send(new TargetCarCommand(carNumber, cameraGroup, cameraIndex));
    }

    /// <inheritdoc />
    public void TargetCar(CameraTargetMode targetMode, int cameraGroup, int cameraIndex)
    {
        Send(new TargetCarCommand(targetMode, cameraGroup, cameraIndex));
    }

    /// <inheritdoc />
    public void TargetCarByPosition(int position, int cameraGroup, int cameraIndex)
    {
        Send(new TargetRacePositionCommand(position, cameraGroup, cameraIndex));
    }

    /// <inheritdoc />
    public void TargetCarByPosition(CameraTargetMode targetMode, int cameraGroup, int cameraIndex)
    {
        Send(new TargetRacePositionCommand(targetMode, cameraGroup, cameraIndex));
    }
}
