namespace SharpRacer.Commands;
public enum VideoCaptureCommandType : ushort
{
    CaptureScreenshot = 0,
    BeginVideoCapture = 1,
    EndVideoCapture = 2,
    ToggleVideoCapture = 3,
    ShowVideoTimer = 4,
    HideVideoTimer = 5
}
