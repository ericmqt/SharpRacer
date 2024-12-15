using System.Runtime.Versioning;
using SharpRacer.Interop;

namespace SharpRacer.Commands;

/// <summary>
/// Provides static methods for invoking camera commands.
/// </summary>
/// <remarks>
/// The simulator must be running for these commands to have any effect.
/// </remarks>
[SupportedOSPlatform("windows5.1.2600")]
public static class CameraCommands
{
    /// <summary>
    /// Sets the camera state.
    /// </summary>
    /// <param name="cameraState">The camera state.</param>
    /// <remarks>See irsdk_BroadcastCamSetState</remarks>
    public static void SetState(CameraState cameraState)
    {
        BroadcastMessage.Send(SimulatorCommandId.CameraSetState, (ushort)cameraState);
    }

    /// <summary>
    /// Sets the camera to target the car in the specified race position.
    /// </summary>
    /// <param name="position">The race position of the car to target.</param>
    /// <param name="cameraGroup">The camera group.</param>
    /// <param name="cameraIndex">The camera index.</param>
    /// <remarks>See irsdk_BroadcastCamSwitchPos</remarks>
    public static void TargetCarPosition(int position, int cameraGroup, int cameraIndex)
    {
        BroadcastMessage.Send(SimulatorCommandId.CameraSwitchPosition, position, cameraGroup, cameraIndex);
    }

    /// <summary>
    /// Sets the camera to use the specified target mode.
    /// </summary>
    /// <param name="targetMode">The camera target mode.</param>
    /// <param name="cameraGroup">The camera group.</param>
    /// <param name="cameraIndex">The camera index.</param>
    /// <remarks>See irsdk_BroadcastCamSwitchPos, irsdk_csMode</remarks>
    public static void TargetCarPosition(CameraTargetMode targetMode, int cameraGroup, int cameraIndex)
    {
        BroadcastMessage.Send(SimulatorCommandId.CameraSwitchPosition, (int)targetMode, cameraGroup, cameraIndex);
    }

    /// <summary>
    /// Sets the camera to target the car driven by the specified driver.
    /// </summary>
    /// <param name="driverNumber"></param>
    /// <param name="cameraGroup">The camera group.</param>
    /// <param name="cameraIndex">The camera index.</param>
    /// <remarks>See irsdk_BroadcastCamSwitchNum</remarks>
    public static void TargetDriver(int driverNumber, int cameraGroup, int cameraIndex)
    {
        BroadcastMessage.Send(SimulatorCommandId.CameraSwitchNumber, driverNumber, cameraGroup, cameraIndex);
    }

    /// <summary>
    /// Sets the camera to use the specified target mode.
    /// </summary>
    /// <param name="targetMode">The camera target mode.</param>
    /// <param name="cameraGroup">The camera group.</param>
    /// <param name="cameraIndex">The camera index.</param>
    /// <remarks>See irsdk_BroadcastCamSwitchNum, irsdk_csMode</remarks>
    public static void TargetDriver(CameraTargetMode targetMode, int cameraGroup, int cameraIndex)
    {
        BroadcastMessage.Send(SimulatorCommandId.CameraSwitchNumber, (int)targetMode, cameraGroup, cameraIndex);
    }
}
