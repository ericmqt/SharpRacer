using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators.TelemetryVariables;
public class GenerateDataVariablesContextAttributeInfoTests
{
    [Fact]
    public void GetIncludedVariablesFileNameOrDefault_ReturnsDefaultOnZeroConstructorArgsTest()
    {
        var attributeData = new TestAttributeData();

        var result = GenerateDataVariablesContextAttributeInfo.GetIncludedVariablesFileNameOrDefault(attributeData);

        Assert.Equal(default, result);
    }

    private class TestAttributeData : AttributeData
    {
        public TestAttributeData()
        {
            CommonConstructorArguments = ImmutableArray<TypedConstant>.Empty;
        }

        protected override INamedTypeSymbol? CommonAttributeClass { get; }
        protected override IMethodSymbol? CommonAttributeConstructor { get; }
        protected override SyntaxReference? CommonApplicationSyntaxReference { get; }
        protected override ImmutableArray<TypedConstant> CommonConstructorArguments { get; }
        protected override ImmutableArray<KeyValuePair<string, TypedConstant>> CommonNamedArguments { get; }
    }
}
