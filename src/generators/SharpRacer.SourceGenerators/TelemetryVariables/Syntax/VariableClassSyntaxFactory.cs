using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Syntax;
internal static class VariableClassSyntaxFactory
{
    public static ConstructorInitializerSyntax BaseConstructorInitializerFromDescriptor(
        IdentifierNameSyntax descriptorFieldIdentifier,
        IdentifierNameSyntax? dataVariableInfoParameterIdentifier)
    {
        var descriptorArg = Argument(descriptorFieldIdentifier);

        ArgumentSyntax variableInfoArg;

        if (dataVariableInfoParameterIdentifier != null)
        {
            variableInfoArg = Argument(dataVariableInfoParameterIdentifier);
        }
        else
        {
            variableInfoArg = Argument(LiteralExpression(SyntaxKind.NullLiteralExpression))
                .WithNameColon(NameColon(IdentifierName("variableInfo")));
        }

        var baseCtorArgList = ArgumentList(SeparatedList([descriptorArg, variableInfoArg]));

        return ConstructorInitializer(SyntaxKind.BaseConstructorInitializer, baseCtorArgList);
    }

    public static ConstructorDeclarationSyntax ConstructorFromDescriptor(TypedVariableClassModelBase model)
    {
        var baseCtorInitializer = BaseConstructorInitializerFromDescriptor(
            model.DescriptorFieldIdentifierName(),
            null);

        return ConstructorDeclaration(model.ClassIdentifier())
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
            .WithInitializer(baseCtorInitializer)
            .WithBody(Block());
    }

    public static ConstructorDeclarationSyntax ConstructorFromDescriptorWithDataVariableInfoParameter(TypedVariableClassModelBase model)
    {
        var dataVariableParameter = Parameter(Identifier("dataVariableInfo"))
            .WithType(NullableType(SharpRacerTypes.DataVariableInfo()));

        var baseCtorInitializer = BaseConstructorInitializerFromDescriptor(
            model.DescriptorFieldIdentifierName(),
            IdentifierName("dataVariableInfo"));

        return ConstructorDeclaration(model.ClassIdentifier())
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
            .WithParameterList(ParameterList(SingletonSeparatedList(dataVariableParameter)))
            .WithInitializer(baseCtorInitializer)
            .WithBody(Block());
    }

    public static FieldDeclarationSyntax DescriptorStaticField(TypedArrayVariableClassModel model)
    {
        return DescriptorStaticField(model.DescriptorFieldIdentifier(), model.VariableName, model.VariableValueType, model.ArrayLength);
    }

    public static FieldDeclarationSyntax DescriptorStaticField(TypedScalarVariableClassModel model)
    {
        return DescriptorStaticField(model.DescriptorFieldIdentifier(), model.VariableName, model.VariableValueType, variableValueCount: 1);
    }

    public static FieldDeclarationSyntax DescriptorStaticField(SyntaxToken identifier, string variableName, VariableValueType variableValueType, int variableValueCount)
    {
        var objectCreationExpr = DataVariableDescriptorSyntaxFactory.CreateNewInstanceExpression(variableName, variableValueType, variableValueCount);

        var declarator = VariableDeclarator(identifier)
            .WithInitializer(EqualsValueClause(objectCreationExpr));

        var fieldModifiers = TokenList([
            Token(SyntaxKind.PrivateKeyword),
            Token(SyntaxKind.StaticKeyword),
            Token(SyntaxKind.ReadOnlyKeyword)]);

        var declaration = VariableDeclaration(SharpRacerTypes.DataVariableDescriptor())
            .WithVariables(SingletonSeparatedList(declarator));

        return FieldDeclaration(declaration)
            .WithModifiers(fieldModifiers);
    }

    public static FieldDeclarationSyntax DescriptorStaticFieldFromDescriptorReferenceDeclaration(
        SyntaxToken identifier,
        MemberAccessExpressionSyntax descriptorReferenceExpression)
    {
        var declarator = VariableDeclarator(identifier)
            .WithInitializer(EqualsValueClause(descriptorReferenceExpression));

        var fieldModifiers = TokenList([
            Token(SyntaxKind.PrivateKeyword),
            Token(SyntaxKind.StaticKeyword),
            Token(SyntaxKind.ReadOnlyKeyword)]);

        var declaration = VariableDeclaration(SharpRacerTypes.DataVariableDescriptor())
            .WithVariables(SingletonSeparatedList(declarator));

        return FieldDeclaration(declaration)
            .WithModifiers(fieldModifiers);
    }

    public static MethodDeclarationSyntax ICreateDataVariableCreateMethodDeclaration(string className)
    {
        var dataVariableInfoParam = Parameter(Identifier("dataVariableInfo"))
            .WithType(SharpRacerTypes.DataVariableInfo());

        var createClassInstanceExpr = ObjectCreationExpression(IdentifierName(className))
            .WithArgumentList(
                ArgumentList(
                    SingletonSeparatedList(
                        Argument(IdentifierName("dataVariableInfo")))));

        return MethodDeclaration(IdentifierName(className), Identifier("Create"))
            .WithModifiers(TokenList(
                [Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)]))
            .WithParameterList(
                ParameterList(SingletonSeparatedList(dataVariableInfoParam)))
            .WithBody(Block(SingletonList(
                ReturnStatement(createClassInstanceExpr))));
    }

    public static BaseTypeSyntax ICreateDataVariableInterfaceBaseType(IdentifierNameSyntax selfTypeArgument)
    {
        return SimpleBaseType(SharpRacerTypes.ICreateDataVariableInterfaceType(selfTypeArgument));
    }
}
