using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.Syntax;

internal static class SyntaxFactoryHelpers
{
    public static IEnumerable<SyntaxToken> ModifiersFromAccessibility(Accessibility accessibility)
    {
        if (accessibility == Accessibility.Public)
        {
            yield return Token(SyntaxKind.PublicKeyword);
        }

        if (accessibility == Accessibility.Private)
        {
            yield return Token(SyntaxKind.PrivateKeyword);
        }

        if (accessibility == Accessibility.Internal)
        {
            yield return Token(SyntaxKind.InternalKeyword);
        }

        if (accessibility == Accessibility.ProtectedAndInternal)
        {
            yield return Token(SyntaxKind.ProtectedKeyword);
            yield return Token(SyntaxKind.InternalKeyword);
        }

        if (accessibility == Accessibility.Protected)
        {
            yield return Token(SyntaxKind.ProtectedKeyword);
        }
    }
}
