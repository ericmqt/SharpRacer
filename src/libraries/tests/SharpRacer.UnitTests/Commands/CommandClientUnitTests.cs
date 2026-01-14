using Moq;
using SharpRacer.Interop;

namespace SharpRacer.Commands;

public abstract class CommandClientUnitTests
{
    public CommandClientUnitTests()
        : this(MockBehavior.Strict)
    {

    }

    public CommandClientUnitTests(MockBehavior mockBehavior)
    {
        Mocks = new MockRepository(mockBehavior);

        CommandSinkObserverMock = Mocks.Create<ISimulatorCommandSinkObserver>();
        NotifyMessageSinkObserverMock = Mocks.Create<ISimulatorNotifyMessageSinkObserver>();

        MessageSink = new ObservableMessageSink(NotifyMessageSinkObserverMock.Object);
        CommandSink = new ObservableCommandSink(MessageSink, CommandSinkObserverMock.Object);
    }

    public ObservableCommandSink CommandSink { get; }
    public Mock<ISimulatorCommandSinkObserver> CommandSinkObserverMock { get; }
    public ObservableMessageSink MessageSink { get; }
    public Mock<ISimulatorNotifyMessageSinkObserver> NotifyMessageSinkObserverMock { get; }
    public MockRepository Mocks { get; }

    public void ConfigureExpectedCommand<T>(T command, Times times)
        where T : ISimulatorCommand<T>
    {
        ConfigureExpectedCommand(command, command.ToCommandMessage(), times);
    }

    public void ConfigureExpectedCommand<T>(T command, CommandMessage commandMessage, Times times)
        where T : ISimulatorCommand<T>
    {
        ConfigureExpectedCommand(command, commandMessage, commandMessage.ToNotifyMessage(), times);
    }

    public void ConfigureExpectedCommand<T>(
        T command,
        CommandMessage commandMessage,
        SimulatorNotifyMessageData notifyMessageData,
        Times times)
        where T : ISimulatorCommand<T>
    {
        CommandSinkObserverMock.Setup(x => x.OnCommandSent(It.IsAny<T>()))
            .Callback<T>(cmd => Assert.Equal(command, cmd))
            .Verifiable(Times.Once());

        CommandSinkObserverMock.Setup(x => x.OnCommandMessageSent(It.IsAny<CommandMessage>()))
            .Callback<CommandMessage>(msg => Assert.Equal(commandMessage, msg))
            .Verifiable(Times.Once());

        NotifyMessageSinkObserverMock.Setup(x => x.OnMessageSent(It.IsAny<SimulatorNotifyMessageData>()))
            .Callback<SimulatorNotifyMessageData>(msg => Assert.Equal(notifyMessageData, msg))
            .Verifiable(Times.Once());
    }
}
