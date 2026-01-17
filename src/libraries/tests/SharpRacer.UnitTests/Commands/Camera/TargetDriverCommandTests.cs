namespace SharpRacer.Commands.Camera;

public class TargetDriverCommandTests : CommandUnitTests<TargetDriverCommand, TargetDriverCommandTests>, ICommandUnitTests<TargetDriverCommand>
{
    public static SimulatorCommandId CommandId { get; } = SimulatorCommandId.CameraTargetDriver;

    [Fact]
    public void Ctor_DriverNumberTest()
    {
        const ushort driverNumber = 42;
        const ushort cameraGroup = 2;
        const ushort cameraIndex = 37;

        var cmd = new TargetDriverCommand(driverNumber, cameraGroup, cameraIndex);

        Assert.Equal(CameraTargetMode.Driver, cmd.TargetMode);
        Assert.Equal(driverNumber, cmd.DriverNumber);
        Assert.Equal(cameraGroup, cmd.CameraGroup);
        Assert.Equal(cameraIndex, cmd.CameraIndex);
    }

    [Fact]
    public void Ctor_TargetModeTest()
    {
        const CameraTargetMode targetMode = CameraTargetMode.Incident;
        const ushort cameraGroup = 2;
        const ushort cameraIndex = 37;

        var cmd = new TargetDriverCommand(targetMode, cameraGroup, cameraIndex);

        Assert.Equal(targetMode, cmd.TargetMode);
        Assert.Equal(0, cmd.DriverNumber);
        Assert.Equal(cameraGroup, cmd.CameraGroup);
        Assert.Equal(cameraIndex, cmd.CameraIndex);
    }

    [Fact]
    public void CtorInternal_ThrowIfTargetModeNotValidForDriverNumberTest()
    {
        Assert.Throws<ArgumentException>(() => new TargetDriverCommand(CameraTargetMode.Incident, 42, 1, 1));
    }

    [Fact]
    public void ToCommandMessage_DriverNumberTest()
    {
        const ushort driverNumber = 42;
        const ushort cameraGroup = 2;
        const ushort cameraIndex = 37;

        var msg = new CommandMessage(TargetDriverCommand.CommandId, driverNumber, cameraGroup, cameraIndex);

        CommandMessageAssert.Arg1Equals(driverNumber, msg);
        CommandMessageAssert.Arg2Equals(cameraGroup, msg);
        CommandMessageAssert.Arg3Equals(cameraIndex, msg);
    }

    [Fact]
    public void ToCommandMessage_TargetModeTest()
    {
        const CameraTargetMode targetMode = CameraTargetMode.Incident;
        const ushort cameraGroup = 2;
        const ushort cameraIndex = 37;

        var msg = new CommandMessage(TargetDriverCommand.CommandId, unchecked((ushort)targetMode), cameraGroup, cameraIndex);

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

        var msg = new CommandMessage(TargetDriverCommand.CommandId, invalidTargetMode, cameraGroup, cameraIndex);

        Assert.Throws<CommandMessageParseException>(() => TargetDriverCommand.Parse(msg));
    }

    [Fact]
    public void TryParse_ReturnFalseIfTargetModeNotValidTest()
    {
        const ushort invalidTargetMode = unchecked((ushort)short.MinValue);
        const ushort cameraGroup = 2;
        const ushort cameraIndex = 37;

        var msg = new CommandMessage(TargetDriverCommand.CommandId, invalidTargetMode, cameraGroup, cameraIndex);

        Assert.False(TargetDriverCommand.TryParse(msg, out var parsedCommand));
        Assert.Equal(default, parsedCommand);
    }

    public static IEnumerable<(TargetDriverCommand Command1, TargetDriverCommand Command2)> EnumerateEqualityValues()
    {
        const CameraTargetMode targetMode = CameraTargetMode.Incident;
        const int driverNumber = 42;
        const int cameraGroup = 2;
        const int cameraIndex = 37;

        yield return (new(targetMode, cameraGroup, cameraIndex), new(targetMode, cameraGroup, cameraIndex));
        yield return (new(driverNumber, cameraGroup, cameraIndex), new(driverNumber, cameraGroup, cameraIndex));
        yield return (default, default);
    }

    public static IEnumerable<(TargetDriverCommand Command1, TargetDriverCommand Command2)> EnumerateInequalityValues()
    {
        const CameraTargetMode targetMode = CameraTargetMode.Incident;
        const int driverNumber = 42;
        const int cameraGroup = 2;
        const int cameraIndex = 37;

        yield return (new(CameraTargetMode.Incident, cameraGroup, cameraIndex), new(CameraTargetMode.Leader, cameraGroup, cameraIndex));
        yield return (new(targetMode, cameraGroup, cameraIndex), new(targetMode, cameraGroup + 1, cameraIndex));
        yield return (new(targetMode, cameraGroup, cameraIndex), new(targetMode, cameraGroup, cameraIndex + 1));
        yield return (new(CameraTargetMode.Incident, cameraGroup, cameraIndex), default);

        yield return (new(1, cameraGroup, cameraIndex), new(2, cameraGroup, cameraIndex));
        yield return (new(driverNumber, cameraGroup, cameraIndex), new(driverNumber, cameraGroup + 1, cameraIndex));
        yield return (new(driverNumber, cameraGroup, cameraIndex), new(driverNumber, cameraGroup, cameraIndex + 1));
        yield return (new(driverNumber, cameraGroup, cameraIndex), default);
    }

    public static IEnumerable<TargetDriverCommand> EnumerateValidCommands()
    {
        const CameraTargetMode targetMode = CameraTargetMode.Incident;
        const int driverNumber = 42;
        const int cameraGroup = 2;
        const int cameraIndex = 37;

        yield return new(targetMode, cameraGroup, cameraIndex);
        yield return new(driverNumber, cameraGroup, cameraIndex);
    }
}
