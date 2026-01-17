using SharpRacer.Commands.Chat;
using SharpRacer.Commands.PitService;
using SharpRacer.Commands.Telemetry;
using SharpRacer.Commands.VideoCapture;

namespace SharpRacer.Commands;

/// <summary>
/// Defines the primary identifiers used by the simulator to discriminate between various types of commands.
/// </summary>
/// <remarks>See irsdk_BroadcastMsg in the iRacing SDK.</remarks>
public enum SimulatorCommandId : ushort
{
    /// <summary>
    /// Directs a camera to target a specific car by race position or to change the targeting mode.
    /// </summary>
    /// <remarks>
    /// Represents the irsdk_BroadcastCamSwitchPos value in the iRacing SDK.
    /// </remarks>
    CameraTargetRacePosition = 0,

    /// <summary>
    /// Directs a camera to target a specific driver or to change the targeting mode.
    /// </summary>
    /// <remarks>
    /// Represents the irsdk_BroadcastCamSwitchNum value in the iRacing SDK.
    /// </remarks>
    CameraTargetDriver = 1,

    /// <summary>
    /// Sets the state of the camera tool.
    /// </summary>
    /// <remarks>
    /// Represents the irsdk_BroadcastCamSetState value in the iRacing SDK.
    /// </remarks>
    CameraSetState = 2,

    /// <summary>
    /// Sets the replay playback speed.
    /// </summary>
    /// <remarks>
    /// Represents the irsdk_BroadcastReplaySetPlaySpeed value in the iRacing SDK.
    /// </remarks>
    ReplaySetPlaySpeed = 3,

    /// <summary>
    /// Sets the replay playback position.
    /// </summary>
    /// <remarks>
    /// Represents the irskd_BroadcastReplaySetPlayPosition value in the iRacing SDK.
    /// </remarks>
    ReplaySetPlayPosition = 4,

    /// <summary>
    /// Searches the replay tape for a particular event.
    /// </summary>
    /// <remarks>
    /// Represents the irsdk_BroadcastReplaySearch value in the iRacing SDK.
    /// </remarks>
    ReplaySearch = 5,

    /// <summary>
    /// Sets the state of the replay tool. Currently only used to erase the replay tape.
    /// </summary>
    /// <remarks>
    /// Represents the irsdk_BroadcastReplaySetState value in the iRacing SDK.
    /// </remarks>
    ReplaySetState = 6,

    /// <summary>
    /// Reloads custom car textures.
    /// </summary>
    /// <remarks>
    /// Represents the irsdk_BroadcastReloadTextures value in the iRacing SDK.
    /// </remarks>
    CarTextureReload = 7,

    /// <summary>
    /// Identifies a chat command. See <see cref="ChatCommandType"/> for a listing of subcommands.
    /// </summary>
    /// <remarks>
    /// Represents the irsdk_BroadcastChatComand value in the iRacing SDK.
    /// </remarks>
    Chat = 8,

    /// <summary>
    /// Identifies a pit service command. See <see cref="PitCommandType"/> for a listing of subcommands.
    /// </summary>
    /// <remarks>
    /// Represents the irsdk_BroadcastPitCommand value in the iRacing SDK.
    /// </remarks>
    PitService = 9,

    /// <summary>
    /// Identifies a telemetry recording command. See <see cref="TelemetryCommandType"/> for a listing of subcommands.
    /// </summary>
    /// <remarks>
    /// Represents the irsdk_BroadcastTelemCommand value in the iRacing SDK.
    /// </remarks>
    Telemetry = 10,

    /// <summary>
    /// Identifies a force-feedback command. Currently only used to set the maximum force generated when mapping steering wheel torque to
    /// direct-drive steering wheels.
    /// </summary>
    /// <remarks>
    /// Represents the irsdk_BroadcastFFBCommand value in the iRacing SDK.
    /// </remarks>
    ForceFeedback = 11,

    /// <summary>
    /// Search the replay tape for a timestamp within a specified session.
    /// </summary>
    /// <remarks>
    /// Represents the irsdk_BroadcastReplaySearchSessionTime value in the iRacing SDK.
    /// </remarks>
    ReplaySearchSessionTime = 12,

    /// <summary>
    /// Identifies a video capture command. See <see cref="VideoCaptureCommandType"/> for a listing of subcommands.
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
