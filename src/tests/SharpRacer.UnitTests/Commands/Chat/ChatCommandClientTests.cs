using Moq;
using SharpRacer.Commands.Interop;

namespace SharpRacer.Commands.Chat;

public class ChatCommandClientTests : CommandClientUnitTests
{
    [Fact]
    public void Ctor_CommandSinkTest()
    {
        var commandSinkMock = Mocks.Create<ISimulatorCommandSink>();

        var client = new ChatCommandClient(commandSinkMock.Object);

        Assert.Equal(commandSinkMock.Object, client.CommandSink);
    }

    [Fact]
    public void Ctor_ThrowIfCommandSinkIsNullTest()
    {
        Assert.Throws<ArgumentNullException>(() => new ChatCommandClient(null!));
    }

    [Fact]
    public void CloseChat_Test()
    {
        var command = new CloseChatCommand();
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new ChatCommandClient(CommandSink);
        client.CloseChat();

        Mocks.Verify();
    }

    [Fact]
    public void Macro_Test()
    {
        ushort macroId = 12;
        var command = new MacroCommand(macroId);
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new ChatCommandClient(CommandSink);
        client.Macro(macroId);

        Mocks.Verify();
    }

    [Fact]
    public void OpenChat_Test()
    {
        var command = new OpenChatCommand();
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new ChatCommandClient(CommandSink);
        client.OpenChat();

        Mocks.Verify();
    }

    [Fact]
    public void ReplyToLastPrivateMessage_Test()
    {
        var command = new ReplyToLastPrivateMessageCommand();
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new ChatCommandClient(CommandSink);
        client.ReplyToLastPrivateMessage();

        Mocks.Verify();

    }
}
