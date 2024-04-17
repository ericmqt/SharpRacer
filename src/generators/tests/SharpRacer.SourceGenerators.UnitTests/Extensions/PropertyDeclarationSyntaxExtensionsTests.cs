using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.Syntax;

namespace SharpRacer.SourceGenerators.Extensions;
public class PropertyDeclarationSyntaxExtensionsTests
{
    [Fact]
    public void WithGetOnlyAutoAccessor_ThrowOnNullSyntaxNodeTest()
    {
        PropertyDeclarationSyntax syntaxNode = null!;

        Assert.Throws<ArgumentNullException>(() => syntaxNode.WithGetOnlyAutoAccessor());
    }

    [Fact]
    public void WithModifiers_ThrowOnNullSyntaxNodeTest()
    {
        PropertyDeclarationSyntax syntaxNode = null!;

        Assert.Throws<ArgumentNullException>(() => syntaxNode.WithModifiers(Accessibility.Public, false));
    }

}
