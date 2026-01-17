namespace SharpRacer.Commands.Chat;

public class MacroCommandTests : ChatCommandUnitTests<MacroCommand, MacroCommandTests>, IChatCommandUnitTests<MacroCommand>
{
    public static SimulatorCommandId CommandId { get; } = SimulatorCommandId.Chat;
    public static ChatCommandType ChatCommandType { get; } = ChatCommandType.Macro;

    [Fact]
    public void Ctor_Test()
    {
        const ushort macroId = 3;

        var command = new MacroCommand(macroId);

        Assert.Equal(macroId, command.MacroId);
    }

    [Fact]
    public void Ctor_ThrowIfMacroIdInvalidTest()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new MacroCommand(MacroCommand.MinMacroId - 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => new MacroCommand(MacroCommand.MaxMacroId + 1));
    }

    [Fact]
    public void ToCommandMessage_Test()
    {
        const ushort macroId = 3;

        var msg = new MacroCommand(macroId).ToCommandMessage();

        CommandMessageAssert.Arg1Equals(ChatCommandType, msg);
        CommandMessageAssert.Arg2Equals(macroId, msg);
        CommandMessageAssert.Arg3Empty(msg);
    }

    [Fact]
    public void Parse_ThrowIfMacroIdNotValidTest()
    {
        var minMsg = new CommandMessage(MacroCommand.CommandId, (ushort)ChatCommandType, MacroCommand.MinMacroId - 1);
        var maxMsg = new CommandMessage(MacroCommand.CommandId, (ushort)ChatCommandType, MacroCommand.MaxMacroId + 1);

        Assert.Throws<CommandMessageParseException>(() => MacroCommand.Parse(minMsg));
        Assert.Throws<CommandMessageParseException>(() => MacroCommand.Parse(maxMsg));
    }

    [Fact]
    public void TryParse_ReturnFalseIfMacroIdNotValidTest()
    {
        var minMsg = new CommandMessage(MacroCommand.CommandId, (ushort)ChatCommandType, MacroCommand.MinMacroId - 1);

        Assert.False(MacroCommand.TryParse(minMsg, out var parsedCommand));
        Assert.Equal(default, parsedCommand);

        var maxMsg = new CommandMessage(MacroCommand.CommandId, (ushort)ChatCommandType, MacroCommand.MaxMacroId + 1);

        Assert.False(MacroCommand.TryParse(maxMsg, out parsedCommand));
        Assert.Equal(default, parsedCommand);
    }

    public static IEnumerable<(MacroCommand Command1, MacroCommand Command2)> EnumerateEqualityValues()
    {
        yield return (new(MacroCommand.MinMacroId), new(MacroCommand.MinMacroId));
        yield return (new(8), new(8));
        yield return (new(MacroCommand.MaxMacroId), new(MacroCommand.MaxMacroId));
        yield return (default, default);
    }

    public static IEnumerable<(MacroCommand Command1, MacroCommand Command2)> EnumerateInequalityValues()
    {
        yield return (new(), new(1));
        yield return (new(4), new(6));
        yield return (default, new(1));
    }

    public static IEnumerable<MacroCommand> EnumerateValidCommands()
    {
        for (int i = MacroCommand.MinMacroId; i <= MacroCommand.MaxMacroId; i++)
        {
            yield return new MacroCommand(i);
        }
    }
}
