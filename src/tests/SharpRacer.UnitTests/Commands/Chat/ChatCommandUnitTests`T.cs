using SharpRacer.Extensions.Xunit.Utilities;

namespace SharpRacer.Commands.Chat;

public abstract class ChatCommandUnitTests<TCommand, TSelf> : CommandUnitTests<TCommand, TSelf>
    where TCommand : struct, ISimulatorCommand<TCommand>
    where TSelf : class, IChatCommandUnitTests<TCommand>
{
    public static TheoryData<ChatCommandType> InvalidChatCommandTypesTestData { get; } = GetInvalidChatCommandTypesTestData();

    [Theory]
    [MemberData(nameof(InvalidChatCommandTypesTestData))]
    public void Parse_ThrowIfChatCommandTypeNotValidTest(ChatCommandType chatCommandType)
    {
        // Get a valid value and convert it to a CommandMessage so we can reuse its Arg2 and Arg3 values. Otherwise, we can't really
        // say anything about what values are valid for Arg2 and Arg3 without knowing the command under test.
        var validMsg = TSelf.EnumerateValidCommands().First().ToCommandMessage();

        var msg = new CommandMessage((ushort)TSelf.CommandId, (ushort)chatCommandType, validMsg.Arg2, validMsg.Arg3);

        Assert.Throws<CommandMessageParseException>(() => TCommand.Parse(msg));
    }

    [Fact]
    public void Parse_ThrowIfChatCommandTypeUndefinedTest()
    {
        // Get a valid value and convert it to a CommandMessage so we can reuse its Arg2 and Arg3 values. Otherwise, we can't really
        // say anything about what values are valid for Arg2 and Arg3 without knowing the command under test.
        var validMsg = TSelf.EnumerateValidCommands().First().ToCommandMessage();

        var msg = new CommandMessage((ushort)TSelf.CommandId, (ushort)GetUndefinedChatCommandType(), validMsg.Arg2, validMsg.Arg3);

        Assert.Throws<CommandMessageParseException>(() => TCommand.Parse(msg));
    }

    [Theory]
    [MemberData(nameof(InvalidChatCommandTypesTestData))]
    public void TryParse_ReturnFalseIfChatCommandTypeNotValidTest(ChatCommandType chatCommandType)
    {
        // Get a valid value and convert it to a CommandMessage so we can reuse its Arg2 and Arg3 values. Otherwise, we can't really
        // say anything about what values are valid for Arg2 and Arg3 without knowing the command under test.
        var validMsg = TSelf.EnumerateValidCommands().First().ToCommandMessage();

        var msg = new CommandMessage((ushort)TSelf.CommandId, (ushort)chatCommandType, validMsg.Arg2, validMsg.Arg3);

        Assert.False(TCommand.TryParse(msg, out var parsedCommand));
        Assert.Equal(default, parsedCommand);
    }

    [Fact]
    public void TryParse_ReturnFalseIfChatCommandTypeUndefinedTest()
    {
        // Get a valid value and convert it to a CommandMessage so we can reuse its Arg2 and Arg3 values. Otherwise, we can't really
        // say anything about what values are valid for Arg2 and Arg3 without knowing the command under test.
        var validMsg = TSelf.EnumerateValidCommands().First().ToCommandMessage();

        var msg = new CommandMessage((ushort)TSelf.CommandId, (ushort)GetUndefinedChatCommandType(), validMsg.Arg2, validMsg.Arg3);

        Assert.False(TCommand.TryParse(msg, out var parsedCommand));
        Assert.Equal(default, parsedCommand);
    }

    private static TheoryData<ChatCommandType> GetInvalidChatCommandTypesTestData()
    {
        return new TheoryData<ChatCommandType>(Enum.GetValues<ChatCommandType>().Where(x => x != TSelf.ChatCommandType));
    }

    protected static ChatCommandType GetUndefinedChatCommandType()
    {
        var undefinedIntegralValue = EnumTestUtilities.MaxIntegralValue<ChatCommandType, ushort>() + 1;

        return (ChatCommandType)undefinedIntegralValue;
    }
}
