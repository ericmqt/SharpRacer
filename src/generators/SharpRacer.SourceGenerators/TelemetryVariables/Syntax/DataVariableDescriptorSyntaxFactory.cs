using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Syntax;

internal static class DataVariableDescriptorSyntaxFactory
{
    internal static ObjectCreationExpressionSyntax CreateNewInstanceExpression(string variableName, VariableValueType valueType, int valueCount)
    {
        var argumentList = ArgumentList(
            SeparatedList(
                new ArgumentSyntax[]
                {
                    Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(variableName))),
                    Argument(DataVariableValueTypeSyntaxFactory.EnumMemberAccessExpression(valueType, TypeNameFormat.GlobalQualified)),
                    Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(valueCount)))
                }));

        return ObjectCreationExpression(SharpRacerTypes.DataVariableDescriptor(TypeNameFormat.GlobalQualified))
            .WithArgumentList(argumentList);
    }

    internal static ImplicitObjectCreationExpressionSyntax ImplicitNewInstanceExpression(string variableName, VariableValueType valueType, int valueCount)
    {
        var argumentList = ArgumentList(
            SeparatedList(
                new ArgumentSyntax[]
                {
                    Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(variableName))),
                    Argument(DataVariableValueTypeSyntaxFactory.EnumMemberAccessExpression(valueType, TypeNameFormat.GlobalQualified)),
                    Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(valueCount)))
                }));

        return ImplicitObjectCreationExpression().WithArgumentList(argumentList);
    }

    internal static InvocationExpressionSyntax StaticFactoryMethodInvocation(
        string variableName,
        VariableValueType variableValueType,
        int valueCount,
        string? valueUnit)
    {
        var methodTypeArg = SharpRacerTypes.DataVariableTypeArgument(variableValueType, valueUnit, TypeNameFormat.GlobalQualified);
        var variableNameArg = Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(variableName)));

        GenericNameSyntax methodName;
        ArgumentListSyntax methodArgs;

        if (valueCount > 1)
        {
            methodName = GenericName(Identifier("CreateArray"), TypeArgumentList(SingletonSeparatedList(methodTypeArg)));

            methodArgs = ArgumentList(
                SeparatedList<ArgumentSyntax>(
                    new SyntaxNodeOrToken[]
                    {
                        variableNameArg,
                        Token(SyntaxKind.CommaToken),
                        Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(valueCount)))
                    }));
        }
        else
        {
            methodName = GenericName(Identifier("CreateScalar"), TypeArgumentList(SingletonSeparatedList(methodTypeArg)));

            methodArgs = ArgumentList(SingletonSeparatedList(variableNameArg));
        }

        var methodAccessExpr = SyntaxFactoryHelpers.StaticMemberAccessGlobalQualified(SharpRacerIdentifiers.DataVariableDescriptor, methodName);

        return InvocationExpression(
            methodAccessExpr,
            methodArgs);
    }
}
