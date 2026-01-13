using System.Runtime.Versioning;
using SharpRacer.Commands.Interop;
using Windows.Win32;

namespace SharpRacer.Commands.Textures;

/// <summary>
/// Provides methods for sending texture commands to the simulator.
/// </summary>
public sealed class TexturesCommandClient : CommandClientBase, ITexturesCommandClient
{
    /// <summary>
    /// Initializes a new <see cref="TexturesCommandClient"/> instance.
    /// </summary>
    [SupportedOSPlatform(PInvoke.MinOSPlatform)]
    public TexturesCommandClient()
        : this(SimulatorCommandSink.Instance)
    {

    }

    /// <summary>
    /// Initializes a new <see cref="TexturesCommandClient"/> instance with the specified command sink.
    /// </summary>
    /// <param name="commandSink">The <see cref="ISimulatorCommandSink"/> to use for sending commands to the simulator.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="commandSink"/> is <see langword="null"/>.
    /// </exception>
    public TexturesCommandClient(ISimulatorCommandSink commandSink)
        : base(commandSink)
    {
    }

    /// <inheritdoc />
    public void ReloadAll()
    {
        Send(new ReloadCarTextureCommand());
    }

    /// <inheritdoc />
    public void ReloadCarAtIndex(int carIndex)
    {
        Send(new ReloadCarTextureCommand(carIndex));
    }
}
