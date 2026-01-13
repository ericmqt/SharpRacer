namespace SharpRacer.Commands.PitService;

public class AddFuelCommandTests :
    PitServiceCommandUnitTests<AddFuelCommand, AddFuelCommandTests>,
    IPitServiceCommandUnitTests<AddFuelCommand>
{
    public static SimulatorCommandId CommandId { get; } = SimulatorCommandId.PitCommand;
    public static PitServiceCommandType[] PitServiceCommandTypes { get; } = [PitServiceCommandType.AddFuel];

    [Fact]
    public void Ctor_Test()
    {
        var cmd = new AddFuelCommand();

        Assert.Equal(0, cmd.FuelQuantity);
    }

    [Fact]
    public void Ctor_FuelQuantityTest()
    {
        const int fuelQuantity = 4;

        var cmd = new AddFuelCommand(fuelQuantity);

        Assert.Equal(fuelQuantity, cmd.FuelQuantity);
    }

    [Fact]
    public void Ctor_ThrowIfFuelQuantityIsNegativeTest()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new AddFuelCommand(-1));
    }

    [Fact]
    public void ToCommandMessage_Test()
    {
        const int fuelQuantity = 4;
        var msg = new AddFuelCommand(fuelQuantity).ToCommandMessage();

        CommandMessageAssert.Arg1Equals(PitServiceCommandType.AddFuel, msg);
        CommandMessageAssert.Arg2Equals(fuelQuantity, msg);

        // Fuel unspecified
        msg = new AddFuelCommand().ToCommandMessage();

        CommandMessageAssert.Arg1Equals(PitServiceCommandType.AddFuel, msg);
        CommandMessageAssert.Arg2Empty(msg);
        CommandMessageAssert.Arg3Empty(msg);
    }

    [Fact]
    public void Parse_ThrowIfFuelQuantityArgIsNegativeTest()
    {
        const int fuelQuantity = -1;
        var msg = new CommandMessage(AddFuelCommand.CommandId, (ushort)PitServiceCommandType.AddFuel, fuelQuantity);

        Assert.Throws<CommandMessageParseException>(() => AddFuelCommand.Parse(msg));
    }

    [Fact]
    public void TryParse_ReturnFalseIfFuelQuantityArgIsNegativeTest()
    {
        const int fuelQuantity = -1;
        var msg = new CommandMessage(AddFuelCommand.CommandId, (ushort)PitServiceCommandType.AddFuel, fuelQuantity);

        Assert.False(AddFuelCommand.TryParse(msg, out var parsedCommand));
        Assert.Equal(default, parsedCommand);
    }

    public static IEnumerable<(AddFuelCommand Command1, AddFuelCommand Command2)> EnumerateEqualityValues()
    {
        yield return (new(), new());
        yield return (new(12), new(12));
        yield return (default, default);
    }

    public static IEnumerable<(AddFuelCommand Command1, AddFuelCommand Command2)> EnumerateInequalityValues()
    {
        yield return (new(), new(123));
        yield return (new(24), new(48));
        yield return (default, new(376));
    }

    public static IEnumerable<AddFuelCommand> EnumerateValidCommands()
    {
        yield return new(24);
        yield return new();
    }
}
