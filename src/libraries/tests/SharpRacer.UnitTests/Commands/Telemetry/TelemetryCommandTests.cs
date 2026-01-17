using SharpRacer.Extensions.Xunit.Utilities;

namespace SharpRacer.Commands.Telemetry;

public class TelemetryCommandTests : CommandUnitTests<TelemetryCommand, TelemetryCommandTests>, ICommandUnitTests<TelemetryCommand>
{
    public static SimulatorCommandId CommandId { get; } = SimulatorCommandId.Telemetry;

    [Fact]
    public void Ctor_Test()
    {
        const TelemetryCommandType commandType = TelemetryCommandType.Restart;

        var command = new TelemetryCommand(commandType);

        Assert.Equal(commandType, command.Type);
    }

    [Fact]
    public void ToCommandMessage_Test()
    {
        const TelemetryCommandType commandType = TelemetryCommandType.Restart;

        var msg = new TelemetryCommand(commandType).ToCommandMessage();

        CommandMessageAssert.Arg1Equals(commandType, msg);
        CommandMessageAssert.Arg2Empty(msg);
        CommandMessageAssert.Arg3Empty(msg);
    }

    [Fact]
    public void Parse_ThrowIfTelemetryCommandTypeNotValidTest()
    {
        var invalidTelemetryCommandTypeVal = (ushort)(EnumTestUtilities.MaxIntegralValue<TelemetryCommandType, ushort>() + 1);

        var msg = ModifyValidCommandMessage(arg1: invalidTelemetryCommandTypeVal);

        Assert.Throws<CommandMessageParseException>(() => TelemetryCommand.Parse(msg));
    }

    [Fact]
    public void TryParse_ReturnFalseIfTelemetryCommandTypeNotValidTest()
    {
        var invalidTelemetryCommandTypeVal = (ushort)(EnumTestUtilities.MaxIntegralValue<TelemetryCommandType, ushort>() + 1);

        var msg = ModifyValidCommandMessage(arg1: invalidTelemetryCommandTypeVal);

        Assert.False(TelemetryCommand.TryParse(msg, out var parsedCommand));
        Assert.Equal(default, parsedCommand);
    }

    public static IEnumerable<(TelemetryCommand Command1, TelemetryCommand Command2)> EnumerateEqualityValues()
    {
        yield return (new(TelemetryCommandType.Stop), new(TelemetryCommandType.Stop));
        yield return (new(TelemetryCommandType.Start), new(TelemetryCommandType.Start));
        yield return (default, default);
    }

    public static IEnumerable<(TelemetryCommand Command1, TelemetryCommand Command2)> EnumerateInequalityValues()
    {
        yield return (new(TelemetryCommandType.Start), new(TelemetryCommandType.Restart));
        yield return (new(TelemetryCommandType.Restart), default);
    }

    public static IEnumerable<TelemetryCommand> EnumerateValidCommands()
    {
        yield return new(TelemetryCommandType.Stop);
        yield return new(TelemetryCommandType.Start);
        yield return new(TelemetryCommandType.Restart);
    }
}
