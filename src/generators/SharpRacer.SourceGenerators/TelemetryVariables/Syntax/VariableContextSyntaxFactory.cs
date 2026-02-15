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
        var variableInfoProviderParameterName = "variableInfoProvider";

        var variableInfoProviderParameter = Parameter(Identifier(variableInfoProviderParameterName))
            .WithType(SharpRacerTypes.ITelemetryVariableInfoProvider(TypeNameFormat.GlobalQualified));

        // Disable CS0618 when deprecated variables are included. Must be disabled on first property assignment and restored on ctor close
        // brace to ensure our syntax tree matches exactly how Roslyn will parse it.
        var containsDeprecatedVariables = model.Variables.Any(x => x.VariableModel.IsDeprecated);

        var bodyStatements = new List<StatementSyntax>()
        {
            NullCheck(IdentifierName(variableInfoProviderParameterName), TypeNameFormat.Qualified)
        };

        foreach (var variable in model.Variables)
        {
            var propertyIdentifier = variable.PropertyIdentifierName();

            if (containsDeprecatedVariables && variable == model.Variables.First())
            {
                propertyIdentifier = propertyIdentifier.WithLeadingTrivia(
                    TriviaList(
                        Trivia(PragmaWarningDisable("CS0618", "// Type or member is obsolete", true)))
                    );
            }

            var assignmentExpr = AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                propertyIdentifier,
                variable.PropertyObjectCreationExpression(IdentifierName(variableInfoProviderParameterName)));

            var initStatement = ExpressionStatement(assignmentExpr);

            bodyStatements.Add(initStatement);
        }

        var bodyBlock = Block(bodyStatements);

        if (containsDeprecatedVariables)
        {
            bodyBlock = bodyBlock.WithCloseBraceToken(
                Token(
                    TriviaList(Trivia(PragmaWarningRestore("CS0618", "// Type or member is obsolete", true))),
                    SyntaxKind.CloseBraceToken,
                    TriviaList())
                );
        }

        return ConstructorDeclaration(model.ClassIdentifier())
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
            .WithParameterList(ParameterList(SingletonSeparatedList(variableInfoProviderParameter)))
            .WithBody(bodyBlock)
            .WithAttributeLists(SingletonList(AttributeList(SingletonSeparatedList(GeneratedCodeAttribute()))));
    }

    public static MethodDeclarationSyntax EnumerateVariablesMethod(ref readonly ContextClassModel model)
    {
        var containsDeprecatedVariables = model.Variables.Any(x => x.VariableModel.IsDeprecated);

        var variableType = SharpRacerIdentifiers.ITelemetryVariable.ToTypeSyntax(TypeNameFormat.GlobalQualified);

        var returnType = SystemIdentifiers.IEnumerable_T.ToGenericTypeSyntax(
            TypeArgumentList(SingletonSeparatedList<TypeSyntax>(variableType)),
            TypeNameFormat.Qualified);

        var yieldReturnStatements = new List<YieldStatementSyntax>();

        foreach (var variable in model.Variables)
        {
            var yieldReturnStatement = YieldStatement(SyntaxKind.YieldReturnStatement, variable.PropertyIdentifierName());

            if (containsDeprecatedVariables && variable == model.Variables.First())
            {
                yieldReturnStatement = yieldReturnStatement.WithYieldKeyword(
                    Token(
                        TriviaList(Trivia(PragmaWarningRestore("CS0618", "// Type or member is obsolete", true))),
                        SyntaxKind.YieldKeyword,
                        TriviaList())
                    );
            }

            yieldReturnStatements.Add(yieldReturnStatement);
        }

        var methodBodyBlock = Block(yieldReturnStatements);

        if (containsDeprecatedVariables)
        {
            methodBodyBlock = methodBodyBlock.WithCloseBraceToken(
                Token(
                    TriviaList(Trivia(PragmaWarningRestore("CS0618", "// Type or member is obsolete", true))),
                    SyntaxKind.CloseBraceToken,
                    TriviaList())
                );
        }

        return MethodDeclaration(returnType, "EnumerateVariables")
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
            .WithBody(methodBodyBlock)
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
