namespace SharpRacer.Commands.Camera;

public class TargetRacePositionCommandTests : CommandUnitTests<TargetRacePositionCommand, TargetRacePositionCommandTests>, ICommandUnitTests<TargetRacePositionCommand>
{
    public static SimulatorCommandId CommandId { get; } = SimulatorCommandId.CameraSwitchPosition;

    [Fact]
    public void Ctor_PositionTest()
    {
        const ushort racePosition = 42;
        const ushort cameraGroup = 2;
        const ushort cameraIndex = 37;

        var cmd = new TargetRacePositionCommand(racePosition, cameraGroup, cameraIndex);

        Assert.Equal(CameraTargetMode.Driver, cmd.TargetMode);
        Assert.Equal(racePosition, cmd.Position);
        Assert.Equal(cameraGroup, cmd.CameraGroup);
        Assert.Equal(cameraIndex, cmd.CameraIndex);
    }

    [Fact]
    public void Ctor_TargetModeTest()
    {
        const CameraTargetMode targetMode = CameraTargetMode.Incident;
        const ushort cameraGroup = 2;
        const ushort cameraIndex = 37;

        var cmd = new TargetRacePositionCommand(targetMode, cameraGroup, cameraIndex);

        Assert.Equal(targetMode, cmd.TargetMode);
        Assert.Equal(0, cmd.Position);
        Assert.Equal(cameraGroup, cmd.CameraGroup);
        Assert.Equal(cameraIndex, cmd.CameraIndex);
    }

    [Fact]
    public void CtorInternal_ThrowIfTargetModeNotValidForPositionTest()
    {
        Assert.Throws<ArgumentException>(() => new TargetRacePositionCommand(CameraTargetMode.Incident, 42, 1, 1));
    }

    [Fact]
    public void ToCommandMessage_PositionTest()
    {
        const ushort racePosition = 42;
        const ushort cameraGroup = 2;
        const ushort cameraIndex = 37;

        var msg = new TargetRacePositionCommand(racePosition, cameraGroup, cameraIndex).ToCommandMessage();

        CommandMessageAssert.Arg1Equals(racePosition, msg);
        CommandMessageAssert.Arg2Equals(cameraGroup, msg);
        CommandMessageAssert.Arg3Equals(cameraIndex, msg);
    }

    [Fact]
    public void ToCommandMessage_TargetModeTest()
    {
        const CameraTargetMode targetMode = CameraTargetMode.Incident;
        const ushort cameraGroup = 2;
        const ushort cameraIndex = 37;

        var msg = new TargetRacePositionCommand(targetMode, cameraGroup, cameraIndex).ToCommandMessage();

        CommandMessageAssert.Arg1Equals(targetMode, msg);
        CommandMessageAssert.Arg2Equals(cameraGroup, msg);
        CommandMessageAssert.Arg3Equals(cameraIndex, msg);
    }

    [Fact]
    public void Parse_ThrowIfTargetModeNotValidTest()
    {
        const ushort invalidTargetMode = unchecked((ushort)short.MinValue);
        const ushort cameraGroup = 2;
        const ushort cameraIndex = 37;

        var msg = new CommandMessage(TargetRacePositionCommand.CommandId, invalidTargetMode, cameraGroup, cameraIndex);

        Assert.Throws<CommandMessageParseException>(() => TargetRacePositionCommand.Parse(msg));
    }

    [Fact]
    public void TryParse_ReturnFalseIfTargetModeNotValidTest()
    {
        const ushort invalidTargetMode = unchecked((ushort)short.MinValue);
        const ushort cameraGroup = 2;
        const ushort cameraIndex = 37;

        var msg = new CommandMessage(TargetRacePositionCommand.CommandId, invalidTargetMode, cameraGroup, cameraIndex);

        Assert.False(TargetRacePositionCommand.TryParse(msg, out var parsedCommand));
        Assert.Equal(default, parsedCommand);
    }

    public static IEnumerable<(TargetRacePositionCommand Command1, TargetRacePositionCommand Command2)> EnumerateEqualityValues()
    {
        const CameraTargetMode targetMode = CameraTargetMode.Incident;
        const int racePosition = 42;
        const int cameraGroup = 2;
        const int cameraIndex = 37;

        yield return (new(targetMode, cameraGroup, cameraIndex), new(targetMode, cameraGroup, cameraIndex));
        yield return (new(racePosition, cameraGroup, cameraIndex), new(racePosition, cameraGroup, cameraIndex));
        yield return (default, default);
    }

    public static IEnumerable<(TargetRacePositionCommand Command1, TargetRacePositionCommand Command2)> EnumerateInequalityValues()
    {
        const CameraTargetMode targetMode = CameraTargetMode.Incident;
        const int racePosition = 42;
        const int cameraGroup = 2;
        const int cameraIndex = 37;

        yield return (new(CameraTargetMode.Incident, cameraGroup, cameraIndex), new(CameraTargetMode.Leader, cameraGroup, cameraIndex));
        yield return (new(targetMode, cameraGroup, cameraIndex), new(targetMode, cameraGroup + 1, cameraIndex));
        yield return (new(targetMode, cameraGroup, cameraIndex), new(targetMode, cameraGroup, cameraIndex + 1));
        yield return (new(CameraTargetMode.Incident, cameraGroup, cameraIndex), default);

        yield return (new(1, cameraGroup, cameraIndex), new(2, cameraGroup, cameraIndex));
        yield return (new(racePosition, cameraGroup, cameraIndex), new(racePosition, cameraGroup + 1, cameraIndex));
        yield return (new(racePosition, cameraGroup, cameraIndex), new(racePosition, cameraGroup, cameraIndex + 1));
        yield return (new(racePosition, cameraGroup, cameraIndex), default);
    }

    public static IEnumerable<TargetRacePositionCommand> EnumerateValidCommands()
    {
        const CameraTargetMode targetMode = CameraTargetMode.Incident;
        const int racePosition = 42;
        const int cameraGroup = 2;
        const int cameraIndex = 37;

        yield return new(targetMode, cameraGroup, cameraIndex);
        yield return new(racePosition, cameraGroup, cameraIndex);
    }
}
