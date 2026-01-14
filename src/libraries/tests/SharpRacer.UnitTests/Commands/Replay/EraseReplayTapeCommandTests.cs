namespace SharpRacer.Commands.Replay;

public class EraseReplayTapeCommandTests : CommandUnitTests<EraseReplayTapeCommand, EraseReplayTapeCommandTests>, ICommandUnitTests<EraseReplayTapeCommand>
{
    public static SimulatorCommandId CommandId { get; } = SimulatorCommandId.ReplaySetState;

    [Fact]
    public void ToCommandMessage_Test()
    {
        var cmd = new EraseReplayTapeCommand();
        var msg = cmd.ToCommandMessage();

        CommandMessageAssert.Arg1Empty(msg);
        CommandMessageAssert.Arg2Empty(msg);
        CommandMessageAssert.Arg3Empty(msg);
    }

    public static IEnumerable<(EraseReplayTapeCommand Command1, EraseReplayTapeCommand Command2)> EnumerateEqualityValues()
    {
        yield return (new(), new());
        yield return (default, default);
    }

    public static IEnumerable<(EraseReplayTapeCommand Command1, EraseReplayTapeCommand Command2)> EnumerateInequalityValues()
    {
        yield break;
    }

    public static IEnumerable<EraseReplayTapeCommand> EnumerateValidCommands()
    {
        yield return new EraseReplayTapeCommand();
    }
}
