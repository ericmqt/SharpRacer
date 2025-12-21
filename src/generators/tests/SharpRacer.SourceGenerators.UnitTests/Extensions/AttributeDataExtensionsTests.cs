using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace SharpRacer.SourceGenerators.Extensions;

public class AttributeDataExtensionsTests
{
    [Fact]
    public void GetLocationTest()
    {
        var descriptorClass = @"
using SharpRacer.Telemetry.Variables;
namespace Test.Assembly;
[GenerateTelemetryVariableDescriptors]
public static partial class MyDescriptors { }";

        var syntaxTree = CSharpSyntaxTree.ParseText(descriptorClass);

        var attributeSpan = new TextSpan(68, length: 36);
        var syntaxRef = new TestSyntaxReference(syntaxTree, attributeSpan);
        var attributeData = new TestAttributeData(syntaxRef);

        var location = attributeData.GetLocation();

        Assert.NotNull(location);
        Assert.Equal(syntaxTree, location.SourceTree);
        Assert.Equal(attributeSpan, location.SourceSpan);
    }

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
        public TestAttributeData()
        {

        }

        public TestAttributeData(TestSyntaxReference syntaxReference)
        {
            CommonApplicationSyntaxReference = syntaxReference;
        }

        protected override INamedTypeSymbol? CommonAttributeClass { get; }
        protected override IMethodSymbol? CommonAttributeConstructor { get; }
        protected override SyntaxReference? CommonApplicationSyntaxReference { get; }
        protected override ImmutableArray<TypedConstant> CommonConstructorArguments { get; }
        protected override ImmutableArray<KeyValuePair<string, TypedConstant>> CommonNamedArguments { get; }
    }

    private class TestSyntaxReference : SyntaxReference
    {
        public TestSyntaxReference(SyntaxTree syntaxTree, TextSpan span)
        {
            SyntaxTree = syntaxTree;
            Span = span;
        }

        public override SyntaxTree SyntaxTree { get; }
        public override TextSpan Span { get; }

        public override SyntaxNode GetSyntax(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
