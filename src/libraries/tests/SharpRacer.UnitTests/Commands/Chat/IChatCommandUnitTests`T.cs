namespace SharpRacer.Commands.Chat;

public interface IChatCommandUnitTests<TCommand> : ICommandUnitTests<TCommand>
    where TCommand : ISimulatorCommand<TCommand>
{
    static abstract ChatCommandType ChatCommandType { get; }
}
