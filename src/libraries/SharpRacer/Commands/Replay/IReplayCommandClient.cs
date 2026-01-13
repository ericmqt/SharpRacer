namespace SharpRacer.Commands.Replay;

/// <summary>
/// Defines methods for sending replay commands to the simulator.
/// </summary>
public interface IReplayCommandClient
{
    /// <summary>
    /// Clears any data in the replay tape.
    /// </summary>
    void EraseTape();

    /// <summary>
    /// Pause playback.
    /// </summary>
    void Pause();

    /// <summary>
    /// Begins or resumes playback at normal speed.
    /// </summary>
    void Play();

    /// <summary>
    /// Sets the playback position according to the result of a search.
    /// </summary>
    /// <param name="searchMode">The replay tape event to search for.</param>
    void Search(ReplaySearchMode searchMode);

    /// <summary>
    /// Sets the playback position according to the result of a search for the specified session and session time.
    /// </summary>
    /// <param name="sessionNumber">The session number.</param>
    /// <param name="sessionTimeMs">The elapsed time within the specified session.</param>
    /// <remarks>
    /// Per the iRacing SDK, this command performs a search instead of a direct jump so the operation "may take a while".
    /// </remarks>
    void SearchSessionTime(int sessionNumber, int sessionTimeMs);

    /// <summary>
    /// Sets the playback position relative to the specified origin point.
    /// </summary>
    /// <param name="frame">The number of frames by which the playback position will be advanced.</param>
    /// <param name="seekOrigin">The reference point used to obtain the desired playback position.</param>
    void SeekFrame(int frame, ReplaySeekOrigin seekOrigin);

    /// <summary>
    /// Sets the playback speed.
    /// </summary>
    /// <param name="playSpeed">The playback rate.</param>
    /// <param name="isSlowMotion">
    /// If <see langword="true"/>, then playback speed is equal to 1/<paramref name="playSpeed"/>.
    /// </param>
    void SetPlaySpeed(int playSpeed, bool isSlowMotion);
}
