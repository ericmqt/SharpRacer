using System.Runtime.Versioning;
using SharpRacer.Commands.Interop;
using Windows.Win32;

namespace SharpRacer.Commands.Telemetry;

/// <summary>
/// Provides methods for sending telemetry recording commands to the simulator.
/// </summary>
public class TelemetryCommandClient : CommandClientBase, ITelemetryCommandClient
{
    /// <summary>
    /// Initializes a new <see cref="TelemetryCommandClient"/> instance.
    /// </summary>
    [SupportedOSPlatform(PInvoke.MinOSPlatform)]
    public TelemetryCommandClient()
        : this(SimulatorCommandSink.Instance)
    {

    }

    /// <summary>
    /// Initializes a new <see cref="TelemetryCommandClient"/> instance with the specified command sink.
    /// </summary>
    /// <param name="commandSink">The <see cref="ISimulatorCommandSink"/> to use for sending commands to the simulator.</param>
    /// <exception cref="ArgumentNullException"><paramref name="commandSink"/> is <see langword="null"/>.</exception>
    public TelemetryCommandClient(ISimulatorCommandSink commandSink)
        : base(commandSink)
    {
    }

    /// <inheritdoc />
    public void Restart()
    {
        Send(new TelemetryCommand(TelemetryCommandType.Restart));
    }

    /// <inheritdoc />
    public void Start()
    {
        Send(new TelemetryCommand(TelemetryCommandType.Start));
    }

    /// <inheritdoc />
    public void Stop()
    {
        Send(new TelemetryCommand(TelemetryCommandType.Stop));
    }
}
