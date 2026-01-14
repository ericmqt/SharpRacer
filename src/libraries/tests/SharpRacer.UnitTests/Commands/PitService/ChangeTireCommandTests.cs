namespace SharpRacer.Commands.PitService;

public class ChangeTireCommandTests :
    PitServiceCommandUnitTests<ChangeTireCommand, ChangeTireCommandTests>,
    IPitServiceCommandUnitTests<ChangeTireCommand>
{
    public static SimulatorCommandId CommandId { get; } = SimulatorCommandId.PitCommand;
    public static PitServiceCommandType[] PitServiceCommandTypes { get; } = [
            (PitServiceCommandType)((ushort)PitServiceCommandType.TireChange + (byte)TireChangeTarget.LeftFront),
            (PitServiceCommandType)((ushort)PitServiceCommandType.TireChange + (byte)TireChangeTarget.RightFront),
            (PitServiceCommandType)((ushort)PitServiceCommandType.TireChange + (byte)TireChangeTarget.LeftRear),
            (PitServiceCommandType)((ushort)PitServiceCommandType.TireChange + (byte)TireChangeTarget.RightRear)
        ];

    [Fact]
    public void Ctor_DefaultPressureTest()
    {
        var tire = TireChangeTarget.RightRear;
        var command = new ChangeTireCommand(tire);

        Assert.Equal(tire, command.Tire);
        Assert.Equal(0, command.PressureKPa);
    }

    [Fact]
    public void Ctor_TirePressureTest()
    {
        var tire = TireChangeTarget.RightRear;
        var tirePressure = 37;
        var command = new ChangeTireCommand(tire, tirePressure);

        Assert.Equal(tire, command.Tire);
        Assert.Equal(tirePressure, command.PressureKPa);
    }

    [Fact]
    public void Ctor_ThrowIfPressureIsNegativeTest()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ChangeTireCommand(TireChangeTarget.RightFront, -1));
    }

    [Fact]
    public void ToCommandMessage_Test()
    {
        const ushort tireChangeBase = (ushort)PitServiceCommandType.TireChange;
        const TireChangeTarget tire = TireChangeTarget.RightRear;
        const ushort expectedTireRawValue = tireChangeBase + (byte)tire;

        var msg = new ChangeTireCommand(tire).ToCommandMessage();

        CommandMessageAssert.Arg1Equals(expectedTireRawValue, msg);
        CommandMessageAssert.Arg2Empty(msg);
        CommandMessageAssert.Arg3Empty(msg);
    }

    [Fact]
    public void ToCommandMessage_PressureTest()
    {
        const ushort tireChangeBase = (ushort)PitServiceCommandType.TireChange;
        const TireChangeTarget tire = TireChangeTarget.RightRear;
        const ushort expectedTireRawValue = tireChangeBase + (byte)tire;
        const int tirePressure = 32;

        var msg = new ChangeTireCommand(tire, tirePressure).ToCommandMessage();

        CommandMessageAssert.Arg1Equals(expectedTireRawValue, msg);
        CommandMessageAssert.Arg2Equals(tirePressure, msg);
    }

    [Fact]
    public void Parse_ThrowIfPressureArgIsNegativeTest()
    {
        var tireChangeArg = (ushort)((ushort)PitServiceCommandType.TireChange + (byte)TireChangeTarget.RightRear);
        var msg = new CommandMessage(ChangeTireCommand.CommandId, tireChangeArg, -1);

        Assert.Throws<CommandMessageParseException>(() => ChangeTireCommand.Parse(msg));
    }

    [Fact]
    public void TryParse_ReturnFalseIfPressureArgIsNegativeTest()
    {
        var tireChangeArg = (ushort)((ushort)PitServiceCommandType.TireChange + (byte)TireChangeTarget.RightRear);
        var msg = new CommandMessage(ChangeTireCommand.CommandId, tireChangeArg, -1);

        Assert.False(ChangeTireCommand.TryParse(msg, out var parsedCommand));
        Assert.Equal(default, parsedCommand);
    }

    public static IEnumerable<(ChangeTireCommand Command1, ChangeTireCommand Command2)> EnumerateEqualityValues()
    {
        yield return (new(TireChangeTarget.LeftRear), new(TireChangeTarget.LeftRear));
        yield return (new(TireChangeTarget.LeftRear, 123), new(TireChangeTarget.LeftRear, 123));
        yield return (default, default);
    }

    public static IEnumerable<(ChangeTireCommand Command1, ChangeTireCommand Command2)> EnumerateInequalityValues()
    {
        yield return (new(TireChangeTarget.LeftRear), new ChangeTireCommand(TireChangeTarget.LeftRear, 1));
        yield return (new(TireChangeTarget.LeftRear, 1), new ChangeTireCommand(TireChangeTarget.RightRear, 1));
        yield return (new(TireChangeTarget.LeftRear, 123), new ChangeTireCommand(TireChangeTarget.LeftRear, 456));
    }

    public static IEnumerable<ChangeTireCommand> EnumerateValidCommands()
    {
        yield return new(TireChangeTarget.LeftFront, 25);
        yield return new(TireChangeTarget.LeftRear, 41);
        yield return new(TireChangeTarget.RightFront);
        yield return new(TireChangeTarget.RightRear);
    }
}
