namespace SharpRacer.Telemetry;

public class TelemetryVariableUnavailableExceptionTests
{
    [Fact]
    public void Ctor_VariableName_Test()
    {
        var variableName = "Foo";

        var ex = new TelemetryVariableUnavailableException(variableName);

        Assert.Equal(variableName, ex.VariableName);
    }

    [Fact]
    public void Ctor_VariableName_NullNameArgIsEmptyStringTest()
    {
        var ex = new TelemetryVariableUnavailableException(variableName: null!);

        Assert.Equal(string.Empty, ex.VariableName);
    }

    [Fact]
    public void Ctor_VariableName_MessageTest()
    {
        var variableName = "Foo";
        var exMessage = $"Variable '{variableName}' is unavailable.";

        var ex = new TelemetryVariableUnavailableException(variableName, exMessage);

        Assert.Equal(variableName, ex.VariableName);
        Assert.Equal(exMessage, ex.Message);
    }
}
