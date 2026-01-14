namespace SharpRacer.Commands.Replay;

public class SetPlaySpeedCommandTests : CommandUnitTests<SetPlaySpeedCommand, SetPlaySpeedCommandTests>, ICommandUnitTests<SetPlaySpeedCommand>
{
    public static SimulatorCommandId CommandId { get; } = SimulatorCommandId.ReplaySetPlaySpeed;

    [Fact]
    public void Ctor_Test()
    {
        const ushort playSpeed = 4;

        var cmd = new SetPlaySpeedCommand(playSpeed);

        Assert.Equal(playSpeed, cmd.PlaySpeed);
        Assert.False(cmd.IsSlowMotion);
    }

    [Fact]
    public void Ctor_SlowMotionTest()
    {
        const ushort playSpeed = 4;

        // Regular
        var cmd = new SetPlaySpeedCommand(playSpeed, false);

        Assert.Equal(playSpeed, cmd.PlaySpeed);
        Assert.False(cmd.IsSlowMotion);

        // Slow motion
        cmd = new SetPlaySpeedCommand(playSpeed, true);

        Assert.Equal(playSpeed, cmd.PlaySpeed);
        Assert.True(cmd.IsSlowMotion);
    }

    [Fact]
    public void ToCommandMessage_Test()
    {
        const ushort playSpeed = 4;
        const bool isSlowMotion = false;

        var msg = new SetPlaySpeedCommand(playSpeed, isSlowMotion).ToCommandMessage();

        CommandMessageAssert.Arg1Equals(playSpeed, msg);
        CommandMessageAssert.Arg2Equals(isSlowMotion, msg);
        CommandMessageAssert.Arg3Empty(msg);
    }

    [Fact]
    public void ToCommandMessage_SlowMotionTest()
    {
        const ushort playSpeed = 4;
        const bool isSlowMotion = true;

        var msg = new SetPlaySpeedCommand(playSpeed, isSlowMotion).ToCommandMessage();

        CommandMessageAssert.Arg1Equals(playSpeed, msg);
        CommandMessageAssert.Arg2Equals(isSlowMotion, msg);
        CommandMessageAssert.Arg3Empty(msg);
    }

    public static IEnumerable<(SetPlaySpeedCommand Command1, SetPlaySpeedCommand Command2)> EnumerateEqualityValues()
    {
        yield return (new(1, false), new(1, false));
        yield return (new(16, true), new(16, true));
        yield return (default, default);
    }

    public static IEnumerable<(SetPlaySpeedCommand Command1, SetPlaySpeedCommand Command2)> EnumerateInequalityValues()
    {
        yield return (new(1, false), new(1, true));
        yield return (new(8, true), new(16, true));
        yield return (default, new(1, true));
    }

    public static IEnumerable<SetPlaySpeedCommand> EnumerateValidCommands()
    {
        yield return new(1, false);
        yield return new(4, false);
        yield return new(8, false);
        yield return new(16, false);
        yield return new(1, true);
        yield return new(4, true);
        yield return new(8, true);
        yield return new(16, true);
    }
}
