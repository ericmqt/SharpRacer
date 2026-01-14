namespace SharpRacer.Commands;

public interface ICommandUnitTests<TCommand>
    where TCommand : ISimulatorCommand<TCommand>
{
    static abstract SimulatorCommandId CommandId { get; }

    static abstract IEnumerable<(TCommand Command1, TCommand Command2)> EnumerateEqualityValues();
    static abstract IEnumerable<(TCommand Command1, TCommand Command2)> EnumerateInequalityValues();
    static abstract IEnumerable<TCommand> EnumerateValidCommands();
}
