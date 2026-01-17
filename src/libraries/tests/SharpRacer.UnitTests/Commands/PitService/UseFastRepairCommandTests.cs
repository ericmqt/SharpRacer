namespace SharpRacer.Commands.PitService;

public class UseFastRepairCommandTests :
    PitServiceCommandUnitTests<UseFastRepairCommand, UseFastRepairCommandTests>,
    IPitServiceCommandUnitTests<UseFastRepairCommand>
{
    public static SimulatorCommandId CommandId { get; } = SimulatorCommandId.PitService;
    public static PitServiceCommandType[] PitServiceCommandTypes { get; } = [PitServiceCommandType.FastRepair];

    [Fact]
    public void ToCommandMessage_Test()
    {
        var msg = new UseFastRepairCommand().ToCommandMessage();

        CommandMessageAssert.Arg1Equals(PitServiceCommandType.FastRepair, msg);
        CommandMessageAssert.Arg2Empty(msg);
        CommandMessageAssert.Arg3Empty(msg);
    }

    [Fact]
    public void Parse_ThrowIfPitServiceTypeArgNotValidTest()
    {
        var msg = new CommandMessage(UseFastRepairCommand.CommandId, (ushort)PitServiceCommandType.AddFuel);

        Assert.Throws<CommandMessageParseException>(() => UseFastRepairCommand.Parse(msg));
    }

    [Fact]
    public void TryParse_ReturnFalseIfPitServiceTypeArgNotValidTest()
    {
        var msg = new CommandMessage(UseFastRepairCommand.CommandId, (ushort)PitServiceCommandType.AddFuel);

        Assert.False(UseFastRepairCommand.TryParse(msg, out var parsedCommand));
        Assert.Equal(default, parsedCommand);
    }

    public static IEnumerable<(UseFastRepairCommand Command1, UseFastRepairCommand Command2)> EnumerateEqualityValues()
    {
        yield return (new(), new());
        yield return (default, default);
    }

    public static IEnumerable<(UseFastRepairCommand Command1, UseFastRepairCommand Command2)> EnumerateInequalityValues()
    {
        yield break;
    }

    public static IEnumerable<UseFastRepairCommand> EnumerateValidCommands()
    {
        yield return new();
    }


}
