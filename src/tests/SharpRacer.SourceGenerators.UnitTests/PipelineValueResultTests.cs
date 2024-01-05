using System.Collections.Immutable;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;

namespace SharpRacer.SourceGenerators;
public class PipelineValueResultTests
{
    [Fact]
    public void Ctor_ParameterlessTest()
    {
        var result = new PipelineValueResult<int>();

        Assert.Empty(result.Diagnostics);
        Assert.False(result.HasErrors);
        Assert.False(result.HasValue);
    }

    [Fact]
    public void Ctor_ResultAndDiagnosticTest()
    {
        var result = new PipelineValueResult<int>(5, GeneratorDiagnostics.TelemetryVariablesFileNotFound("Test"));

        Assert.True(result.HasValue);
        Assert.True(result.HasErrors);
        Assert.Single(result.Diagnostics);
    }

    [Fact]
    public void Equals_HasValueTest()
    {
        PipelineValueResult<int> result1 = 4;
        PipelineValueResult<int> result2 = 4;

        Assert.True(result1 == result2);
        Assert.False(result1 != result2);
        Assert.True(result1.Equals(result2));
        Assert.Equal(result1.GetHashCode(), result2.GetHashCode());
    }

    [Fact]
    public void Equals_DefaultTest()
    {
        PipelineValueResult<int> result1 = 4;
        PipelineValueResult<int> result2 = default;

        Assert.False(result1 == result2);
        Assert.False(result2 == result1);

        Assert.True(result1 != result2);
        Assert.True(result2 != result1);

        Assert.False(result1.Equals(result2));
        Assert.False(result2.Equals(result1));

        Assert.NotEqual(result1.GetHashCode(), result2.GetHashCode());
    }

    [Fact]
    public void Equals_OtherNoValueTest()
    {
        PipelineValueResult<int> result1 = 4;
        PipelineValueResult<int> result2 = GeneratorDiagnostics.TelemetryVariablesFileNotFound("Test");

        Assert.False(result1 == result2);
        Assert.False(result2 == result1);

        Assert.True(result1 != result2);
        Assert.True(result2 != result1);

        Assert.False(result1.Equals(result2));
        Assert.False(result2.Equals(result1));

        Assert.NotEqual(result1.GetHashCode(), result2.GetHashCode());
    }

    [Fact]
    public void ImplicitConversionOperator_FromDiagnosticTest()
    {
        PipelineValueResult<int> result = GeneratorDiagnostics.TelemetryVariablesFileNotFound("Test");

        Assert.False(result.HasValue);
        Assert.True(result.HasErrors);
        Assert.Single(result.Diagnostics);
    }

    [Fact]
    public void ImplicitConversionOperator_FromDiagnosticArrayTest()
    {
        PipelineValueResult<int> result = ImmutableArray.Create(GeneratorDiagnostics.TelemetryVariablesFileNotFound("Test"));

        Assert.False(result.HasValue);
        Assert.True(result.HasErrors);
        Assert.Single(result.Diagnostics);
    }

    [Fact]
    public void ImplicitConversionOperator_FromResultTest()
    {
        PipelineValueResult<int> result = 4;

        Assert.True(result.HasValue);
        Assert.False(result.HasErrors);
        Assert.Empty(result.Diagnostics);
        Assert.Equal(4, result.Value);
    }
}
