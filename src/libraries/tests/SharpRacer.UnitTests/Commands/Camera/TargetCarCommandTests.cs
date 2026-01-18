namespace SharpRacer.Commands.Camera;

public class TargetCarCommandTests : CommandUnitTests<TargetCarCommand, TargetCarCommandTests>, ICommandUnitTests<TargetCarCommand>
{
    public static SimulatorCommandId CommandId { get; } = SimulatorCommandId.CameraTargetDriver;

    [Fact]
    public void Ctor_CarNumberTest()
    {
        var carNumber = new CarNumber(42);
        const ushort cameraGroup = 2;
        const ushort cameraIndex = 37;

        var cmd = new TargetCarCommand(carNumber, cameraGroup, cameraIndex);

        Assert.Equal(CameraTargetMode.Driver, cmd.TargetMode);
        Assert.Equal(carNumber, cmd.CarNumber);
        Assert.Equal(cameraGroup, cmd.CameraGroup);
        Assert.Equal(cameraIndex, cmd.CameraIndex);
    }

    [Fact]
    public void Ctor_CarNumber_ThrowIfCarNumberEqualsNoneTest()
    {
        const ushort cameraGroup = 2;
        const ushort cameraIndex = 37;

        Assert.Throws<ArgumentException>(() => new TargetCarCommand(CarNumber.None, cameraGroup, cameraIndex));

        Assert.Throws<ArgumentException>(() => new TargetCarCommand(default(CarNumber), cameraGroup, cameraIndex));
    }

    [Fact]
    public void Ctor_TargetModeTest()
    {
        const CameraTargetMode targetMode = CameraTargetMode.Incident;
        const ushort cameraGroup = 2;
        const ushort cameraIndex = 37;

        var cmd = new TargetCarCommand(targetMode, cameraGroup, cameraIndex);

        Assert.Equal(targetMode, cmd.TargetMode);
        Assert.Equal(CarNumber.None, cmd.CarNumber);
        Assert.Equal(cameraGroup, cmd.CameraGroup);
        Assert.Equal(cameraIndex, cmd.CameraIndex);
    }

    [Fact]
    public void CtorInternal_ThrowIfCarNumberNotValidForTargetModeTest()
    {
        Assert.Throws<ArgumentException>(() => new TargetCarCommand(CarNumber.None, CameraTargetMode.Driver, 1, 1));
    }

    [Fact]
    public void CtorInternal_ThrowIfTargetModeNotValidForCarNumberTest()
    {
        Assert.Throws<ArgumentException>(() => new TargetCarCommand(new CarNumber(42), CameraTargetMode.Incident, 1, 1));
    }

    [Fact]
    public void ToCommandMessage_DriverNumberTest()
    {
        const ushort driverNumber = 42;
        const ushort cameraGroup = 2;
        const ushort cameraIndex = 37;

        var msg = new CommandMessage(TargetCarCommand.CommandId, driverNumber, cameraGroup, cameraIndex);

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

        var msg = new CommandMessage(TargetCarCommand.CommandId, unchecked((ushort)targetMode), cameraGroup, cameraIndex);

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

        var msg = new CommandMessage(TargetCarCommand.CommandId, invalidTargetMode, cameraGroup, cameraIndex);

        Assert.Throws<CommandMessageParseException>(() => TargetCarCommand.Parse(msg));
    }

    [Fact]
    public void TryParse_ReturnFalseIfTargetModeNotValidTest()
    {
        const ushort invalidTargetMode = unchecked((ushort)short.MinValue);
        const ushort cameraGroup = 2;
        const ushort cameraIndex = 37;

        var msg = new CommandMessage(TargetCarCommand.CommandId, invalidTargetMode, cameraGroup, cameraIndex);

        Assert.False(TargetCarCommand.TryParse(msg, out var parsedCommand));
        Assert.Equal(default, parsedCommand);
    }

    public static IEnumerable<(TargetCarCommand Command1, TargetCarCommand Command2)> EnumerateEqualityValues()
    {
        const CameraTargetMode targetMode = CameraTargetMode.Incident;
        const int cameraGroup = 2;
        const int cameraIndex = 37;
        var carNumber = new CarNumber(42);

        yield return (new(targetMode, cameraGroup, cameraIndex), new(targetMode, cameraGroup, cameraIndex));
        yield return (new(carNumber, cameraGroup, cameraIndex), new(carNumber, cameraGroup, cameraIndex));
        yield return (default, default);
    }

    public static IEnumerable<(TargetCarCommand Command1, TargetCarCommand Command2)> EnumerateInequalityValues()
    {
        const CameraTargetMode targetMode = CameraTargetMode.Incident;
        const int cameraGroup = 2;
        const int cameraIndex = 37;
        var carNumber = new CarNumber(42);

        yield return (new(CameraTargetMode.Incident, cameraGroup, cameraIndex), new(CameraTargetMode.Leader, cameraGroup, cameraIndex));
        yield return (new(targetMode, cameraGroup, cameraIndex), new(targetMode, cameraGroup + 1, cameraIndex));
        yield return (new(targetMode, cameraGroup, cameraIndex), new(targetMode, cameraGroup, cameraIndex + 1));
        yield return (new(CameraTargetMode.Incident, cameraGroup, cameraIndex), default);

        yield return (new(new CarNumber(1), cameraGroup, cameraIndex), new(new CarNumber(3), cameraGroup, cameraIndex));
        yield return (new(carNumber, cameraGroup, cameraIndex), new(carNumber, cameraGroup + 1, cameraIndex));
        yield return (new(carNumber, cameraGroup, cameraIndex), new(carNumber, cameraGroup, cameraIndex + 1));
        yield return (new(carNumber, cameraGroup, cameraIndex), default);
    }

    public static IEnumerable<TargetCarCommand> EnumerateValidCommands()
    {
        const CameraTargetMode targetMode = CameraTargetMode.Incident;
        const int cameraGroup = 2;
        const int cameraIndex = 37;
        var carNumber = new CarNumber(42);

        yield return new(targetMode, cameraGroup, cameraIndex);
        yield return new(carNumber, cameraGroup, cameraIndex);
    }
}
