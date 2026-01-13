namespace SharpRacer.Commands;

public interface ISimulatorCommandSinkObserver
{
    void OnCommandSent<TCommand>(TCommand command)
        where TCommand : ISimulatorCommand<TCommand>;

    void OnCommandMessageSent(CommandMessage message);
}
