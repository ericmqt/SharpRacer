using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace SharpRacer.SourceGenerators.Syntax;
public class SyntaxFactoryHelpersTests
{
    [Fact]
    public void ModifiersFromAccessibility_InternalTest()
    {
        var tokens = SyntaxFactoryHelpers.ModifiersFromAccessibility(Accessibility.Internal).ToList();

        Assert.Single(tokens);
        Assert.Equal(SyntaxKind.InternalKeyword, tokens.Single().Kind());
    }

    [Fact]
    public void ModifiersFromAccessibility_PrivateTest()
    {
        var tokens = SyntaxFactoryHelpers.ModifiersFromAccessibility(Accessibility.Private).ToList();

        Assert.Single(tokens);
        Assert.Equal(SyntaxKind.PrivateKeyword, tokens.Single().Kind());
    }

    [Fact]
    public void ModifiersFromAccessibility_ProtectedTest()
    {
        var tokens = SyntaxFactoryHelpers.ModifiersFromAccessibility(Accessibility.Protected).ToList();

        Assert.Single(tokens);
        Assert.Equal(SyntaxKind.ProtectedKeyword, tokens.Single().Kind());
    }

    [Fact]
    public void ModifiersFromAccessibility_ProtectedInternalTest()
    {
        var tokens = SyntaxFactoryHelpers.ModifiersFromAccessibility(Accessibility.ProtectedAndInternal).ToList();

        Assert.Equal(2, tokens.Count);

        Assert.Contains(SyntaxKind.ProtectedKeyword, tokens.Select(x => x.Kind()));
        Assert.Contains(SyntaxKind.InternalKeyword, tokens.Select(x => x.Kind()));
    }

    [Fact]
    public void ModifiersFromAccessibility_PublicTest()
    {
        var tokens = SyntaxFactoryHelpers.ModifiersFromAccessibility(Accessibility.Public).ToList();

        Assert.Single(tokens);
        Assert.Equal(SyntaxKind.PublicKeyword, tokens.Single().Kind());
    }
}
