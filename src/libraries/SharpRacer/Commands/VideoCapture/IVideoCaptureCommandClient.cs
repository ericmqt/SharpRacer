namespace SharpRacer.Commands.VideoCapture;

/// <summary>
/// Defines methods for sending video capture commands to the simulator.
/// </summary>
public interface IVideoCaptureCommandClient
{
    /// <summary>
    /// Hide the video capture timer.
    /// </summary>
    void HideTimer();

    /// <summary>
    /// Save a screenshot to disk.
    /// </summary>
    void Screenshot();

    /// <summary>
    /// Show the video capture timer.
    /// </summary>
    void ShowTimer();

    /// <summary>
    /// Start video capture.
    /// </summary>
    void Start();

    /// <summary>
    /// Stop video capture.
    /// </summary>
    void Stop();

    /// <summary>
    /// Toggle video capture on and off.
    /// </summary>
    void ToggleVideoCapture();
}
