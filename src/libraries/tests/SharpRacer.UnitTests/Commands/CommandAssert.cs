namespace SharpRacer.Commands;

public static class CommandAssert
{
    public static void CommandIdEqual<TCommand>(SimulatorCommandId commandId)
        where TCommand : ISimulatorCommand<TCommand>
    {
        CommandIdEqual<TCommand>((ushort)commandId);
    }

    public static void CommandIdEqual<TCommand>(ushort commandId)
        where TCommand : ISimulatorCommand<TCommand>
    {
        Assert.Equal(commandId, TCommand.CommandId);
    }

    public static CommandMessageParseException ParseThrows<TCommand>(CommandMessage commandMessage)
        where TCommand : ISimulatorCommand<TCommand>
    {
        return ParseThrows<TCommand, CommandMessageParseException>(commandMessage);
    }

    public static TException ParseThrows<TCommand, TException>(CommandMessage commandMessage)
        where TCommand : ISimulatorCommand<TCommand>
        where TException : Exception
    {
        return Assert.Throws<TException>(() => TCommand.Parse(commandMessage));
    }

    public static void TryParseFail<TCommand>(CommandMessage commandMessage)
        where TCommand : ISimulatorCommand<TCommand>
    {
        Assert.False(TCommand.TryParse(commandMessage, out var parseResult));
        Assert.Equal(parseResult, default);
    }

    public static void TryParseFail<TCommand>(CommandMessage commandMessage, out TCommand parseResult)
        where TCommand : ISimulatorCommand<TCommand>
    {
        Assert.False(TCommand.TryParse(commandMessage, out parseResult));
        Assert.Equal(parseResult, default);
    }
}
