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
    /// Sets the camera to target a car by its car number.
    /// </summary>
    /// <param name="carNumber">The number of the car to target.</param>
    /// <param name="cameraGroup">The camera group.</param>
    /// <param name="cameraIndex">The camera index.</param>
    /// <remarks>See irsdk_BroadcastCamSwitchNum in the iRacing SDK.</remarks>
    /// <exception cref="ArgumentException">
    /// <paramref name="carNumber"/> equals <see cref="CarNumber.None"/>.
    /// </exception>
    void TargetCar(CarNumber carNumber, int cameraGroup, int cameraIndex);

    /// <summary>
    /// Sets the camera to use the specified <see cref="CameraTargetMode">CameraTargetMode</see>.
    /// </summary>
    /// <param name="targetMode">The camera target mode.</param>
    /// <param name="cameraGroup">The camera group.</param>
    /// <param name="cameraIndex">The camera index.</param>
    /// <remarks>See irsdk_BroadcastCamSwitchNum and irsdk_csMode in the iRacing SDK.</remarks>
    void TargetCar(CameraTargetMode targetMode, int cameraGroup, int cameraIndex);

    /// <summary>
    /// Sets the camera to target the car in the specified position.
    /// </summary>
    /// <param name="position">The race position of the car to target.</param>
    /// <param name="cameraGroup">The camera group.</param>
    /// <param name="cameraIndex">The camera index.</param>
    /// <remarks>See irsdk_BroadcastCamSwitchPos in the iRacing SDK.</remarks>
    void TargetCarByPosition(int position, int cameraGroup, int cameraIndex);

    /// <summary>
    /// Sets the camera to use the specified <see cref="CameraTargetMode">CameraTargetMode</see>.
    /// </summary>
    /// <param name="targetMode">The camera target mode.</param>
    /// <param name="cameraGroup">The camera group.</param>
    /// <param name="cameraIndex">The camera index.</param>
    /// <remarks>See irsdk_BroadcastCamSwitchPos and irsdk_csMode in the iRacing SDK.</remarks>
    void TargetCarByPosition(CameraTargetMode targetMode, int cameraGroup, int cameraIndex);
}
