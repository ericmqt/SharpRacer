using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static SharpRacer.SourceGenerators.Syntax.SyntaxFactoryHelpers;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Syntax;

internal static class DataVariableTypeSyntaxFactory
{
    public static ObjectCreationExpressionSyntax ArrayDataVariableConstructor(
        TypeSyntax typeArgument,
        ExpressionSyntax descriptorArgumentExpression,
        IdentifierNameSyntax dataVariableInfoProviderIdentifier)
    {
        var ctorArgs = ArgumentList(
            SeparatedList<ArgumentSyntax>(
                new SyntaxNodeOrToken[]
                {
                    Argument(descriptorArgumentExpression),
                    Token(SyntaxKind.CommaToken),
                    Argument(dataVariableInfoProviderIdentifier)
                }));

        return ObjectCreationExpression(SharpRacerTypes.ArrayDataVariableType(typeArgument, TypeNameFormat.GlobalQualified))
            .WithArgumentList(ctorArgs);
    }

    public static ObjectCreationExpressionSyntax ScalarDataVariableConstructor(
        TypeSyntax typeArgument,
        ExpressionSyntax descriptorArgumentExpression,
        IdentifierNameSyntax dataVariableInfoProviderIdentifier)
    {
        var ctorArgs = ArgumentList(
            SeparatedList<ArgumentSyntax>(
                new SyntaxNodeOrToken[]
                {
                    Argument(descriptorArgumentExpression),
                    Token(SyntaxKind.CommaToken),
                    Argument(dataVariableInfoProviderIdentifier)
                }));

        return ObjectCreationExpression(SharpRacerTypes.ScalarDataVariableType(typeArgument, TypeNameFormat.GlobalQualified))
            .WithArgumentList(ctorArgs);
    }
}
