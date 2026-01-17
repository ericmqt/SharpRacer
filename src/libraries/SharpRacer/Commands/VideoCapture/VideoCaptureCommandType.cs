namespace SharpRacer.Commands.VideoCapture;

/// <summary>
/// Defines video capture command types.
/// </summary>
/// <remarks>
/// See irsdk_VideoCaptureMode in the iRacing SDK.
/// </remarks>
public enum VideoCaptureCommandType : ushort
{
    /// <summary>
    /// Capture a screenshot to disk.
    /// </summary>
    CaptureScreenshot = 0,

    /// <summary>
    /// Start capturing video.
    /// </summary>
    StartVideoCapture = 1,

    /// <summary>
    /// Stop capturing video.
    /// </summary>
    StopVideoCapture = 2,

    /// <summary>
    /// Toggle video capture on/off.
    /// </summary>
    ToggleVideoCapture = 3,

    /// <summary>
    /// Show video capture timer in the upper-left corner of the display.
    /// </summary>
    ShowVideoTimer = 4,

    /// <summary>
    /// Hide video capture timer.
    /// </summary>
    HideVideoTimer = 5
}
