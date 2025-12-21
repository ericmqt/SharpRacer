using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Syntax;

internal static class TelemetryVariableTypeSyntaxFactory
{
    public static ObjectCreationExpressionSyntax ArrayTelemetryVariableConstructor(
        TypeSyntax typeArgument,
        ExpressionSyntax descriptorArgumentExpression,
        IdentifierNameSyntax variableInfoProviderIdentifier)
    {
        var ctorArgs = ArgumentList(
            SeparatedList<ArgumentSyntax>(
                new SyntaxNodeOrToken[]
                {
                    Argument(descriptorArgumentExpression),
                    Token(SyntaxKind.CommaToken),
                    Argument(variableInfoProviderIdentifier)
                }));

        return ObjectCreationExpression(SharpRacerTypes.ArrayTelemetryVariableType(typeArgument, TypeNameFormat.GlobalQualified))
            .WithArgumentList(ctorArgs);
    }

    public static ObjectCreationExpressionSyntax ScalarTelemetryVariableConstructor(
        TypeSyntax typeArgument,
        ExpressionSyntax descriptorArgumentExpression,
        IdentifierNameSyntax variableInfoProviderIdentifier)
    {
        var ctorArgs = ArgumentList(
            SeparatedList<ArgumentSyntax>(
                new SyntaxNodeOrToken[]
                {
                    Argument(descriptorArgumentExpression),
                    Token(SyntaxKind.CommaToken),
                    Argument(variableInfoProviderIdentifier)
                }));

        return ObjectCreationExpression(SharpRacerTypes.ScalarTelemetryVariableType(typeArgument, TypeNameFormat.GlobalQualified))
            .WithArgumentList(ctorArgs);
    }
}
