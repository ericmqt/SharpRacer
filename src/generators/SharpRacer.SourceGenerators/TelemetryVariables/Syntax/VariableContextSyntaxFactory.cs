using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static SharpRacer.SourceGenerators.Syntax.SyntaxFactoryHelpers;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Syntax;

internal static class VariableContextSyntaxFactory
{
    public static ConstructorDeclarationSyntax Constructor(ref readonly ContextClassModel model)
    {
        var variableInfoProviderParameterName = "dataVariableInfoProvider";

        var variableInfoProviderParameter = Parameter(Identifier(variableInfoProviderParameterName))
            .WithType(SharpRacerTypes.ITelemetryVariableInfoProvider(TypeNameFormat.GlobalQualified));

        var bodyStatements = new List<StatementSyntax>()
        {
            NullCheck(IdentifierName(variableInfoProviderParameterName), TypeNameFormat.Qualified)
        };

        foreach (var variable in model.Variables)
        {
            var initStatement = InitializeVariableProperty(in variable, IdentifierName(variableInfoProviderParameterName));

            bodyStatements.Add(initStatement);
        }

        return ConstructorDeclaration(model.ClassIdentifier())
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
            .WithParameterList(ParameterList(SingletonSeparatedList(variableInfoProviderParameter)))
            .WithBody(Block(bodyStatements))
            .WithAttributeLists(SingletonList(AttributeList(SingletonSeparatedList(GeneratedCodeAttribute()))));
    }

    public static MethodDeclarationSyntax EnumerateVariablesMethod(ref readonly ContextClassModel model)
    {
        var variableType = SharpRacerIdentifiers.ITelemetryVariable.ToTypeSyntax(TypeNameFormat.GlobalQualified);

        var returnType = SystemIdentifiers.IEnumerable_T.ToGenericTypeSyntax(
            TypeArgumentList(SingletonSeparatedList<TypeSyntax>(variableType)),
            TypeNameFormat.Qualified);

        var yieldReturnStatements = model.Variables
            .Select(x => YieldStatement(SyntaxKind.YieldReturnStatement, x.PropertyIdentifierName()))
            .ToList();

        return MethodDeclaration(returnType, "EnumerateVariables")
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
            .WithBody(Block(yieldReturnStatements))
            .WithAttributeLists(SingletonList(AttributeList(SingletonSeparatedList(GeneratedCodeAttribute()))))
            .WithLeadingTrivia(Trivia(XmlDocumentationFactory.InheritDoc()));
    }

    public static ExpressionStatementSyntax InitializeVariableProperty(
        ref readonly ContextVariableModel model,
        IdentifierNameSyntax variableInfoProviderIdentifier)
    {
        var assignmentExpr = AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            model.PropertyIdentifierName(),
            model.PropertyObjectCreationExpression(variableInfoProviderIdentifier));

        return ExpressionStatement(assignmentExpr);
    }

    public static PropertyDeclarationSyntax VariablePropertyDeclaration(ref readonly ContextVariableModel model)
    {
        return PropertyDeclaration(model.PropertyType(TypeNameFormat.GlobalQualified), model.PropertyIdentifier())
            .WithModifiers(Accessibility.Public)
            .WithGetOnlyAutoAccessor();
    }

    public static DocumentationCommentTriviaSyntax VariablePropertyDeclarationXmlDocumentation(ref readonly ContextVariableModel model)
    {
        var summaryText = model.PropertyXmlSummary;

        return new XmlDocumentationTriviaBuilder()
            .Summary(b => b.Text(summaryText))
            .ToTrivia();
    }
}
