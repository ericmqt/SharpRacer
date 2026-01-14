namespace SharpRacer.Commands.Chat;

public class ReplyToLastPrivateMessageCommandTests :
    ChatCommandUnitTests<ReplyToLastPrivateMessageCommand, ReplyToLastPrivateMessageCommandTests>,
    IChatCommandUnitTests<ReplyToLastPrivateMessageCommand>
{
    public static ChatCommandType ChatCommandType { get; } = ChatCommandType.ReplyToLastPrivateMessage;
    public static SimulatorCommandId CommandId { get; } = SimulatorCommandId.ChatCommand;

    [Fact]
    public void ToCommandMessage_Test()
    {
        var command = new ReplyToLastPrivateMessageCommand();

        CommandMessageAssert.Arg1Equals(ChatCommandType, command);
        CommandMessageAssert.Arg2Empty(command);
        CommandMessageAssert.Arg3Empty(command);
    }

    public static IEnumerable<(ReplyToLastPrivateMessageCommand Command1, ReplyToLastPrivateMessageCommand Command2)> EnumerateEqualityValues()
    {
        yield return (new(), new());
        yield return (default, default);
    }

    public static IEnumerable<(ReplyToLastPrivateMessageCommand Command1, ReplyToLastPrivateMessageCommand Command2)> EnumerateInequalityValues()
    {
        // No testable values. ReplyToLastPrivateMessageCommand takes no parameters nor holds any comparable values and it is equal to its
        // default instance.
        yield break;
    }

    public static IEnumerable<ReplyToLastPrivateMessageCommand> EnumerateValidCommands()
    {
        yield return new();
    }


}
