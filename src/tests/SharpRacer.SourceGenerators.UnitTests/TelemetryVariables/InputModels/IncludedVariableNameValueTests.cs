using Microsoft.CodeAnalysis.Text;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
public class IncludedVariableNameValueTests
{
    [Fact]
    public void Ctor_StringTextSpanTest()
    {
        var variableName = "Test";
        var textSpan = new TextSpan(32, 64);

        var nameValue = new IncludedVariableNameValue(variableName, textSpan);

        Assert.Equal(variableName, nameValue.Value);
        Assert.Equal(textSpan, nameValue.ValueSpan);
    }

    [Fact]
    public void Equals_Test()
    {
        var variableName = "Test";
        var textSpan = new TextSpan(32, 64);

        var nameValue1 = new IncludedVariableNameValue(variableName, textSpan);
        var nameValue2 = new IncludedVariableNameValue(variableName, textSpan);

        EquatableStructAssert.Equal(nameValue1, nameValue2);
        EquatableStructAssert.ObjectEqualsMethod(false, nameValue1, DateTime.MinValue);
    }
}
