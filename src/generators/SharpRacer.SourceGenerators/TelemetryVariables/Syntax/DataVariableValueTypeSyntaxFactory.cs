using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Syntax;
internal static class DataVariableValueTypeSyntaxFactory
{
    internal static MemberAccessExpressionSyntax EnumMemberAccessExpression(VariableValueType variableValueType)
    {
        return MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            SharpRacerTypes.DataVariableValueType(),
            MemberIdentifier(variableValueType));
    }

    internal static IdentifierNameSyntax MemberIdentifier(VariableValueType variableValueType)
    {
        return variableValueType switch
        {
            VariableValueType.Byte => IdentifierName("Byte"),
            VariableValueType.Bool => IdentifierName("Bool"),
            VariableValueType.Int => IdentifierName("Int"),
            VariableValueType.Bitfield => IdentifierName("Bitfield"),
            VariableValueType.Float => IdentifierName("Float"),
            VariableValueType.Double => IdentifierName("Double"),

            _ => throw new ArgumentException($"'{variableValueType}' value is invalid.", nameof(variableValueType))
        };
    }
}
