using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static SharpRacer.SourceGenerators.Syntax.SyntaxFactoryHelpers;

namespace SharpRacer.SourceGenerators.Syntax;
public static class ClassDeclarationSyntaxExtensions
{
    public static bool HasAttributes(this ClassDeclarationSyntax classDeclarationSyntax)
    {
        return classDeclarationSyntax.AttributeLists.Count > 0;
    }

    public static bool IsPartialClass(this ClassDeclarationSyntax classDeclarationSyntax)
    {
        return classDeclarationSyntax.Modifiers.Any(x => x.IsKind(SyntaxKind.PartialKeyword));
    }

    public static bool IsStaticClass(this ClassDeclarationSyntax classDeclarationSyntax)
    {
        return classDeclarationSyntax.Modifiers.Any(x => x.IsKind(SyntaxKind.StaticKeyword));
    }

    public static bool IsStaticPartialClass(this ClassDeclarationSyntax classDeclarationSyntax)
    {
        return classDeclarationSyntax.Modifiers.Any(x => x.IsKind(SyntaxKind.StaticKeyword)) &&
            classDeclarationSyntax.Modifiers.Any(x => x.IsKind(SyntaxKind.PartialKeyword));
    }

    public static ClassDeclarationSyntax WithModifiers(this ClassDeclarationSyntax node, Accessibility accessibility, bool isStatic = false, bool isPartial = false)
    {
        if (node is null)
        {
            throw new ArgumentNullException(nameof(node));
        }

        var modifierTokens = new List<SyntaxToken>();

        modifierTokens.AddRange(ModifiersFromAccessibility(accessibility));

        if (isStatic)
        {
            modifierTokens.Add(Token(SyntaxKind.StaticKeyword));
        }

        if (isPartial)
        {
            modifierTokens.Add(Token(SyntaxKind.PartialKeyword));
        }

        return node.WithModifiers(TokenList(modifierTokens));
    }
}
