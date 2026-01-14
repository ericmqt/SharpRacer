using Moq;
using SharpRacer.Commands.Interop;
using SharpRacer.Commands.PitService;

namespace SharpRacer.Commands;

public class CommandClientBaseTests
{
    [Fact]
    public void Ctor_MessageSinkTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var commandSinkMock = mocks.Create<ISimulatorCommandSink>();
        var client = new ConcreteTestCommandClient(commandSinkMock.Object);

        Assert.Equal(commandSinkMock.Object, client.CommandSink);
    }

    [Fact]
    public void Ctor_ThrowIfMessageSinkIsNullTest()
    {
        Assert.Throws<ArgumentNullException>(() => new ConcreteTestCommandClient(null!));
    }

    [Fact]
    public void Send_TCommand_Test()
    {
        var testCommand = new AddFuelCommand(fuelQuantityLiters: 4);
        var testMessage = (CommandMessage)testCommand;
        bool commandSent = false;

        void onSendMessage(CommandMessage message)
        {
            Assert.Equal(testMessage, message);
            commandSent = true;
        }

        var commandSink = new TestCommandSink(onSendMessage);
        var client = new ConcreteTestCommandClient(commandSink);

        client.Send(testCommand);

        Assert.True(commandSent);
    }

    private class ConcreteTestCommandClient : CommandClientBase
    {
        public ConcreteTestCommandClient(ISimulatorCommandSink commandSink)
            : base(commandSink)
        {
        }
    }

    private class TestCommandSink : ISimulatorCommandSink
    {
        private readonly Action<CommandMessage> _onSendMessage;

        public TestCommandSink(Action<CommandMessage> onSendMessage)
        {
            _onSendMessage = onSendMessage ?? throw new ArgumentNullException(nameof(onSendMessage));
        }

        public void Send(CommandMessage commandMessage)
        {
            _onSendMessage(commandMessage);
        }

        public void Send<TCommand>(TCommand command)
            where TCommand : ISimulatorCommand<TCommand>
        {
            Send(command.ToCommandMessage());
        }
    }
}
