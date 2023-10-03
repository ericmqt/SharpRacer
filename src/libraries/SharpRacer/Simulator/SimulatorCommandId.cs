namespace SharpRacer.Simulator;
public enum SimulatorCommandId : ushort
{
    CameraSwitchPosition = 0,
    CameraSwitchNumber = 1,
    CameraSetState = 2,
    ReplaySetPlaySpeed = 3,
    ReplaySetPlayPosition = 4,
    ReplaySearch = 5,
    ReplaySetState = 6,
    ReloadTextures = 7,
    ChatCommand = 8,
    PitCommand = 9,
    TelemetryCommand = 10,
    ForceFeedbackCommand = 11,
    ReplaySearchSessionTime = 12,
    VideoCapture = 13,
    Unused = 14
}