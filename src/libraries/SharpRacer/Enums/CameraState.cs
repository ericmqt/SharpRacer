namespace SharpRacer;

/// <summary>
/// Defines flags that describe the state of the camera tool.
/// </summary>
/// <remarks>See irsdk_CameraState in the iRacing SDK.</remarks>
[Flags]
public enum CameraState : uint
{
    /// <summary>
    /// Undefined camera state.
    /// </summary>
    /// <remarks>
    /// This value is not defined in the irsdk_CameraState enumeration in the iRacing SDK.
    /// </remarks>
    None = 0,

    /// <summary>
    /// Indicates the session screen is active. The camera tool can only be activated in this state.
    /// </summary>
    IsSessionScreenActive = 0x0001,

    /// <summary>
    /// The scenic camera is active (no focus car).
    /// </summary>
    IsScenicCameraActive = 0x0002,

    /// <summary>
    /// Indicates the camera tool is active.
    /// </summary>
    IsCameraToolActive = 0x0004,

    /// <summary>
    /// Simulator UI is hidden.
    /// </summary>
    IsUIHidden = 0x0008,

    UseAutoShotSelection = 0x0010,
    UseTemporaryEdits = 0x0020,
    UseKeyAcceleration = 0x0040,
    UseKey10xAcceleration = 0x0080,
    UseMouseAimMode = 0x0100
}
