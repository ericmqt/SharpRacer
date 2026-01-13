namespace SharpRacer;

/// <summary>
/// Defines flags that describe the state of the camera tool.
/// </summary>
/// <remarks>See irsdk_CameraState in the iRacing SDK.</remarks>
[Flags]
public enum CameraState : ushort
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
    /// <remarks>
    /// Represents the irsdk_IsSessionScreen value in the iRacing SDK.
    /// </remarks>
    IsSessionScreenActive = 0x0001,

    /// <summary>
    /// The scenic camera is active (no focus car).
    /// </summary>
    /// <remarks>
    /// Represents the irsdk_IsScenicActive value in the iRacing SDK.
    /// </remarks>
    IsScenicCameraActive = 0x0002,

    /// <summary>
    /// Indicates the camera tool is active.
    /// </summary>
    /// <remarks>
    /// Represents the irsdk_CamToolActive value in the iRacing SDK.
    /// </remarks>
    IsCameraToolActive = 0x0004,

    /// <summary>
    /// Simulator UI is hidden.
    /// </summary>
    /// <remarks>
    /// Represents the irsdk_UIHidden value in the iRacing SDK.
    /// </remarks>
    IsUIHidden = 0x0008,

    /// <summary>
    /// Use automatic shot selection.
    /// </summary>
    UseAutoShotSelection = 0x0010,

    /// <summary>
    /// Use temporary edits.
    /// </summary>
    UseTemporaryEdits = 0x0020,

    /// <summary>
    /// Use key acceleration.
    /// </summary>
    UseKeyAcceleration = 0x0040,

    /// <summary>
    /// Use key 10x acceleration.
    /// </summary>
    UseKey10xAcceleration = 0x0080,

    /// <summary>
    /// Use mouse aiming.
    /// </summary>
    UseMouseAimMode = 0x0100
}
