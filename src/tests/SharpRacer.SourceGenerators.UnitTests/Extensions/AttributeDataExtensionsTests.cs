using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators.Extensions;
public class AttributeDataExtensionsTests
{
    [Fact]
    public void NullAttributeDataReturnsLocationNoneTest()
    {
        AttributeData? attributeData = null;
        var location = attributeData!.GetLocation();

        Assert.Equal(Location.None, location);
    }

    [Fact]
    public void NullApplicationSyntaxReferenceReturnsLocationNoneTest()
    {
        var testData = new TestAttributeData();
        var location = testData.GetLocation();

        Assert.Equal(Location.None, location);
    }

    private class TestAttributeData : AttributeData
    {
        protected override INamedTypeSymbol? CommonAttributeClass { get; }
        protected override IMethodSymbol? CommonAttributeConstructor { get; }
        protected override SyntaxReference? CommonApplicationSyntaxReference { get; } = null;
        protected override ImmutableArray<TypedConstant> CommonConstructorArguments { get; }
        protected override ImmutableArray<KeyValuePair<string, TypedConstant>> CommonNamedArguments { get; }
    }
}
