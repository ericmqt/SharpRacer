using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
                    Argument(DataVariableValueTypeSyntaxFactory.EnumMemberAccessExpression(valueType)),
                    Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(valueCount)))
                }));

        return ObjectCreationExpression(SharpRacerTypes.DataVariableDescriptor()).WithArgumentList(argumentList);
    }

    internal static ImplicitObjectCreationExpressionSyntax ImplicitNewInstanceExpression(string variableName, VariableValueType valueType, int valueCount)
    {
        var argumentList = ArgumentList(
            SeparatedList(
                new ArgumentSyntax[]
                {
                    Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(variableName))),
                    Argument(DataVariableValueTypeSyntaxFactory.EnumMemberAccessExpression(valueType)),
                    Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(valueCount)))
                }));

        return ImplicitObjectCreationExpression().WithArgumentList(argumentList);
    }
}
