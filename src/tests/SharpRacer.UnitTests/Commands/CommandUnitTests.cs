using SharpRacer.Extensions.Xunit;

namespace SharpRacer.Commands;

public abstract class CommandUnitTests<TCommand, TSelf>
    where TCommand : struct, ISimulatorCommand<TCommand>
    where TSelf : class, ICommandUnitTests<TCommand>
{
    public static TheoryData<TCommand, TCommand> EqualityTestData { get; } = GetEqualityTestData();
    public static TheoryData<TCommand, TCommand> InequalityTestData { get; } = GetInequalityTestData();

    [Fact]
    public void CommandId_Test()
    {
        Assert.Equal((ushort)TSelf.CommandId, TCommand.CommandId);
    }

    [Fact]
    public void Equals_Test()
    {
        foreach (var (Command1, Command2) in TSelf.EnumerateEqualityValues())
        {
            EquatableStructAssert.Equal(Command1, Command2);
        }
    }

    [Fact]
    public void Equals_InequalityTest()
    {
        foreach (var (Command1, Command2) in TSelf.EnumerateInequalityValues())
        {
            EquatableStructAssert.NotEqual(Command1, Command2);
        }
    }

    [Fact]
    public void Equals_ObjectInequalityTest()
    {
        foreach (var command1 in TSelf.EnumerateValidCommands())
        {
            var command2 = new object();

            EquatableStructAssert.ObjectEqualsMethod(false, command1, command2);
        }
    }

    [Fact]
    public void ImplicitConversionOp_CommandMessageTest()
    {
        foreach (var command in TSelf.EnumerateValidCommands())
        {
            Assert.Equal(command.ToCommandMessage(), (CommandMessage)command);
        }
    }

    [Fact]
    public void Parse_Test()
    {
        foreach (var command in TSelf.EnumerateValidCommands())
        {
            var message = command.ToCommandMessage();

            var parsedCommand = TCommand.Parse(message);

            Assert.Equal(command, parsedCommand);
            Assert.Equal(message, (CommandMessage)parsedCommand);
        }
    }

    [Fact]
    public void Parse_ThrowIfCommandIdNotValidTest()
    {
        const ushort invalidCommandId = (ushort)SimulatorCommandId.Unused;

        var command = TSelf.EnumerateValidCommands().First();
        var validMessage = command.ToCommandMessage();

        var msg = new CommandMessage(invalidCommandId, validMessage.Arg1, validMessage.Arg2, validMessage.Arg3);

        Assert.Throws<CommandMessageParseException>(() => TCommand.Parse(msg));
    }

    [Fact]
    public void TryParse_Test()
    {
        foreach (var command in TSelf.EnumerateValidCommands())
        {
            var message = command.ToCommandMessage();

            Assert.True(TCommand.TryParse(message, out var parsedCommand));

            Assert.Equal(command, parsedCommand);
            Assert.Equal(message, parsedCommand.ToCommandMessage());
        }
    }

    [Fact]
    public void TryParse_ReturnFalseIfCommandIdNotValidTest()
    {
        const ushort invalidCommandId = (ushort)SimulatorCommandId.Unused;

        var command = TSelf.EnumerateValidCommands().First();
        var validMessage = command.ToCommandMessage();

        var msg = new CommandMessage(invalidCommandId, validMessage.Arg1, validMessage.Arg2, validMessage.Arg3);

        Assert.False(TCommand.TryParse(msg, out var result));
        Assert.Equal(default, result);
    }

    protected static CommandMessage ModifyValidCommandMessage(
        ushort? commandId = null,
        ushort? arg1 = null,
        ushort? arg2 = null,
        ushort? arg3 = null)
    {
        var msg = TSelf.EnumerateValidCommands().First().ToCommandMessage();

        return new CommandMessage(
                commandId ?? msg.CommandId,
                arg1 ?? msg.Arg1,
                arg2 ?? msg.Arg2,
                arg3 ?? msg.Arg3
            );
    }

    private static TheoryData<TCommand, TCommand> GetEqualityTestData()
    {
        var data = new TheoryData<TCommand, TCommand>();

        foreach (var (Command1, Command2) in TSelf.EnumerateEqualityValues())
        {
            data.Add(Command1, Command2);
        }

        return data;
    }

    private static TheoryData<TCommand, TCommand> GetInequalityTestData()
    {
        var data = new TheoryData<TCommand, TCommand>();

        foreach (var (Command1, Command2) in TSelf.EnumerateInequalityValues())
        {
            data.Add(Command1, Command2);
        }

        return data;
    }
}
