using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static SharpRacer.SourceGenerators.Syntax.SyntaxFactoryHelpers;
namespace SharpRacer.SourceGenerators.Syntax;

public static class PropertyDeclarationSyntaxExtensions
{
    public static PropertyDeclarationSyntax WithGetOnlyAutoAccessor(this PropertyDeclarationSyntax syntaxNode)
    {
        if (syntaxNode is null)
        {
            throw new ArgumentNullException(nameof(syntaxNode));
        }

        var accessor = AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        return syntaxNode.WithAccessorList(AccessorList(SingletonList(accessor)));
    }

    public static PropertyDeclarationSyntax WithModifiers(this PropertyDeclarationSyntax syntaxNode, Accessibility accessibility)
    {
        return syntaxNode.WithModifiers(accessibility, isStatic: false);
    }

    public static PropertyDeclarationSyntax WithModifiers(this PropertyDeclarationSyntax syntaxNode, Accessibility accessibility, bool isStatic)
    {
        if (syntaxNode is null)
        {
            throw new ArgumentNullException(nameof(syntaxNode));
        }

        var tokens = ModifiersFromAccessibility(accessibility).ToList();

        if (isStatic)
        {
            tokens.Add(Token(SyntaxKind.StaticKeyword));
        }

        return syntaxNode.WithModifiers(TokenList(tokens));
    }
}
