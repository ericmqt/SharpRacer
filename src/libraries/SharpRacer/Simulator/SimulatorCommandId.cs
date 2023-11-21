namespace SharpRacer.Simulator;

/// <summary>
/// Defines window message IDs used to send commands to the simulator.
/// </summary>
/// <remarks>See: irsdk_BroadcastMsg</remarks>
public enum SimulatorCommandId : ushort
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
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
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}