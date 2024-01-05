using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static SharpRacer.SourceGenerators.Syntax.SyntaxFactoryHelpers;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Syntax;
internal static class VariableClassSyntaxFactory
{
    private static string DataVariableInfoCtorParameterName = "dataVariableInfo";

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

    /// <summary>
    /// <see langword="false" />
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public static ConstructorDeclarationSyntax Constructor(ref readonly VariableClassGeneratorModel model)
    {
        var baseCtorInitializer = BaseConstructorInitializerFromDescriptor(
            model.DescriptorFieldIdentifierName(),
            null);

        return ConstructorDeclaration(model.ClassIdentifier())
            .WithAttributeLists(SingletonList(AttributeList(SingletonSeparatedList(GeneratedCodeAttribute()))))
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
            .WithInitializer(baseCtorInitializer)
            .WithBody(Block());
    }

    public static ConstructorDeclarationSyntax ConstructorWithDataVariableInfoParameter(ref readonly VariableClassGeneratorModel model)
    {
        var dataVariableParameter = Parameter(Identifier(DataVariableInfoCtorParameterName))
            .WithType(NullableType(SharpRacerTypes.DataVariableInfo()));

        var baseCtorInitializer = BaseConstructorInitializerFromDescriptor(
            model.DescriptorFieldIdentifierName(),
            IdentifierName(DataVariableInfoCtorParameterName));

        return ConstructorDeclaration(model.ClassIdentifier())
            .WithAttributeLists(SingletonList(AttributeList(SingletonSeparatedList(GeneratedCodeAttribute()))))
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
            .WithParameterList(ParameterList(SingletonSeparatedList(dataVariableParameter)))
            .WithInitializer(baseCtorInitializer)
            .WithBody(Block());
    }

    public static DocumentationCommentTriviaSyntax ConstructorXmlDocumentation(ref readonly VariableClassGeneratorModel model)
    {
        var classTypeIdentifier = model.ClassIdentifierName();

        return new XmlDocumentationTriviaBuilder()
            .Summary(b =>
                b.Text("Creates an instance of ")
                .See(classTypeIdentifier)
                .Text(" configured as a placeholder for the underlying telemetry variable which is unavailable in the current context."))
            .Remarks(b =>
                b.Text("The returned instance cannot be used to read data because the value for property ")
                .See(NameMemberCref(ParseName("IsAvailable"))).Text(" is ").Langword("false").Text("."))
            .ToTrivia();
    }

    public static DocumentationCommentTriviaSyntax ConstructorWithDataVariableInfoParameterXmlDocumentation(ref readonly VariableClassGeneratorModel model)
    {
        var classTypeIdentifier = model.ClassIdentifierName();

        return new XmlDocumentationTriviaBuilder()
            .Summary(
                b => b.Text("Creates an instance of ").See(classTypeIdentifier).Text(" from the specified ").See(SharpRacerTypes.DataVariableInfo()).Text("."))
            .Remarks(
                b => b.Text("If ").ParamRef(DataVariableInfoCtorParameterName).Text(" is ").NullKeyword()
                .Text(", the returned instance represents a telemetry variable which is unavailable in the current context and cannot be used to read data."))
            .Exception(
                TypeCref(SharpRacerTypes.DataVariableInitializationException(TypeNameFormat.Qualified)),
                b => b.ParamRef(DataVariableInfoCtorParameterName).Text(" is not compatible with the telemetry variable represented by this instance."))
            .ToTrivia();
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
            .WithInitializer(EqualsValueClause(descriptorReferenceExpression.WithLeadingTrivia(LineFeed, LineFeed)));

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

        var xmlDoc = XmlDocumentationFactory.InheritDoc();

        return MethodDeclaration(IdentifierName(className), Identifier("Create"))
            .WithAttributeLists(SingletonList(AttributeList(SingletonSeparatedList(GeneratedCodeAttribute()))))
            .WithModifiers(TokenList(
                [Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)]))
            .WithParameterList(
                ParameterList(SingletonSeparatedList(dataVariableInfoParam)))
            .WithBody(Block(SingletonList(
                ReturnStatement(createClassInstanceExpr))))
            .WithLeadingTrivia(Trivia(xmlDoc));
    }

    public static BaseTypeSyntax ICreateDataVariableInterfaceBaseType(IdentifierNameSyntax selfTypeArgument)
    {
        return SimpleBaseType(SharpRacerTypes.ICreateDataVariableInterfaceType(selfTypeArgument));
    }
}
