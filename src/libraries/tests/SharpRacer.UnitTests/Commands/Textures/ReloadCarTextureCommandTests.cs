namespace SharpRacer.Commands.Textures;

public class ReloadCarTextureCommandTests : CommandUnitTests<ReloadCarTextureCommand, ReloadCarTextureCommandTests>, ICommandUnitTests<ReloadCarTextureCommand>
{
    public static SimulatorCommandId CommandId { get; } = SimulatorCommandId.ReloadTextures;

    [Fact]
    public void Ctor_Test()
    {
        const ushort carIndex = 42;

        var command = new ReloadCarTextureCommand(carIndex);

        Assert.Equal(carIndex, command.CarIndex);
    }

    [Fact]
    public void Ctor_ReloadAllCarsTest()
    {
        var command = new ReloadCarTextureCommand();

        Assert.Equal(0, command.CarIndex);
    }

    [Fact]
    public void Ctor_ThrowIfCarIndexIsNegativeTest()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ReloadCarTextureCommand(-1));
    }

    [Fact]
    public void ToCommandMessage_Test()
    {
        const ushort carIndex = 42;

        var msg = new ReloadCarTextureCommand(carIndex).ToCommandMessage();

        CommandMessageAssert.Arg1Equals(carIndex, msg);
        CommandMessageAssert.Arg2Empty(msg);
        CommandMessageAssert.Arg3Empty(msg);
    }

    public static IEnumerable<(ReloadCarTextureCommand Command1, ReloadCarTextureCommand Command2)> EnumerateEqualityValues()
    {
        yield return (new(0), new(0));
        yield return (new(1), new(1));
        yield return (new(32), new(32));
        yield return (default, default);
    }

    public static IEnumerable<(ReloadCarTextureCommand Command1, ReloadCarTextureCommand Command2)> EnumerateInequalityValues()
    {
        yield return (new(0), new(100));
        yield return (new(1), new(2));
        yield return (new(34), new(56));
        yield return (default, new(78));
    }

    public static IEnumerable<ReloadCarTextureCommand> EnumerateValidCommands()
    {
        yield return new(37);
        yield return new(1);
        yield return new(0);
    }
}
