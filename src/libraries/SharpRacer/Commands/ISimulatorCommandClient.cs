using SharpRacer.Commands.Camera;
using SharpRacer.Commands.PitService;

namespace SharpRacer.Commands;

/// <summary>
/// Defines methods for sending commands to the simulator.
/// </summary>
public interface ISimulatorCommandClient
{
    /// <summary>
    /// Captures a screenshot and saves it to disk.
    /// </summary>
    void CaptureScreenshot();

    /// <summary>
    /// Resets the specified pit services.
    /// </summary>
    /// <param name="pitService"></param>
    void ClearPitService(PitServiceResetType pitService);

    /// <summary>
    /// Resets all pit service options.
    /// </summary>
    void ClearAllPitServices();

    /// <summary>
    /// Hides the on-screen video capture timer.
    /// </summary>
    void HideVideoCaptureTimer();

    /// <summary>
    /// Reloads custom textures for all cars.
    /// </summary>
    void ReloadAllCarTextures();

    /// <summary>
    /// Reloads the custom texture for a car by index.
    /// </summary>
    /// <param name="carIndex">The index of the car whose custom texture will be reloaded.</param>
    void ReloadCarTexture(int carIndex);

    /// <summary>
    /// Request a fast repair during the next pit service, if one is available.
    /// </summary>
    void RequestFastRepairPitService();

    /// <summary>
    /// Request fuel during the next pit service without modifying the fuel quantity.
    /// </summary>
    void RequestFuelPitService();

    /// <summary>
    /// Request the specified amount of fuel during the next pit service.
    /// </summary>
    /// <param name="liters">The amount of fuel to add, in liters.</param>
    void RequestFuelPitService(int liters);

    /// <summary>
    /// Request a tire change for the specified tire during the next pit service without modifying the tire pressure of the new tire.
    /// </summary>
    /// <param name="tire"></param>
    void RequestTireChangePitService(TireChangeTarget tire);

    /// <summary>
    /// Request a tire change for the specified tire during the next pit service, inflated to the specified tire pressure.
    /// </summary>
    /// <param name="tire">The tire to change.</param>
    /// <param name="pressureKPa">The tire pressure in kilopascals (kPa).</param>
    void RequestTireChangePitService(TireChangeTarget tire, int pressureKPa);

    /// <summary>
    /// Request a windscreen tear-off, if one is available, during the next pit service.
    /// </summary>
    void RequestWindscreenTearOffPitService();

    /// <summary>
    /// Write the current telemetry file to disk and begin a new telemetry recording.
    /// </summary>
    void RestartTelemetryRecording();

    /// <summary>
    /// Sets the camera state.
    /// </summary>
    /// <param name="cameraState">The camera state.</param>
    /// <remarks>See irsdk_BroadcastCamSetState in the iRacing SDK.</remarks>
    void SetCameraState(CameraState cameraState);

    /// <summary>
    /// Sets the maximum force when mapping steering wheel torque to direct input units.
    /// </summary>
    /// <param name="forceNm">The maximum force in Newton-meters.</param>
    void SetMaxSteeringWheelForce(float forceNm);

    /// <summary>
    /// Sets the camera to target the car in the specified race position.
    /// </summary>
    /// <param name="position">The race position of the car to target.</param>
    /// <param name="cameraGroup">The camera group.</param>
    /// <param name="cameraIndex">The camera index.</param>
    /// <remarks>See irsdk_BroadcastCamSwitchPos in the iRacing SDK.</remarks>
    void SetCameraTargetToCarPosition(int position, int cameraGroup, int cameraIndex);

    /// <summary>
    /// Sets the camera to use the specified <see cref="CameraTargetMode">CameraTargetMode</see>.
    /// </summary>
    /// <param name="targetMode">The camera target mode.</param>
    /// <param name="cameraGroup">The camera group.</param>
    /// <param name="cameraIndex">The camera index.</param>
    /// <remarks>See irsdk_BroadcastCamSwitchPos and irsdk_csMode in the iRacing SDK.</remarks>
    void SetCameraTargetToCarPosition(CameraTargetMode targetMode, int cameraGroup, int cameraIndex);

    /// <summary>
    /// Sets the camera to target the car driven by the specified driver.
    /// </summary>
    /// <param name="driverNumber"></param>
    /// <param name="cameraGroup">The camera group.</param>
    /// <param name="cameraIndex">The camera index.</param>
    /// <remarks>See irsdk_BroadcastCamSwitchNum in the iRacing SDK.</remarks>
    void SetCameraTargetToDriver(int driverNumber, int cameraGroup, int cameraIndex);

    /// <summary>
    /// Sets the camera to use the specified <see cref="CameraTargetMode">CameraTargetMode</see>.
    /// </summary>
    /// <param name="targetMode">The camera target mode.</param>
    /// <param name="cameraGroup">The camera group.</param>
    /// <param name="cameraIndex">The camera index.</param>
    /// <remarks>See irsdk_BroadcastCamSwitchNum and irsdk_csMode in the iRacing SDK.</remarks>
    void SetCameraTargetToDriver(CameraTargetMode targetMode, int cameraGroup, int cameraIndex);

    /// <summary>
    /// Show the on-screen video capture timer.
    /// </summary>
    void ShowVideoCaptureTimer();

    /// <summary>
    /// Start recording telemetry to disk.
    /// </summary>
    void StartTelemetryRecording();

    /// <summary>
    /// Start video capture.
    /// </summary>
    void StartVideoCapture();

    /// <summary>
    /// Stop telemetry recording and write the file to disk.
    /// </summary>
    void StopTelemetryRecording();

    /// <summary>
    /// Stops video capture.
    /// </summary>
    void StopVideoCapture();

    /// <summary>
    /// Toggles video capture on and off.
    /// </summary>
    void ToggleVideoCapture();
}
