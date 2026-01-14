using SharpRacer.Extensions.Xunit.Utilities;

namespace SharpRacer.Commands.Replay;

public class SeekFrameCommandTests : CommandUnitTests<SeekFrameCommand, SeekFrameCommandTests>, ICommandUnitTests<SeekFrameCommand>
{
    public static SimulatorCommandId CommandId { get; } = SimulatorCommandId.ReplaySetPlayPosition;

    [Fact]
    public void Ctor_FrameTest()
    {
        const int frame = 12345;

        var cmd = new SeekFrameCommand(frame);

        Assert.Equal(frame, cmd.Frame);
        Assert.Equal(ReplaySeekOrigin.Current, cmd.SeekOrigin);
    }

    [Fact]
    public void Ctor_FrameSeekOriginTest()
    {
        const int frame = 12345;
        const ReplaySeekOrigin seekOrigin = ReplaySeekOrigin.Current;

        var cmd = new SeekFrameCommand(frame, seekOrigin);

        Assert.Equal(frame, cmd.Frame);
        Assert.Equal(seekOrigin, cmd.SeekOrigin);
    }

    [Fact]
    public void Ctor_SeekOriginBegin_ThrowIfFrameIsNegativeTest()
    {
        const int frame = -1;
        const ReplaySeekOrigin seekOrigin = ReplaySeekOrigin.Begin;

        Assert.Throws<ArgumentOutOfRangeException>(() => new SeekFrameCommand(frame, seekOrigin));
    }

    [Fact]
    public void Ctor_SeekOriginEnd_ThrowIfFrameIsPositiveTest()
    {
        const int frame = 1;
        const ReplaySeekOrigin seekOrigin = ReplaySeekOrigin.End;

        Assert.Throws<ArgumentOutOfRangeException>(() => new SeekFrameCommand(frame, seekOrigin));
    }

    [Fact]
    public void ToCommandMessage_Test()
    {
        const int frame = 12345;
        const ReplaySeekOrigin seekOrigin = ReplaySeekOrigin.Current;

        var cmd = new SeekFrameCommand(frame, seekOrigin);
        var msg = cmd.ToCommandMessage();

        CommandMessageAssert.Arg1Equals(seekOrigin, msg);
        CommandMessageAssert.Arg2Equals(frame, msg);
    }

    [Fact]
    public void Parse_ThrowIfSeekOriginBeginAndNegativeFrameTest()
    {
        var msg = new CommandMessage((ushort)CommandId, (ushort)ReplaySeekOrigin.Begin, -1);

        Assert.Throws<CommandMessageParseException>(() => SeekFrameCommand.Parse(msg));
    }

    [Fact]
    public void Parse_ThrowIfSeekOriginEndAndPositiveFrameTest()
    {
        var msg = new CommandMessage((ushort)CommandId, (ushort)ReplaySeekOrigin.End, 1);

        Assert.Throws<CommandMessageParseException>(() => SeekFrameCommand.Parse(msg));
    }

    [Fact]
    public void Parse_ThrowIfSeekOriginNotValidTest()
    {
        var invalidSeekOriginVal = (ushort)(EnumTestUtilities.MaxIntegralValue<ReplaySeekOrigin, ushort>() + 1);

        var msg = ModifyValidCommandMessage(arg1: invalidSeekOriginVal);

        Assert.Throws<CommandMessageParseException>(() => SeekFrameCommand.Parse(msg));
    }

    [Fact]
    public void TryParse_ReturnFalseIfSeekOriginBeginAndNegativeFrameTest()
    {
        var msg = new CommandMessage((ushort)CommandId, (ushort)ReplaySeekOrigin.Begin, -1);

        Assert.False(SeekFrameCommand.TryParse(msg, out var parsedCommand));
        Assert.Equal(default, parsedCommand);
    }

    [Fact]
    public void TryParse_ReturnFalseIfSeekOriginEndAndPositiveFrameTest()
    {
        var msg = new CommandMessage((ushort)CommandId, (ushort)ReplaySeekOrigin.End, 1);

        Assert.False(SeekFrameCommand.TryParse(msg, out var parsedCommand));
        Assert.Equal(default, parsedCommand);
    }

    [Fact]
    public void TryParse_ReturnFalseIfSeekOriginNotValidTest()
    {
        var invalidSeekOriginVal = (ushort)(EnumTestUtilities.MaxIntegralValue<ReplaySeekOrigin, ushort>() + 1);

        var msg = ModifyValidCommandMessage(arg1: invalidSeekOriginVal);

        Assert.False(SeekFrameCommand.TryParse(msg, out var parsedCommand));
        Assert.Equal(default, parsedCommand);
    }

    public static IEnumerable<(SeekFrameCommand Command1, SeekFrameCommand Command2)> EnumerateEqualityValues()
    {
        yield return (new(123, ReplaySeekOrigin.Begin), new(123, ReplaySeekOrigin.Begin));
        yield return (new(456, ReplaySeekOrigin.Current), new(456, ReplaySeekOrigin.Current));
        yield return (new(-789, ReplaySeekOrigin.End), new(-789, ReplaySeekOrigin.End));
        yield return (default, default);
    }

    public static IEnumerable<(SeekFrameCommand Command1, SeekFrameCommand Command2)> EnumerateInequalityValues()
    {
        yield return (new(123, ReplaySeekOrigin.Begin), new(-456, ReplaySeekOrigin.End));
        yield return (new(-10, ReplaySeekOrigin.Current), new(-20, ReplaySeekOrigin.Current));
    }

    public static IEnumerable<SeekFrameCommand> EnumerateValidCommands()
    {
        yield return new(234, ReplaySeekOrigin.Current);
        yield return new(1, ReplaySeekOrigin.Begin);
        yield return new(-1, ReplaySeekOrigin.End);
    }
}
