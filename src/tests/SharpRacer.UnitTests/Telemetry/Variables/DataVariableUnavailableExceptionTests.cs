namespace SharpRacer.Telemetry.Variables;
public class DataVariableUnavailableExceptionTests
{
    [Fact]
    public void Ctor_VariableName_Test()
    {
        var variableName = "Foo";

        var ex = new DataVariableUnavailableException(variableName);

        Assert.Equal(variableName, ex.VariableName);
    }

    [Fact]
    public void Ctor_VariableName_NullNameArgIsEmptyStringTest()
    {
        var ex = new DataVariableUnavailableException(variableName: null!);

        Assert.Equal(string.Empty, ex.VariableName);
    }

    [Fact]
    public void Ctor_VariableName_MessageTest()
    {
        var variableName = "Foo";
        var exMessage = $"Variable '{variableName}' is unavailable.";

        var ex = new DataVariableUnavailableException(variableName, exMessage);

        Assert.Equal(variableName, ex.VariableName);
        Assert.Equal(exMessage, ex.Message);
    }
}
