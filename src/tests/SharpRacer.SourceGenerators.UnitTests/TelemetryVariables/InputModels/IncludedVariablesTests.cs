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

        Assert.True(includes1 == includes2);
        Assert.False(includes1 != includes2);

        Assert.True(includes1.Equals(includes2));
        Assert.True(includes2.Equals(includes1));
        Assert.True(includes1.Equals((object)includes2));

        Assert.Equal(includes1.GetHashCode(), includes2.GetHashCode());
    }

    [Fact]
    public void Equals_DefaultTest()
    {
        var includes1 = new IncludedVariables([
           new IncludedVariableName("Test", Location.None),
            new IncludedVariableName("Test2", Location.None)
        ]);

        var includes2 = default(IncludedVariables);

        Assert.False(includes1 == includes2);
        Assert.True(includes1 != includes2);

        Assert.False(includes1.Equals(includes2));
        Assert.False(includes2.Equals(includes1));
        Assert.False(includes1.Equals((object)includes2));

        Assert.NotEqual(includes1.GetHashCode(), includes2.GetHashCode());
    }
}
