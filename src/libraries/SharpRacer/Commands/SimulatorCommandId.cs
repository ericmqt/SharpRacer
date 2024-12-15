namespace SharpRacer.Commands;

/// <summary>
/// Defines window message IDs used to send commands to the simulator.
/// </summary>
/// <remarks>See irsdk_BroadcastMsg in the iRacing SDK.</remarks>
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
