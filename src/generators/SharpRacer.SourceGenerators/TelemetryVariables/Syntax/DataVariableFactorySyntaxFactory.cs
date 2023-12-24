using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Syntax;
internal static class DataVariableFactorySyntaxFactory
{
    private static string CreateArray_MethodName = "CreateType";
    private static string CreateScalar_MethodName = "CreateType";
    private static string CreateType_MethodName = "CreateType";

    internal static ObjectCreationExpressionSyntax InstanceCreationExpression(IdentifierNameSyntax dataVariableProviderIdentifier)
    {
        var dataVariableProviderArg = Argument(dataVariableProviderIdentifier);

        return ObjectCreationExpression(SharpRacerTypes.DataVariableFactory())
            .WithArgumentList(
                ArgumentList(SingletonSeparatedList(dataVariableProviderArg)));
    }

    internal static MemberAccessExpressionSyntax CreateArrayMethodAccessExpression(
        IdentifierNameSyntax factoryInstanceIdentifier,
        TypeSyntax variableValueTypeArgument)
    {
        var methodName = GenericName(Identifier(CreateArray_MethodName))
            .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(variableValueTypeArgument)));

        return MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            factoryInstanceIdentifier,
            methodName);
    }

    internal static MemberAccessExpressionSyntax CreateScalarMethodAccessExpression(
        IdentifierNameSyntax factoryInstanceIdentifier,
        TypeSyntax variableValueTypeArgument)
    {
        var methodName = GenericName(Identifier(CreateScalar_MethodName))
            .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(variableValueTypeArgument)));

        return MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            factoryInstanceIdentifier,
            methodName);
    }

    internal static MemberAccessExpressionSyntax CreateTypeMethodAccessExpression(
        IdentifierNameSyntax factoryInstanceIdentifier,
        TypeSyntax variableClassType)
    {
        var methodName = GenericName(Identifier(CreateType_MethodName))
            .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(variableClassType)));

        return MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            factoryInstanceIdentifier,
            methodName);
    }

    internal static InvocationExpressionSyntax CreateArrayFromDescriptorMethodInvocation(
        IdentifierNameSyntax factoryInstanceIdentifier,
        TypeSyntax variableValueTypeArgument,
        MemberAccessExpressionSyntax staticDescriptorPropertyAccessExpression)
    {
        var methodMemberExpr = CreateArrayMethodAccessExpression(factoryInstanceIdentifier, variableValueTypeArgument);

        var descriptorArg = Argument(staticDescriptorPropertyAccessExpression);

        return InvocationExpression(methodMemberExpr)
            .WithArgumentList(ArgumentList(SingletonSeparatedList(descriptorArg)));
    }

    internal static InvocationExpressionSyntax CreateArrayFromVariableNameAndArrayLengthMethodInvocation(
        IdentifierNameSyntax factoryInstanceIdentifier,
        TypeSyntax variableValueTypeArgument,
        string variableName,
        int arrayLength)
    {
        var methodMemberExpr = CreateArrayMethodAccessExpression(factoryInstanceIdentifier, variableValueTypeArgument);

        var variableNameArg = Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(variableName)));
        var arrayLengthArg = Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(arrayLength)));

        return InvocationExpression(methodMemberExpr)
            .WithArgumentList(ArgumentList(SeparatedList([variableNameArg, arrayLengthArg])));
    }

    internal static InvocationExpressionSyntax CreateScalarFromDescriptorMethodInvocation(
        IdentifierNameSyntax factoryInstanceIdentifier,
        TypeSyntax variableValueTypeArgument,
        MemberAccessExpressionSyntax staticDescriptorPropertyAccessExpression)
    {
        var methodMemberExpr = CreateScalarMethodAccessExpression(factoryInstanceIdentifier, variableValueTypeArgument);

        var descriptorArg = Argument(staticDescriptorPropertyAccessExpression);

        return InvocationExpression(methodMemberExpr)
            .WithArgumentList(ArgumentList(SingletonSeparatedList(descriptorArg)));
    }

    internal static InvocationExpressionSyntax CreateScalarFromVariableNameMethodInvocation(
        IdentifierNameSyntax factoryInstanceIdentifier,
        TypeSyntax variableValueTypeArgument,
        string variableName)
    {
        var methodMemberExpr = CreateScalarMethodAccessExpression(factoryInstanceIdentifier, variableValueTypeArgument);

        var variableNameArg = Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(variableName)));

        return InvocationExpression(methodMemberExpr)
            .WithArgumentList(ArgumentList(SingletonSeparatedList(variableNameArg)));
    }

    internal static InvocationExpressionSyntax CreateTypeFromDescriptorMethodInvocation(
        IdentifierNameSyntax factoryInstanceIdentifier,
        TypeSyntax variableClassType,
        MemberAccessExpressionSyntax staticDescriptorPropertyAccessExpression)
    {
        var methodMemberExpr = CreateTypeMethodAccessExpression(factoryInstanceIdentifier, variableClassType);

        var descriptorArg = Argument(staticDescriptorPropertyAccessExpression);

        return InvocationExpression(methodMemberExpr)
            .WithArgumentList(ArgumentList(SingletonSeparatedList(descriptorArg)));
    }

    internal static InvocationExpressionSyntax CreateTypeFromVariableNameMethodInvocation(
        IdentifierNameSyntax factoryInstanceIdentifier,
        TypeSyntax variableClassType,
        string variableName)
    {
        var methodMemberExpr = CreateTypeMethodAccessExpression(factoryInstanceIdentifier, variableClassType);

        var variableNameArg = Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(variableName)));

        return InvocationExpression(methodMemberExpr)
            .WithArgumentList(ArgumentList(SingletonSeparatedList(variableNameArg)));
    }
}
