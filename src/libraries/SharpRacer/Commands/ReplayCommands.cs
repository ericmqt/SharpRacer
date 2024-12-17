using System.Runtime.Versioning;
using SharpRacer.Interop;

namespace SharpRacer.Commands;

/// <summary>
/// Provides static methods for invoking replay commands.
/// </summary>
/// <remarks>
/// The simulator must be running for these commands to have any effect.
/// </remarks>
[SupportedOSPlatform("windows5.1.2600")]
public static class ReplayCommands
{
    /// <summary>
    /// Clears any data in the replay tape.
    /// </summary>
    public static void EraseTape()
    {
        BroadcastMessage.Send(SimulatorCommandId.ReplaySetState, 0);
    }

    /// <summary>
    /// Pause playback.
    /// </summary>
    public static void Pause()
    {
        SetPlaySpeed(0, false);
    }

    /// <summary>
    /// Begins or resumes playback at normal speed.
    /// </summary>
    public static void Play()
    {
        SetPlaySpeed(1, false);
    }

    /// <summary>
    /// Sets the playback position according to the result of a search.
    /// </summary>
    /// <param name="searchMode"></param>
    public static void Search(ReplaySearchMode searchMode)
    {
        BroadcastMessage.Send(SimulatorCommandId.ReplaySearch, (ushort)searchMode);
    }

    /// <summary>
    /// Sets the playback position according to the result of a search for the specified session and session time.
    /// </summary>
    /// <param name="sessionNumber">The session number.</param>
    public static void SearchSessionTime(int sessionNumber, int sessionTimeMs)
    {
        BroadcastMessage.Send(SimulatorCommandId.ReplaySearchSessionTime, sessionNumber, sessionTimeMs);
    }

    /// <summary>
    /// Sets the playback position relative to the specified origin point.
    /// </summary>
    /// <param name="frame">The number of frames by which the playback position will be advanced.</param>
    /// <param name="origin">The reference point used to obtain the desired playback position.</param>
    public static void SeekFrame(int frame, ReplaySeekOrigin origin)
    {
        // TODO: Verify frame should be negative with ReplaySeekOrigin.End
        BroadcastMessage.Send(SimulatorCommandId.ReplaySetPlayPosition, (ushort)origin, frame);
    }

    /// <summary>
    /// Sets the playback speed.
    /// </summary>
    /// <param name="playSpeed">The playback rate.</param>
    /// <param name="isSlowMotion">
    /// If <see langword="true"/>, then playback speed is equal to 1/<paramref name="playSpeed"/>.
    /// </param>
    public static void SetPlaySpeed(int playSpeed, bool isSlowMotion)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(playSpeed, 0);

        BroadcastMessage.Send(SimulatorCommandId.ReplaySetPlaySpeed, playSpeed, isSlowMotion ? 1 : 0);
    }
}
