using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.Syntax;

internal static class SyntaxFactoryHelpers
{
    public static AttributeSyntax GeneratedCodeAttribute()
        => GeneratedCodeAttribute("SharpRacer.SourceGenerators", "0.1.0.0");

    public static AttributeSyntax GeneratedCodeAttribute(string tool, string version)
    {
        var toolArg = AttributeArgument(
            LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(tool)));

        var versionArg = AttributeArgument(
            LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(version)));

        var attributeArgs = new List<AttributeArgumentSyntax>()
        {
            toolArg,
            versionArg
        };

        var argumentList = AttributeArgumentList(SeparatedList(attributeArgs));

        return Attribute(ParseName("System.CodeDom.Compiler.GeneratedCodeAttribute"))
            .WithArgumentList(argumentList);
    }

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

    public static IfStatementSyntax NullCheck(string identifier)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            throw new ArgumentException($"'{nameof(identifier)}' cannot be null or empty.", nameof(identifier));
        }

        var throwParamNameArg = Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(identifier)));

        var throwStatement = ThrowStatement(
            ObjectCreationExpression(IdentifierName("ArgumentNullException"))
                .WithArgumentList(
                    ArgumentList(SingletonSeparatedList(throwParamNameArg))));

        return IfStatement(
            IsPatternExpression(
                IdentifierName(identifier),
                ConstantPattern(LiteralExpression(SyntaxKind.NullLiteralExpression))),
            Block(SingletonList<StatementSyntax>(throwStatement)));
    }

    public static IfStatementSyntax NullCheck(IdentifierNameSyntax identifier)
    {
        var throwParamNameArg = Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(identifier.Identifier.ValueText)));

        var throwStatement = ThrowStatement(
            ObjectCreationExpression(IdentifierName("ArgumentNullException"))
                .WithArgumentList(
                    ArgumentList(SingletonSeparatedList(throwParamNameArg))));

        return IfStatement(
            IsPatternExpression(
                identifier,
                ConstantPattern(LiteralExpression(SyntaxKind.NullLiteralExpression))),
            Block(SingletonList<StatementSyntax>(throwStatement)));
    }
}
