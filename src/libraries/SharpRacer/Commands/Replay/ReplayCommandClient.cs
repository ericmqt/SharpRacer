using System.Runtime.Versioning;
using SharpRacer.Commands.Interop;
using Windows.Win32;

namespace SharpRacer.Commands.Replay;

/// <summary>
/// Provides methods for sending replay commands to the simulator.
/// </summary>
public class ReplayCommandClient : CommandClientBase, IReplayCommandClient
{
    /// <summary>
    /// Initializes a new <see cref="ReplayCommandClient"/> instance.
    /// </summary>
    [SupportedOSPlatform(PInvoke.MinOSPlatform)]
    public ReplayCommandClient()
        : this(SimulatorCommandSink.Instance)
    {

    }

    /// <summary>
    /// Initializes a new <see cref="ReplayCommandClient"/> instance with the specified command sink.
    /// </summary>
    /// <param name="commandSink">The <see cref="ISimulatorCommandSink"/> to use for sending commands to the simulator.</param>
    /// <exception cref="ArgumentNullException"><paramref name="commandSink"/> is <see langword="null"/>.</exception>
    public ReplayCommandClient(ISimulatorCommandSink commandSink)
        : base(commandSink)
    {

    }

    /// <inheritdoc />
    public void EraseTape()
    {
        Send(new EraseReplayTapeCommand());
    }

    /// <inheritdoc />
    public void Pause()
    {
        Send(new SetPlaySpeedCommand(playSpeed: 0, isSlowMotion: false));
    }

    /// <inheritdoc />
    public void Play()
    {
        Send(new SetPlaySpeedCommand(playSpeed: 1, isSlowMotion: false));
    }

    /// <inheritdoc />
    public void Search(ReplaySearchMode searchMode)
    {
        Send(new SearchCommand(searchMode));
    }

    /// <inheritdoc />
    public void SearchSessionTime(int sessionNumber, int sessionTimeMs)
    {
        Send(new SearchSessionTimeCommand((ushort)sessionNumber, sessionTimeMs));
    }

    /// <inheritdoc />
    public void SeekFrame(int frame, ReplaySeekOrigin seekOrigin)
    {
        Send(new SeekFrameCommand(frame, seekOrigin));
    }

    /// <inheritdoc />
    public void SetPlaySpeed(int playSpeed, bool isSlowMotion)
    {
        Send(new SetPlaySpeedCommand((ushort)playSpeed, isSlowMotion));
    }
}
