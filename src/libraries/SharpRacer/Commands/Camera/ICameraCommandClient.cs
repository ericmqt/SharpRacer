namespace SharpRacer.Commands.Camera;

/// <summary>
/// Defines methods for sending camera commands to the simulator.
/// </summary>
public interface ICameraCommandClient
{
    /// <summary>
    /// Sets the camera state.
    /// </summary>
    /// <param name="cameraState">The camera state.</param>
    /// <remarks>See irsdk_BroadcastCamSetState in the iRacing SDK.</remarks>
    void SetState(CameraState cameraState);

    /// <summary>
    /// Sets the camera to target the car driven by the specified driver.
    /// </summary>
    /// <param name="driverNumber"></param>
    /// <param name="cameraGroup">The camera group.</param>
    /// <param name="cameraIndex">The camera index.</param>
    /// <remarks>See irsdk_BroadcastCamSwitchNum in the iRacing SDK.</remarks>
    void TargetDriver(int driverNumber, int cameraGroup, int cameraIndex);

    /// <summary>
    /// Sets the camera to use the specified <see cref="CameraTargetMode">CameraTargetMode</see>.
    /// </summary>
    /// <param name="targetMode">The camera target mode.</param>
    /// <param name="cameraGroup">The camera group.</param>
    /// <param name="cameraIndex">The camera index.</param>
    /// <remarks>See irsdk_BroadcastCamSwitchNum and irsdk_csMode in the iRacing SDK.</remarks>
    void TargetDriver(CameraTargetMode targetMode, int cameraGroup, int cameraIndex);

    /// <summary>
    /// Sets the camera to target the car in the specified race position.
    /// </summary>
    /// <param name="position">The race position of the car to target.</param>
    /// <param name="cameraGroup">The camera group.</param>
    /// <param name="cameraIndex">The camera index.</param>
    /// <remarks>See irsdk_BroadcastCamSwitchPos in the iRacing SDK.</remarks>
    void TargetRacePosition(int position, int cameraGroup, int cameraIndex);

    /// <summary>
    /// Sets the camera to use the specified <see cref="CameraTargetMode">CameraTargetMode</see>.
    /// </summary>
    /// <param name="targetMode">The camera target mode.</param>
    /// <param name="cameraGroup">The camera group.</param>
    /// <param name="cameraIndex">The camera index.</param>
    /// <remarks>See irsdk_BroadcastCamSwitchPos and irsdk_csMode in the iRacing SDK.</remarks>
    void TargetRacePosition(CameraTargetMode targetMode, int cameraGroup, int cameraIndex);
}
