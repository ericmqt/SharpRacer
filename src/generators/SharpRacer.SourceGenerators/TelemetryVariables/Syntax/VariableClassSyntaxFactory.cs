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
    private static string TelemetryVariableInfoCtorParameterName = "dataVariableInfo";
    private static string TelemetryVariableInfoProviderCtorParameterName = "dataVariableInfoProvider";

    public static ConstructorDeclarationSyntax ConstructorWithTelemetryVariableInfoParameter(
        ref readonly VariableClassModel model, bool appendXmlDocumentation)
    {
        var telemetryVariableParameter = Parameter(Identifier(TelemetryVariableInfoCtorParameterName))
            .WithType(NullableType(SharpRacerTypes.TelemetryVariableInfo(TypeNameFormat.GlobalQualified)));

        var baseCtorInitializer = ConstructorInitializer(
            SyntaxKind.BaseConstructorInitializer,
            ArgumentList(
                SeparatedList(
                [
                    Argument(model.DescriptorFieldIdentifierName()),
                    Argument(IdentifierName(TelemetryVariableInfoCtorParameterName))
                ])));

        var node = ConstructorDeclaration(model.ClassIdentifier())
            .WithAttributeLists(SingletonList(AttributeList(SingletonSeparatedList(GeneratedCodeAttribute()))))
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
            .WithParameterList(ParameterList(SingletonSeparatedList(telemetryVariableParameter)))
            .WithInitializer(baseCtorInitializer)
            .WithBody(Block());

        return appendXmlDocumentation
            ? node.WithLeadingTrivia(Trivia(ConstructorWithTelemetryVariableInfoParameterXmlDocumentation(in model)))
            : node;
    }

    public static ConstructorDeclarationSyntax ConstructorWithITelemetryVariableInfoProviderParameter(
        ref readonly VariableClassModel model, bool appendXmlDocumentation)
    {
        var variableInfoProviderParameter = Parameter(Identifier(TelemetryVariableInfoProviderCtorParameterName))
            .WithType(SharpRacerTypes.ITelemetryVariableInfoProvider(TypeNameFormat.GlobalQualified));

        var ctorParamList = ParameterList(SingletonSeparatedList(variableInfoProviderParameter));

        var baseCtorInitializer = ConstructorInitializer(
            SyntaxKind.BaseConstructorInitializer,
            ArgumentList(
                SeparatedList(
                [
                    Argument(model.DescriptorFieldIdentifierName()),
                    Argument(IdentifierName(TelemetryVariableInfoProviderCtorParameterName))
                ])));

        var node = ConstructorDeclaration(model.ClassIdentifier())
            .WithAttributeLists(SingletonList(AttributeList(SingletonSeparatedList(GeneratedCodeAttribute()))))
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
            .WithParameterList(ctorParamList)
            .WithInitializer(baseCtorInitializer)
            .WithBody(Block());

        return appendXmlDocumentation
            ? node.WithLeadingTrivia(Trivia(ConstructorWithITelemetryVariableInfoProviderParameterXmlDocumentation(in model)))
            : node;
    }

    public static DocumentationCommentTriviaSyntax ConstructorWithTelemetryVariableInfoParameterXmlDocumentation(
        ref readonly VariableClassModel model)
    {
        var classTypeIdentifier = model.ClassIdentifierName();

        return new XmlDocumentationTriviaBuilder()
            .Summary(
                b => b.Text("Creates an instance of ").See(classTypeIdentifier).Text(" from the specified ")
                    .See(SharpRacerTypes.TelemetryVariableInfo(TypeNameFormat.GlobalQualified)).Text("."))
            .Remarks(
                b => b.Text("If ").ParamRef(TelemetryVariableInfoCtorParameterName).Text(" is ").NullKeyword()
                .Text(", the returned instance represents a telemetry variable which is unavailable in the current context and cannot be used to read data."))
            .Exception(
                TypeCref(SharpRacerTypes.TelemetryVariableInitializationException(TypeNameFormat.Qualified)),
                b => b.ParamRef(TelemetryVariableInfoCtorParameterName).Text(" is not compatible with the telemetry variable represented by this instance."))
            .ToTrivia();
    }

    public static DocumentationCommentTriviaSyntax ConstructorWithITelemetryVariableInfoProviderParameterXmlDocumentation(
        ref readonly VariableClassModel model)
    {
        var classTypeIdentifier = model.ClassIdentifierName();

        return new XmlDocumentationTriviaBuilder()
            .Summary(
                b => b.Text("Creates an instance of ").See(classTypeIdentifier).Text(" from the specified ")
                    .See(SharpRacerTypes.ITelemetryVariableInfoProvider(TypeNameFormat.GlobalQualified)).Text("."))
            .Param(TelemetryVariableInfoProviderCtorParameterName,
                b => b.Text("The ").SeeAlso(SharpRacerTypes.ITelemetryVariableInfoProvider(TypeNameFormat.GlobalQualified))
                    .Text(" instance used to perform delayed initialization of ")
                    .See(classTypeIdentifier).Text(" when the associated telemetry variable is activated by the data source."))
            .ToTrivia();
    }

    public static FieldDeclarationSyntax DescriptorStaticField(ref readonly VariableClassModel model)
    {
        var objectCreationExpr = TelemetryVariableDescriptorSyntaxFactory.CreateNewInstanceExpression(
            model.VariableName, model.VariableValueType, model.VariableValueCount);

        var declarator = VariableDeclarator(model.DescriptorFieldIdentifier())
            .WithInitializer(EqualsValueClause(objectCreationExpr));

        var fieldModifiers = TokenList([
            Token(SyntaxKind.PrivateKeyword),
            Token(SyntaxKind.StaticKeyword),
            Token(SyntaxKind.ReadOnlyKeyword)]);

        var declaration = VariableDeclaration(SharpRacerTypes.TelemetryVariableDescriptor(TypeNameFormat.GlobalQualified))
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

        var declaration = VariableDeclaration(SharpRacerTypes.TelemetryVariableDescriptor(TypeNameFormat.GlobalQualified))
            .WithVariables(SingletonSeparatedList(declarator));

        return FieldDeclaration(declaration)
            .WithModifiers(fieldModifiers);
    }
}
