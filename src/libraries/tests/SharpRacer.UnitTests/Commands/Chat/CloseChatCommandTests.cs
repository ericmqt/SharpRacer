namespace SharpRacer.Commands.Chat;

public class CloseChatCommandTests : ChatCommandUnitTests<CloseChatCommand, CloseChatCommandTests>, IChatCommandUnitTests<CloseChatCommand>
{
    public static ChatCommandType ChatCommandType { get; } = ChatCommandType.CloseChat;
    public static SimulatorCommandId CommandId { get; } = SimulatorCommandId.Chat;

    [Fact]
    public void ToCommandMessage_Test()
    {
        var command = new CloseChatCommand();

        CommandMessageAssert.Arg1Equals(ChatCommandType, command);
        CommandMessageAssert.Arg2Empty(command);
        CommandMessageAssert.Arg3Empty(command);
    }

    public static IEnumerable<(CloseChatCommand Command1, CloseChatCommand Command2)> EnumerateEqualityValues()
    {
        yield return (new(), new());
        yield return (default, default);
    }

    public static IEnumerable<(CloseChatCommand Command1, CloseChatCommand Command2)> EnumerateInequalityValues()
    {
        // No testable values. CloseChatCommand takes no parameters nor holds any comparable values and it is equal to its default instance.
        yield break;
    }

    public static IEnumerable<CloseChatCommand> EnumerateValidCommands()
    {
        yield return new();
    }
}
