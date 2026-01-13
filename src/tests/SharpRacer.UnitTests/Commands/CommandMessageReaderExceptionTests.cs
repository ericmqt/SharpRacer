namespace SharpRacer.Commands;

public class CommandMessageReaderExceptionTests
{
    [Fact]
    public void Ctor_MessageTest()
    {
        var message = "Error!";

        var ex = new CommandMessageReaderException(message);

        Assert.Equal(message, ex.Message);
        Assert.Null(ex.InnerException);
    }

    [Fact]
    public void Ctor_NullInnerExceptionTest()
    {
        var message = "Error!";

        var ex = new CommandMessageReaderException(message, null);
        Assert.Equal(message, ex.Message);
        Assert.Null(ex.InnerException);

        ex = new CommandMessageReaderException(null, null);
        Assert.NotNull(ex.Message);
        Assert.Null(ex.InnerException);
    }

    [Fact]
    public void Ctor_NullMessageTest()
    {
        var ex = new CommandMessageReaderException(null);
        Assert.NotNull(ex.Message);
    }
}
