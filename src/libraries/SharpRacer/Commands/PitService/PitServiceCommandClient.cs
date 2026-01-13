using System.Runtime.Versioning;
using SharpRacer.Commands.Interop;
using Windows.Win32;

namespace SharpRacer.Commands.PitService;

/// <summary>
/// Provides methods for requesting or resetting various pit services in the simulator.
/// </summary>
public sealed class PitServiceCommandClient : CommandClientBase, IPitServiceCommandClient
{
    /// <summary>
    /// Initializes a new <see cref="PitServiceCommandClient"/> instance.
    /// </summary>
    [SupportedOSPlatform(PInvoke.MinOSPlatform)]
    public PitServiceCommandClient()
        : this(SimulatorCommandSink.Instance)
    {

    }

    /// <summary>
    /// Initializes a new <see cref="PitServiceCommandClient"/> with the specified command sink.
    /// </summary>
    /// <param name="commandSink">The <see cref="ISimulatorCommandSink"/> to use for sending commands to the simulator.</param>
    /// <exception cref="ArgumentNullException"><paramref name="commandSink"/> is <see langword="null"/>.</exception>
    public PitServiceCommandClient(ISimulatorCommandSink commandSink)
        : base(commandSink)
    {

    }

    /// <inheritdoc />
    public void Clear(PitServiceResetType pitService)
    {
        Send(new ResetPitServiceCommand(pitService));
    }

    /// <inheritdoc />
    public void ClearAll()
    {
        Send(new ResetPitServiceCommand(PitServiceResetType.All));
    }

    /// <inheritdoc />
    public void RequestFastRepair()
    {
        Send(new UseFastRepairCommand());
    }

    /// <inheritdoc />
    public void RequestFuel()
    {
        Send(new AddFuelCommand(fuelQuantityLiters: 0));
    }

    /// <inheritdoc />
    public void RequestFuel(int liters)
    {
        Send(new AddFuelCommand(liters));
    }

    /// <inheritdoc />
    public void RequestTireChange(TireChangeTarget tire)
    {
        Send(new ChangeTireCommand(tire, pressureKPa: 0));
    }

    /// <inheritdoc />
    public void RequestTireChange(TireChangeTarget tire, int pressureKPa)
    {
        Send(new ChangeTireCommand(tire, pressureKPa));
    }

    /// <inheritdoc />
    public void RequestWindscreenTearOff()
    {
        Send(new UseWindscreenTearOffCommand());
    }
}
