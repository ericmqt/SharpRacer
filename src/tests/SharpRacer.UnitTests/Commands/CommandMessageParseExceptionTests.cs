namespace SharpRacer.Commands;

public class CommandMessageParseExceptionTests
{
    [Fact]
    public void Ctor_MessageTest()
    {
        var message = "Error!";

        var ex = new CommandMessageParseException(message);

        Assert.Equal(message, ex.Message);
        Assert.Null(ex.InnerException);
    }

    [Fact]
    public void Ctor_NullInnerExceptionTest()
    {
        var message = "Error!";

        var ex = new CommandMessageParseException(message, null);
        Assert.Equal(message, ex.Message);
        Assert.Null(ex.InnerException);

        ex = new CommandMessageParseException(null, null);
        Assert.NotNull(ex.Message);
        Assert.Null(ex.InnerException);
    }

    [Fact]
    public void Ctor_NullMessageTest()
    {
        var ex = new CommandMessageParseException(null);
        Assert.NotNull(ex.Message);
    }

    [Fact]
    public void ThrowIfCommandIdNotEqual_Test()
    {
        const ushort commandId = (ushort)SimulatorCommandId.CameraSwitchNumber;
        const ushort messageCommandId = (ushort)SimulatorCommandId.ReplaySearchSessionTime;

        var msg = new CommandMessage(messageCommandId);

        Assert.Throws<CommandMessageParseException>(() => CommandMessageParseException.ThrowIfCommandIdNotEqual(msg, commandId));
    }

    [Fact]
    public void ThrowIfCommandIdNotEqual_DoesNotThrowIfEqualTest()
    {
        const ushort commandId = (ushort)SimulatorCommandId.CameraSwitchNumber;
        const ushort messageCommandId = (ushort)SimulatorCommandId.CameraSwitchNumber;

        var msg = new CommandMessage(messageCommandId);

        CommandMessageParseException.ThrowIfCommandIdNotEqual(msg, commandId);
    }
}
