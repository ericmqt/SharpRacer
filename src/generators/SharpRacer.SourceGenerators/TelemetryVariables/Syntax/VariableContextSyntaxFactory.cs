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
        var dataVariableInfoProviderParameterName = "dataVariableInfoProvider";

        var dataVariableInfoProviderParameter = Parameter(Identifier(dataVariableInfoProviderParameterName))
            .WithType(SharpRacerTypes.IDataVariableInfoProvider(TypeNameFormat.GlobalQualified));

        var bodyStatements = new List<StatementSyntax>()
        {
            NullCheck(IdentifierName(dataVariableInfoProviderParameterName), TypeNameFormat.Qualified)
        };

        foreach (var variable in model.Variables)
        {
            var initStatement = InitializeVariableProperty(in variable, IdentifierName(dataVariableInfoProviderParameterName));

            bodyStatements.Add(initStatement);
        }

        return ConstructorDeclaration(model.ClassIdentifier())
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
            .WithParameterList(ParameterList(SingletonSeparatedList(dataVariableInfoProviderParameter)))
            .WithBody(Block(bodyStatements))
            .WithAttributeLists(SingletonList(AttributeList(SingletonSeparatedList(GeneratedCodeAttribute()))));
    }

    public static MethodDeclarationSyntax EnumerateVariablesMethod(ref readonly ContextClassModel model)
    {
        var dataVariableType = SharpRacerIdentifiers.IDataVariable.ToTypeSyntax(TypeNameFormat.GlobalQualified);

        var returnType = SystemIdentifiers.IEnumerable_T.ToGenericTypeSyntax(
            TypeArgumentList(SingletonSeparatedList<TypeSyntax>(dataVariableType)),
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
        IdentifierNameSyntax dataVariableInfoProviderIdentifier)
    {
        var assignmentExpr = AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            model.PropertyIdentifierName(),
            model.PropertyObjectCreationExpression(dataVariableInfoProviderIdentifier));

        return ExpressionStatement(assignmentExpr);
    }

    public static PropertyDeclarationSyntax VariablePropertyDeclaration(ref readonly ContextVariableModel model)
    {
        return PropertyDeclaration(model.PropertyType(TypeNameFormat.GlobalQualified), model.PropertyIdentifier())
            .WithModifiers(Accessibility.Public)
            .WithGetOnlyAutoAccessor();
    }
}
