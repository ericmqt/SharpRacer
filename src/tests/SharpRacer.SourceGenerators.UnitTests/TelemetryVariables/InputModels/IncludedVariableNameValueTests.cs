using Microsoft.CodeAnalysis.Text;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
public class IncludedVariableNameValueTests
{
    [Fact]
    public void Ctor_Test()
    {
        var variableName = "Test";
        var nameValue = new IncludedVariableNameValue(variableName);

        Assert.Equal(variableName, nameValue.Value);
        Assert.Equal(default, nameValue.ValueSpan);
    }

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
    public void Ctor_ThrowOnNullButNotEmptyStringTest()
    {
        Assert.Throws<ArgumentNullException>(() => new IncludedVariableNameValue(variableName: null!, new TextSpan(0, 0)));
    }

    [Fact]
    public void Equals_Test()
    {
        var variableName = "Test";
        var textSpan = new TextSpan(32, 64);

        var nameValue1 = new IncludedVariableNameValue(variableName, textSpan);
        var nameValue2 = new IncludedVariableNameValue(variableName, textSpan);

        EquatableStructAssert.Equal(nameValue1, nameValue2);
    }

    [Fact]
    public void Equals_DefaultValueTest()
    {
        var nameValue1 = new IncludedVariableNameValue("Test", new TextSpan(32, 64));

        EquatableStructAssert.NotEqual(nameValue1, default);
    }

    [Theory]
    [MemberData(nameof(GetInequalityData))]
    public void Equals_InequalityTest(IncludedVariableNameValue value1, IncludedVariableNameValue value2)
    {
        EquatableStructAssert.NotEqual(value1, value2);
    }

    [Fact]
    public void EqualsObject_WrongObjectTypeTest()
    {
        var nameValue1 = new IncludedVariableNameValue("Test", new TextSpan(32, 64));

        EquatableStructAssert.ObjectEqualsMethod(false, nameValue1, DateTime.MinValue);
    }

    public static TheoryData<IncludedVariableNameValue, IncludedVariableNameValue> GetInequalityData()
    {
        return new TheoryData<IncludedVariableNameValue, IncludedVariableNameValue>()
        {
            // Value
            {
                new IncludedVariableNameValue("Test1", default),
                new IncludedVariableNameValue("Test2", default)
            },

            // ValueSpan
            {
                new IncludedVariableNameValue("Test", new TextSpan(12, 22)),
                new IncludedVariableNameValue("Test", new TextSpan(32, 64))
            }
        };
    }
}
