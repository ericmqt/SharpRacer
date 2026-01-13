namespace SharpRacer.Commands.PitService;

public interface IPitServiceCommandUnitTests<TCommand> : ICommandUnitTests<TCommand>
    where TCommand : ISimulatorCommand<TCommand>
{
    /// <summary>
    /// Gets the <see cref="PitServiceCommandType"/> values that are valid for <typeparamref name="TCommand"/>.
    /// </summary>
    static abstract PitServiceCommandType[] PitServiceCommandTypes { get; }
}
