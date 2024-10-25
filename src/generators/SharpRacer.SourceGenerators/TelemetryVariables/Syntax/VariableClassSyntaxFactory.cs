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
    private static string DataVariableInfoProviderCtorParameterName = "dataVariableInfoProvider";

    public static ConstructorDeclarationSyntax ConstructorWithDataVariableInfoParameter(
        ref readonly VariableClassModel model, bool appendXmlDocumentation)
    {
        var dataVariableParameter = Parameter(Identifier(DataVariableInfoCtorParameterName))
            .WithType(NullableType(SharpRacerTypes.DataVariableInfo(TypeNameFormat.GlobalQualified)));

        var baseCtorInitializer = ConstructorInitializer(
            SyntaxKind.BaseConstructorInitializer,
            ArgumentList(
                SeparatedList(
                [
                    Argument(model.DescriptorFieldIdentifierName()),
                    Argument(IdentifierName(DataVariableInfoCtorParameterName))
                ])));

        var node = ConstructorDeclaration(model.ClassIdentifier())
            .WithAttributeLists(SingletonList(AttributeList(SingletonSeparatedList(GeneratedCodeAttribute()))))
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
            .WithParameterList(ParameterList(SingletonSeparatedList(dataVariableParameter)))
            .WithInitializer(baseCtorInitializer)
            .WithBody(Block());

        return appendXmlDocumentation
            ? node.WithLeadingTrivia(Trivia(ConstructorWithDataVariableInfoParameterXmlDocumentation(in model)))
            : node;
    }

    public static ConstructorDeclarationSyntax ConstructorWithIDataVariableInfoProviderParameter(
        ref readonly VariableClassModel model, bool appendXmlDocumentation)
    {
        var dataVariableInfoProviderParameter = Parameter(Identifier(DataVariableInfoProviderCtorParameterName))
            .WithType(SharpRacerTypes.IDataVariableInfoProvider(TypeNameFormat.GlobalQualified));

        var ctorParamList = ParameterList(SingletonSeparatedList(dataVariableInfoProviderParameter));

        var baseCtorInitializer = ConstructorInitializer(
            SyntaxKind.BaseConstructorInitializer,
            ArgumentList(
                SeparatedList(
                [
                    Argument(model.DescriptorFieldIdentifierName()),
                    Argument(IdentifierName(DataVariableInfoProviderCtorParameterName))
                ])));

        var node = ConstructorDeclaration(model.ClassIdentifier())
            .WithAttributeLists(SingletonList(AttributeList(SingletonSeparatedList(GeneratedCodeAttribute()))))
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
            .WithParameterList(ctorParamList)
            .WithInitializer(baseCtorInitializer)
            .WithBody(Block());

        return appendXmlDocumentation
            ? node.WithLeadingTrivia(Trivia(ConstructorWithIDataVariableInfoProviderParameterXmlDocumentation(in model)))
            : node;
    }

    public static DocumentationCommentTriviaSyntax ConstructorWithDataVariableInfoParameterXmlDocumentation(ref readonly VariableClassModel model)
    {
        var classTypeIdentifier = model.ClassIdentifierName();

        return new XmlDocumentationTriviaBuilder()
            .Summary(
                b => b.Text("Creates an instance of ").See(classTypeIdentifier).Text(" from the specified ")
                    .See(SharpRacerTypes.DataVariableInfo(TypeNameFormat.GlobalQualified)).Text("."))
            .Remarks(
                b => b.Text("If ").ParamRef(DataVariableInfoCtorParameterName).Text(" is ").NullKeyword()
                .Text(", the returned instance represents a telemetry variable which is unavailable in the current context and cannot be used to read data."))
            .Exception(
                TypeCref(SharpRacerTypes.DataVariableInitializationException(TypeNameFormat.Qualified)),
                b => b.ParamRef(DataVariableInfoCtorParameterName).Text(" is not compatible with the telemetry variable represented by this instance."))
            .ToTrivia();
    }

    public static DocumentationCommentTriviaSyntax ConstructorWithIDataVariableInfoProviderParameterXmlDocumentation(ref readonly VariableClassModel model)
    {
        var classTypeIdentifier = model.ClassIdentifierName();

        return new XmlDocumentationTriviaBuilder()
            .Summary(
                b => b.Text("Creates an instance of ").See(classTypeIdentifier).Text(" from the specified ")
                    .See(SharpRacerTypes.IDataVariableInfoProvider(TypeNameFormat.GlobalQualified)).Text("."))
            .Param(DataVariableInfoProviderCtorParameterName,
                b => b.Text("The ").SeeAlso(SharpRacerTypes.IDataVariableInfoProvider(TypeNameFormat.GlobalQualified))
                    .Text(" instance used to perform delayed initialization of ")
                    .See(classTypeIdentifier).Text(" when the associated telemetry variable is activated by the data source."))
            .ToTrivia();
    }

    public static FieldDeclarationSyntax DescriptorStaticField(ref readonly VariableClassModel model)
    {
        var objectCreationExpr = DataVariableDescriptorSyntaxFactory.CreateNewInstanceExpression(
            model.VariableName, model.VariableValueType, model.VariableValueCount);

        var declarator = VariableDeclarator(model.DescriptorFieldIdentifier())
            .WithInitializer(EqualsValueClause(objectCreationExpr));

        var fieldModifiers = TokenList([
            Token(SyntaxKind.PrivateKeyword),
            Token(SyntaxKind.StaticKeyword),
            Token(SyntaxKind.ReadOnlyKeyword)]);

        var declaration = VariableDeclaration(SharpRacerTypes.DataVariableDescriptor(TypeNameFormat.GlobalQualified))
            .WithVariables(SingletonSeparatedList(declarator));

        return FieldDeclaration(declaration)
            .WithModifiers(fieldModifiers);
    }

    public static FieldDeclarationSyntax DescriptorStaticFieldFromDescriptorReferenceDeclaration(
        ref readonly VariableClassModel model,
        MemberAccessExpressionSyntax descriptorReferenceExpression)
    {
        var declarator = VariableDeclarator(model.DescriptorFieldIdentifier())
            .WithInitializer(EqualsValueClause(descriptorReferenceExpression.WithLeadingTrivia(LineFeed, LineFeed)));

        var fieldModifiers = TokenList([
            Token(SyntaxKind.PrivateKeyword),
            Token(SyntaxKind.StaticKeyword),
            Token(SyntaxKind.ReadOnlyKeyword)]);

        var declaration = VariableDeclaration(SharpRacerTypes.DataVariableDescriptor(TypeNameFormat.GlobalQualified))
            .WithVariables(SingletonSeparatedList(declarator));

        return FieldDeclaration(declaration)
            .WithModifiers(fieldModifiers);
    }
}
