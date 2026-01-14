namespace SharpRacer.Telemetry;

public class TelemetryVariableInfoCollectionExtensionsTests
{
    [Fact]
    public void FindByName_Test()
    {
        var fooVar = TelemetryVariableInfoFactory.CreateScalar("Foo", TelemetryVariableValueType.Int);
        var barVar = TelemetryVariableInfoFactory.CreateArray("Bar", TelemetryVariableValueType.Float, 3);

        List<TelemetryVariableInfo> variables = [fooVar, barVar];

        Assert.Equal(fooVar, variables.FindByName("Foo"));

        // Ensure default search is case-sensitive
        Assert.Null(variables.FindByName("foo"));
    }

    [Fact]
    public void FindByName_StringComparisonTest()
    {
        var fooVar = TelemetryVariableInfoFactory.CreateScalar("foo", TelemetryVariableValueType.Double);
        var barVar = TelemetryVariableInfoFactory.CreateArray("Bar", TelemetryVariableValueType.Float, 3);

        List<TelemetryVariableInfo> variables = [fooVar, barVar];

        // Use differently cased version of variable name
        Assert.Equal(fooVar, variables.FindByName("Foo", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void TryFindByName_Test()
    {
        var fooVar = TelemetryVariableInfoFactory.CreateScalar("Foo", TelemetryVariableValueType.Int);
        var barVar = TelemetryVariableInfoFactory.CreateArray("Bar", TelemetryVariableValueType.Float, 3);

        List<TelemetryVariableInfo> variables = [fooVar, barVar];

        Assert.True(variables.TryFindByName("Foo", out var result));
        Assert.Equal(fooVar, result);

        // Ensure default search is case-sensitive
        Assert.False(variables.TryFindByName("foo", out var nullResult));
        Assert.Null(nullResult);
    }

    [Fact]
    public void TryFindByName_StringComparisonTest()
    {
        var fooVar = TelemetryVariableInfoFactory.CreateScalar("foo", TelemetryVariableValueType.Int);
        var barVar = TelemetryVariableInfoFactory.CreateArray("bar", TelemetryVariableValueType.Float, 3);

        List<TelemetryVariableInfo> variables = [fooVar, barVar];

        // Use differently cased version of variable name
        Assert.True(variables.TryFindByName("Foo", StringComparison.OrdinalIgnoreCase, out var caseInsensitiveResult));
        Assert.Equal(fooVar, caseInsensitiveResult);

        Assert.True(variables.TryFindByName("foo", StringComparison.OrdinalIgnoreCase, out var exactMatchResult));
        Assert.Equal(fooVar, exactMatchResult);

        Assert.False(variables.TryFindByName("xyz", StringComparison.OrdinalIgnoreCase, out var nullResult));
        Assert.Null(nullResult);
    }
}
