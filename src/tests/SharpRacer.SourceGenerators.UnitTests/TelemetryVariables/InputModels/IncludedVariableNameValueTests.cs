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

        Assert.True(nameValue1 == nameValue2);
        Assert.False(nameValue1 != nameValue2);
        Assert.True(nameValue1.Equals(nameValue2));
        Assert.Equal(nameValue1.GetHashCode(), nameValue2.GetHashCode());

        Assert.True(nameValue1.Equals((object)nameValue2));
    }
}
