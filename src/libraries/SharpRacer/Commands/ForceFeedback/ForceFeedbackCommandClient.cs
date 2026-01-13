using System.Runtime.Versioning;
using SharpRacer.Commands.Interop;
using Windows.Win32;

namespace SharpRacer.Commands.ForceFeedback;

/// <summary>
/// Provides methods for sending force-feedback commands to the simulator.
/// </summary>
public class ForceFeedbackCommandClient : CommandClientBase, IForceFeedbackCommandClient
{
    /// <summary>
    /// Initializes a new <see cref="ForceFeedbackCommandClient"/> instance.
    /// </summary>
    [SupportedOSPlatform(PInvoke.MinOSPlatform)]
    public ForceFeedbackCommandClient()
        : this(SimulatorCommandSink.Instance)
    {

    }

    /// <summary>
    /// Initializes a new <see cref="ForceFeedbackCommandClient"/> instance with the specified command sink.
    /// </summary>
    /// <param name="commandSink">The <see cref="ISimulatorCommandSink"/> to use for sending commands to the simulator.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="commandSink"/> is <see langword="null"/>.
    /// </exception>
    public ForceFeedbackCommandClient(ISimulatorCommandSink commandSink)
        : base(commandSink)
    {

    }

    /// <inheritdoc />
    public void SetMaxForce(float forceNm)
    {
        Send(new SetMaxForceCommand(forceNm));
    }
}
