using SharpRacer.Extensions.Xunit.Utilities;

namespace SharpRacer.Commands.VideoCapture;

public class VideoCaptureCommandTests : CommandUnitTests<VideoCaptureCommand, VideoCaptureCommandTests>, ICommandUnitTests<VideoCaptureCommand>
{
    public static SimulatorCommandId CommandId { get; } = SimulatorCommandId.VideoCapture;

    [Fact]
    public void Ctor_Test()
    {
        const VideoCaptureCommandType commandType = VideoCaptureCommandType.ToggleVideoCapture;

        var command = new VideoCaptureCommand(commandType);

        Assert.Equal(commandType, command.Type);
    }

    [Fact]
    public void ToCommandMessage_Test()
    {
        const VideoCaptureCommandType commandType = VideoCaptureCommandType.ToggleVideoCapture;

        var msg = new VideoCaptureCommand(commandType).ToCommandMessage();

        CommandMessageAssert.Arg1Equals(commandType, msg);
        CommandMessageAssert.Arg2Empty(msg);
        CommandMessageAssert.Arg3Empty(msg);
    }

    [Fact]
    public void Parse_ThrowIfTelemetryCommandTypeNotValidTest()
    {
        var invalidVideoCaptureCommandTypeVal = (ushort)(EnumTestUtilities.MaxIntegralValue<VideoCaptureCommandType, ushort>() + 1);

        var msg = ModifyValidCommandMessage(arg1: invalidVideoCaptureCommandTypeVal);

        Assert.Throws<CommandMessageParseException>(() => VideoCaptureCommand.Parse(msg));
    }

    [Fact]
    public void TryParse_ReturnFalseIfTelemetryCommandTypeNotValidTest()
    {
        var invalidVideoCaptureCommandTypeVal = (ushort)(EnumTestUtilities.MaxIntegralValue<VideoCaptureCommandType, ushort>() + 1);

        var msg = ModifyValidCommandMessage(arg1: invalidVideoCaptureCommandTypeVal);

        Assert.False(VideoCaptureCommand.TryParse(msg, out var parsedCommand));
        Assert.Equal(default, parsedCommand);
    }

    public static IEnumerable<(VideoCaptureCommand Command1, VideoCaptureCommand Command2)> EnumerateEqualityValues()
    {
        yield return (new(VideoCaptureCommandType.ToggleVideoCapture), new(VideoCaptureCommandType.ToggleVideoCapture));
        yield return (default, default);
    }

    public static IEnumerable<(VideoCaptureCommand Command1, VideoCaptureCommand Command2)> EnumerateInequalityValues()
    {
        yield return (new(VideoCaptureCommandType.CaptureScreenshot), new(VideoCaptureCommandType.ToggleVideoCapture));
        yield return (new(VideoCaptureCommandType.HideVideoTimer), default);
    }

    public static IEnumerable<VideoCaptureCommand> EnumerateValidCommands()
    {
        yield return new(VideoCaptureCommandType.CaptureScreenshot);
        yield return new(VideoCaptureCommandType.BeginVideoCapture);
        yield return new(VideoCaptureCommandType.EndVideoCapture);
        yield return new(VideoCaptureCommandType.ToggleVideoCapture);
        yield return new(VideoCaptureCommandType.ShowVideoTimer);
        yield return new(VideoCaptureCommandType.HideVideoTimer);
    }
}

