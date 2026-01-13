using SharpRacer.Commands.Interop;
using SharpRacer.Interop;

namespace SharpRacer.Commands;

public class ObservableCommandSink : ISimulatorCommandSink
{
    private readonly ISimulatorCommandSinkObserver _observer;
    private readonly ISimulatorNotifyMessageSink _messageSink;

    public ObservableCommandSink(ISimulatorNotifyMessageSink messageSink, ISimulatorCommandSinkObserver observer)
    {
        _messageSink = messageSink ?? throw new ArgumentNullException(nameof(messageSink));
        _observer = observer ?? throw new ArgumentNullException(nameof(observer));
    }

    public void Send(CommandMessage commandMessage)
    {
        _observer.OnCommandMessageSent(commandMessage);

        _messageSink.Send(commandMessage.ToNotifyMessage());
    }

    public void Send<TCommand>(TCommand command)
        where TCommand : ISimulatorCommand<TCommand>
    {
        _observer.OnCommandSent(command);

        Send(command.ToCommandMessage());
    }
}
