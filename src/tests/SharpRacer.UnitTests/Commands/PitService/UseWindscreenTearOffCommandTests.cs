namespace SharpRacer.Commands.PitService;

public class UseWindscreenTearOffCommandTests :
    PitServiceCommandUnitTests<UseWindscreenTearOffCommand, UseWindscreenTearOffCommandTests>,
    IPitServiceCommandUnitTests<UseWindscreenTearOffCommand>
{
    public static SimulatorCommandId CommandId { get; } = SimulatorCommandId.PitCommand;
    public static PitServiceCommandType[] PitServiceCommandTypes { get; } = [PitServiceCommandType.WindscreenTearOff];

    [Fact]
    public void ToCommandMessage_Test()
    {
        var msg = new UseWindscreenTearOffCommand().ToCommandMessage();

        CommandMessageAssert.Arg1Equals(PitServiceCommandType.WindscreenTearOff, msg);
        CommandMessageAssert.Arg2Empty(msg);
        CommandMessageAssert.Arg3Empty(msg);
    }

    [Fact]
    public void Parse_ThrowIfPitServiceTypeArgNotValidTest()
    {
        var msg = new CommandMessage(UseWindscreenTearOffCommand.CommandId, (ushort)PitServiceCommandType.AddFuel);

        Assert.Throws<CommandMessageParseException>(() => UseWindscreenTearOffCommand.Parse(msg));
    }

    [Fact]
    public void TryParse_ReturnFalseIfPitServiceTypeArgNotValidTest()
    {
        var msg = new CommandMessage(UseWindscreenTearOffCommand.CommandId, (ushort)PitServiceCommandType.AddFuel);

        Assert.False(UseFastRepairCommand.TryParse(msg, out var parsedCommand));
        Assert.Equal(default, parsedCommand);
    }

    public static IEnumerable<(UseWindscreenTearOffCommand Command1, UseWindscreenTearOffCommand Command2)> EnumerateEqualityValues()
    {
        yield return (new(), new());
        yield return (default, default);
    }

    public static IEnumerable<(UseWindscreenTearOffCommand Command1, UseWindscreenTearOffCommand Command2)> EnumerateInequalityValues()
    {
        yield break;
    }

    public static IEnumerable<UseWindscreenTearOffCommand> EnumerateValidCommands()
    {
        yield return new();
    }
}
