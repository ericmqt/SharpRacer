namespace SharpRacer.Commands;

/// <summary>
/// Defines window message IDs used to send commands to the simulator.
/// </summary>
/// <remarks>See irsdk_BroadcastMsg in the iRacing SDK.</remarks>
public enum SimulatorCommandId : ushort
{
    /// <summary>
    /// The CameraSwitchPosition command.
    /// </summary>
    /// <remarks>
    /// Represents the irsdk_BroadcastCamSwitchPos value in the iRacing SDK.
    /// </remarks>
    CameraSwitchPosition = 0,

    /// <summary>
    /// The CameraSwitchNumber command.
    /// </summary>
    /// <remarks>
    /// Represents the irsdk_BroadcastCamSwitchNum value in the iRacing SDK.
    /// </remarks>
    CameraSwitchNumber = 1,

    /// <summary>
    /// The CameraSetState command.
    /// </summary>
    /// <remarks>
    /// Represents the irsdk_BroadcastCamSetState value in the iRacing SDK.
    /// </remarks>
    CameraSetState = 2,

    /// <summary>
    /// The ReplaySetPlaySpeed command.
    /// </summary>
    /// <remarks>
    /// Represents the irsdk_BroadcastReplaySetPlaySpeed value in the iRacing SDK.
    /// </remarks>
    ReplaySetPlaySpeed = 3,

    /// <summary>
    /// The ReplaySetPlayPosition command.
    /// </summary>
    /// <remarks>
    /// Represents the irskd_BroadcastReplaySetPlayPosition value in the iRacing SDK.
    /// </remarks>
    ReplaySetPlayPosition = 4,

    /// <summary>
    /// The ReplaySearch command.
    /// </summary>
    /// <remarks>
    /// Represents the irsdk_BroadcastReplaySearch value in the iRacing SDK.
    /// </remarks>
    ReplaySearch = 5,

    /// <summary>
    /// The ReplaySetState command.
    /// </summary>
    /// <remarks>
    /// Represents the irsdk_BroadcastReplaySetState value in the iRacing SDK.
    /// </remarks>
    ReplaySetState = 6,

    /// <summary>
    /// The ReloadTextures command.
    /// </summary>
    /// <remarks>
    /// Represents the irsdk_BroadcastReloadTextures value in the iRacing SDK.
    /// </remarks>
    ReloadTextures = 7,

    /// <summary>
    /// The ChatCommand command.
    /// </summary>
    /// <remarks>
    /// Represents the irsdk_BroadcastChatComand value in the iRacing SDK.
    /// </remarks>
    ChatCommand = 8,

    /// <summary>
    /// The PitCommand command.
    /// </summary>
    /// <remarks>
    /// Represents the irsdk_BroadcastPitCommand value in the iRacing SDK.
    /// </remarks>
    PitCommand = 9,

    /// <summary>
    /// The TelemetryCommand command.
    /// </summary>
    /// <remarks>
    /// Represents the irsdk_BroadcastTelemCommand value in the iRacing SDK.
    /// </remarks>
    TelemetryCommand = 10,

    /// <summary>
    /// The ForceFeedbackCommand command.
    /// </summary>
    /// <remarks>
    /// Represents the irsdk_BroadcastFFBCommand value in the iRacing SDK.
    /// </remarks>
    ForceFeedbackCommand = 11,

    /// <summary>
    /// The ReplaySearchSessionTime command.
    /// </summary>
    /// <remarks>
    /// Represents the irsdk_BroadcastReplaySearchSessionTime value in the iRacing SDK.
    /// </remarks>
    ReplaySearchSessionTime = 12,

    /// <summary>
    /// The VideoCapture command.
    /// </summary>
    /// <remarks>
    /// Represents the irsdk_BroadcastVideoCapture value in the iRacing SDK.
    /// </remarks>
    VideoCapture = 13,

    /// <summary>
    /// Unused.
    /// </summary>
    /// <remarks>
    /// Represents the irsdk_BroadcastLast value in the iRacing SDK.
    /// </remarks>
    Unused = 14
}
