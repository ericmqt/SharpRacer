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
    public static LocalDeclarationStatementSyntax ConstructorDataVariableFactoryLocal(string factoryIdentifier, IdentifierNameSyntax dataVariableProviderIdentifier)
    {
        if (string.IsNullOrEmpty(factoryIdentifier))
        {
            throw new ArgumentException($"'{nameof(factoryIdentifier)}' cannot be null or empty.", nameof(factoryIdentifier));
        }

        if (dataVariableProviderIdentifier is null)
        {
            throw new ArgumentNullException(nameof(dataVariableProviderIdentifier));
        }

        var factoryCreationExpr = DataVariableFactorySyntaxFactory.InstanceCreationExpression(dataVariableProviderIdentifier);

        var variableDeclarator = VariableDeclarator(factoryIdentifier)
            .WithInitializer(EqualsValueClause(factoryCreationExpr));

        var variableDeclaration = VariableDeclaration(
            IdentifierName(
                Identifier(TriviaList(), SyntaxKind.VarKeyword, "var", "var", TriviaList())))
            .WithVariables(SingletonSeparatedList(variableDeclarator));

        return LocalDeclarationStatement(variableDeclaration);
    }

    public static ConstructorDeclarationSyntax Constructor(ref readonly ContextClassModel model)
    {
        var dataVariableInfoProviderParameter = Parameter(Identifier("dataVariableProvider"))
            .WithType(SharpRacerTypes.IDataVariableInfoProvider());

        var nullCheck = NullCheck(IdentifierName("dataVariableProvider"));

        var factoryLocal = ConstructorDataVariableFactoryLocal("factory", IdentifierName("dataVariableProvider"));
        var factoryIdentifierName = IdentifierName("factory");

        var bodyStatements = new List<StatementSyntax>()
        {
            nullCheck,
            factoryLocal
        };

        foreach (var variable in model.Variables)
        {
            var statement = InitializeVariablePropertyFromFactory(in variable, factoryIdentifierName);

            bodyStatements.Add(statement);
        }

        return ConstructorDeclaration(model.ClassIdentifier())
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
            .WithParameterList(ParameterList(SingletonSeparatedList(dataVariableInfoProviderParameter)))
            .WithBody(Block(bodyStatements));
    }

    public static ParameterListSyntax ConstructorParameterList(string dataVariableProviderParameterName)
    {
        if (string.IsNullOrEmpty(dataVariableProviderParameterName))
        {
            throw new ArgumentException($"'{nameof(dataVariableProviderParameterName)}' cannot be null or empty.", nameof(dataVariableProviderParameterName));
        }

        var dataVariableInfoProviderParameter = Parameter(Identifier(dataVariableProviderParameterName))
            .WithType(SharpRacerTypes.IDataVariableInfoProvider());

        return ParameterList(SingletonSeparatedList(dataVariableInfoProviderParameter));
    }

    public static MethodDeclarationSyntax EnumerateVariablesMethod(ref readonly ContextClassModel model)
    {
        var dataVariableType = IdentifierName(SharpRacerIdentifiers.IDataVariable);
        var returnType = GenericName(Identifier("IEnumerable"), TypeArgumentList(SingletonSeparatedList<TypeSyntax>(dataVariableType)));

        var yieldReturnStatements = model.Variables
            .Select(x => YieldStatement(SyntaxKind.YieldReturnStatement, x.PropertyIdentifierName()))
            .ToList();

        return MethodDeclaration(returnType, "EnumerateVariables")
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
            .WithBody(Block(yieldReturnStatements))
            .WithLeadingTrivia(Trivia(XmlDocumentationFactory.InheritDoc()));
    }

    public static ExpressionStatementSyntax InitializeVariablePropertyFromFactory(
        ref readonly ContextVariableModel model,
        IdentifierNameSyntax factoryInstanceIdentifier)
    {
        var assignmentExpr = AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            model.PropertyIdentifierName(),
            model.DataVariableFactoryCreateMethodInvocation(factoryInstanceIdentifier));

        return ExpressionStatement(assignmentExpr);
    }

    public static PropertyDeclarationSyntax VariablePropertyDeclaration(ref readonly ContextVariableModel model)
    {
        return PropertyDeclaration(model.PropertyType(), model.PropertyIdentifier())
            .WithModifiers(Accessibility.Public)
            .WithGetOnlyAutoAccessor();
    }
}
