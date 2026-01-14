using Moq;
using SharpRacer.Commands.Interop;

namespace SharpRacer.Commands.Textures;

public class TexturesCommandClientTests : CommandClientUnitTests
{
    [Fact]
    public void Ctor_CommandSinkTest()
    {
        var commandSinkMock = Mocks.Create<ISimulatorCommandSink>();

        var client = new TexturesCommandClient(commandSinkMock.Object);

        Assert.Equal(commandSinkMock.Object, client.CommandSink);
    }

    [Fact]
    public void Ctor_ThrowIfCommandSinkIsNullTest()
    {
        Assert.Throws<ArgumentNullException>(() => new TexturesCommandClient(null!));
    }

    [Fact]
    public void Reload_Test()
    {
        const ushort carIndex = 32;

        var command = new ReloadCarTextureCommand(carIndex);
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new TexturesCommandClient(CommandSink);
        client.ReloadCarAtIndex(carIndex);

        Mocks.Verify();
    }

    [Fact]
    public void ReloadAll_Test()
    {
        var command = new ReloadCarTextureCommand();
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new TexturesCommandClient(CommandSink);
        client.ReloadAll();

        Mocks.Verify();
    }
}
