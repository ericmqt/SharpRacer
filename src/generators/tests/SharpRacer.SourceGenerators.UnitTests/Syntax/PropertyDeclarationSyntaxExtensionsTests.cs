using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
namespace SharpRacer.SourceGenerators.Syntax;
public class PropertyDeclarationSyntaxExtensionsTests
{
    [Fact]
    public void WithGetOnlyAutoAccessorTest()
    {
        var decl = PropertyDeclaration(ParseTypeName("object"), Identifier("MyProperty")).WithGetOnlyAutoAccessor();

        var declString = decl.NormalizeWhitespace().ToFullString();

        Assert.Equal("object MyProperty { get; }", declString);
    }

    [Fact]
    public void WithGetOnlyAutoAccessor_ThrowOnNullSyntaxNodeTest()
    {
        PropertyDeclarationSyntax node = null!;

        Assert.Throws<ArgumentNullException>(() => node.WithGetOnlyAutoAccessor());
    }

    [Fact]
    public void WithModifiers_AccessibilityTest()
    {
        var decl = PropertyDeclaration(ParseTypeName("object"), Identifier("MyProperty"))
            .WithModifiers(Accessibility.Public)
            .WithGetOnlyAutoAccessor();

        var declString = decl.NormalizeWhitespace().ToFullString();

        Assert.Equal("public object MyProperty { get; }", declString);
    }

    [Fact]
    public void WithModifiers_AccessibilityAndStaticTest()
    {
        var decl = PropertyDeclaration(ParseTypeName("object"), Identifier("MyProperty"))
            .WithModifiers(Accessibility.Public, isStatic: true)
            .WithGetOnlyAutoAccessor();

        var declString = decl.NormalizeWhitespace().ToFullString();

        Assert.Equal("public static object MyProperty { get; }", declString);
    }

    [Fact]
    public void WithModifiers_ThrowOnNullNodeArg()
    {
        PropertyDeclarationSyntax node = null!;

        Assert.Throws<ArgumentNullException>(() => node.WithModifiers(Accessibility.Public, false));
    }
}
