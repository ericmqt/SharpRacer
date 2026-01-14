namespace SharpRacer.Commands.Chat;

public class OpenChatCommandTests : ChatCommandUnitTests<OpenChatCommand, OpenChatCommandTests>, IChatCommandUnitTests<OpenChatCommand>
{
    public static ChatCommandType ChatCommandType { get; } = ChatCommandType.OpenChat;
    public static SimulatorCommandId CommandId { get; } = SimulatorCommandId.ChatCommand;

    [Fact]
    public void ToCommandMessage_Test()
    {
        var command = new OpenChatCommand();

        CommandMessageAssert.Arg1Equals(ChatCommandType, command);
        CommandMessageAssert.Arg2Empty(command);
        CommandMessageAssert.Arg3Empty(command);
    }

    public static IEnumerable<(OpenChatCommand Command1, OpenChatCommand Command2)> EnumerateEqualityValues()
    {
        yield return (new(), new());
        yield return (default, default);
    }

    public static IEnumerable<(OpenChatCommand Command1, OpenChatCommand Command2)> EnumerateInequalityValues()
    {
        // No testable values. OpenChatCommand takes no parameters nor holds any comparable values and it is equal to its default instance.
        yield break;
    }

    public static IEnumerable<OpenChatCommand> EnumerateValidCommands()
    {
        yield return new();
    }
}
