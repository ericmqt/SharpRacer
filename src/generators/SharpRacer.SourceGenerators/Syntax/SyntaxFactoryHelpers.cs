using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.Syntax;

internal static class SyntaxFactoryHelpers
{
    public static AttributeSyntax GeneratedCodeAttribute()
        => GeneratedCodeAttribute(TelemetryVariablesGenerator.ToolName, TelemetryVariablesGenerator.ToolVersion);

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

        return Attribute(TypeIdentifierSyntaxFactory.QualifiedTypeName(SystemIdentifiers.GeneratedCodeAttribute))
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

    public static IfStatementSyntax NullCheck(string identifier, TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            throw new ArgumentException($"'{nameof(identifier)}' cannot be null or empty.", nameof(identifier));
        }

        var throwParamNameArg = Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(identifier)));

        var throwStatement = ThrowStatement(
            ObjectCreationExpression(SystemIdentifiers.ArgumentNullException.ToTypeSyntax(typeNameFormat))
                .WithArgumentList(
                    ArgumentList(SingletonSeparatedList(throwParamNameArg))));

        return IfStatement(
            IsPatternExpression(
                IdentifierName(identifier),
                ConstantPattern(LiteralExpression(SyntaxKind.NullLiteralExpression))),
            Block(SingletonList<StatementSyntax>(throwStatement)));
    }

    public static IfStatementSyntax NullCheck(IdentifierNameSyntax identifier, TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        var throwParamNameArg = Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(identifier.Identifier.ValueText)));

        var throwStatement = ThrowStatement(
            ObjectCreationExpression(SystemIdentifiers.ArgumentNullException.ToTypeSyntax(typeNameFormat))
                .WithArgumentList(
                    ArgumentList(SingletonSeparatedList(throwParamNameArg))));

        return IfStatement(
            IsPatternExpression(
                identifier,
                ConstantPattern(LiteralExpression(SyntaxKind.NullLiteralExpression))),
            Block(SingletonList<StatementSyntax>(throwStatement)));
    }

    public static AttributeSyntax ObsoleteAttribute(string message)
    {
        var obsoleteAttributeType = TypeIdentifierSyntaxFactory.QualifiedTypeName(SystemIdentifiers.ObsoleteAttribute);

        var messageArgument = AttributeArgument(
            LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(message)));

        return Attribute(obsoleteAttributeType)
            .WithArgumentList(AttributeArgumentList(SingletonSeparatedList(messageArgument)));
    }

    public static AttributeSyntax ObsoleteAttribute(string message, bool error)
    {
        var obsoleteAttributeType = TypeIdentifierSyntaxFactory.QualifiedTypeName(SystemIdentifiers.ObsoleteAttribute);

        var messageArgument = AttributeArgument(
            LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(message)));

        var errorFlagArgument = AttributeArgument(
            LiteralExpression(error ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression))
                .WithNameColon(NameColon(IdentifierName("error")));

        var argumentList = AttributeArgumentList(
            SeparatedList<AttributeArgumentSyntax>(
                new SyntaxNodeOrToken[]
                {
                    messageArgument,
                    Token(SyntaxKind.CommaToken),
                    errorFlagArgument
                }));

        return Attribute(obsoleteAttributeType)
            .WithArgumentList(argumentList);
    }

    public static MemberAccessExpressionSyntax StaticMemberAccessGlobalQualified(
        TypeIdentifier typeIdentifier,
        SimpleNameSyntax memberIdentifier)
    {
        var identifierParts = typeIdentifier.Namespace.ToString()
            .Split('.')
            .Select(IdentifierName)
            .Concat([IdentifierName(typeIdentifier.TypeName)])
            .ToList();

        var last = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            AliasQualifiedName(
                IdentifierName(Token(SyntaxKind.GlobalKeyword)), identifierParts[0]),
            identifierParts[1]);

        foreach (var idPart in identifierParts.Skip(2))
        {
            last = MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                last,
                idPart);
        }

        return MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            last,
            memberIdentifier);
    }
}
