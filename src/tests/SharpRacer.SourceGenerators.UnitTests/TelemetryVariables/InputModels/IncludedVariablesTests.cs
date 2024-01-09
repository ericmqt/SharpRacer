using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
public class IncludedVariablesTests
{
    [Fact]
    public void Ctor_DefaultImmutableArrayCoercedToEmptyTest()
    {
        var includes = new IncludedVariables(default);

        Assert.False(includes.VariableNames.IsDefault);
        Assert.True(includes.VariableNames.IsEmpty);
    }

    [Fact]
    public void Default_IncludeAllTest()
    {
        IncludedVariables includedVariables = default;

        Assert.True(includedVariables.IncludeAll());
    }

    [Fact]
    public void Equals_Test()
    {
        var includes1 = new IncludedVariables([
            new IncludedVariableName("Test", Location.None),
            new IncludedVariableName("Test2", Location.None)
        ]);

        var includes2 = new IncludedVariables([
            new IncludedVariableName("Test", Location.None),
            new IncludedVariableName("Test2", Location.None)
        ]);

        EquatableStructAssert.Equal(includes1, includes2);
        EquatableStructAssert.ObjectEqualsMethod(false, includes1, int.MinValue);
    }

    [Fact]
    public void Equals_DefaultTest()
    {
        var includes1 = new IncludedVariables([
           new IncludedVariableName("Test", Location.None),
            new IncludedVariableName("Test2", Location.None)
        ]);

        EquatableStructAssert.NotEqual(includes1, default);
    }
}
