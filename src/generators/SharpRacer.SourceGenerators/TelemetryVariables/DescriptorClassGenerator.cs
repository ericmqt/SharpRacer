using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
using SharpRacer.SourceGenerators.TelemetryVariables.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.TelemetryVariables;

internal static class DescriptorClassGenerator
{
    public static ClassDeclarationSyntax CreateClassDeclaration(
        ref readonly DescriptorClassModel model,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var xmlDoc = new XmlDocumentationTriviaBuilder()
            .Summary(b =>
                b.Text("Provides ").See(SharpRacerTypes.DataVariableDescriptor(TypeNameFormat.Qualified))
                .Text(" values that describe telemetry variables."))
            .ToTrivia();

        return ClassDeclaration(model.TypeName)
            .WithKeyword(Token(SyntaxKind.ClassKeyword))
            .WithModifiers(Accessibility.NotApplicable, isStatic: true, isPartial: true)
            .WithMembers(GetClassMemberDeclarations(in model, cancellationToken))
            .WithLeadingTrivia(Trivia(xmlDoc));
    }

    public static CompilationUnitSyntax CreateCompilationUnit(
        ref readonly DescriptorClassModel model,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var classDecl = CreateClassDeclaration(in model, cancellationToken);

        var namespaceDecl = NamespaceDeclaration(ParseName(model.TypeNamespace))
            .WithLeadingTrivia(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true)))
            .WithMembers(List(new MemberDeclarationSyntax[] { classDecl }));

        var usingDirectives = new SyntaxList<UsingDirectiveSyntax>(UsingDirective(ParseName("SharpRacer.Telemetry")));

        return CompilationUnit()
            .WithUsings(usingDirectives)
            .AddMembers(namespaceDecl);
    }

    private static SyntaxList<MemberDeclarationSyntax> GetClassMemberDeclarations(
        ref readonly DescriptorClassModel model,
        CancellationToken cancellationToken)
    {
        var members = new List<MemberDeclarationSyntax>();

        for (int i = 0; i < model.DescriptorProperties.Length; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var property = model.DescriptorProperties[i];

            members.Add(DescriptorProperty(ref property, Accessibility.Public));
        }

        return List(members);
    }

    private static PropertyDeclarationSyntax DescriptorProperty(
        ref readonly DescriptorPropertyModel descriptorPropertyModel,
        Accessibility accessibility)
    {
        var objectCreationExpr = DataVariableDescriptorSyntaxFactory.ImplicitNewInstanceExpression(
            descriptorPropertyModel.VariableModel.VariableName,
            descriptorPropertyModel.VariableModel.ValueType,
            descriptorPropertyModel.VariableModel.ValueCount);

        var decl = PropertyDeclaration(SharpRacerTypes.DataVariableDescriptor(), descriptorPropertyModel.PropertyIdentifier())
            .WithModifiers(accessibility, isStatic: true)
            .WithGetOnlyAutoAccessor()
            .WithInitializer(EqualsValueClause(objectCreationExpr))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        var obsoleteAttribute = GetObsoleteDescriptorPropertyAttribute(in descriptorPropertyModel);

        if (obsoleteAttribute != null)
        {
            var attributeList = AttributeList(SingletonSeparatedList(obsoleteAttribute));

            decl = decl.WithAttributeLists(SingletonList(attributeList));
        }

        if (descriptorPropertyModel.PropertyXmlSummary != null)
        {
            var docTrivia = new XmlDocumentationTriviaBuilder()
                .Summary(descriptorPropertyModel.PropertyXmlSummary)
                .ToTrivia();

            decl = decl.WithLeadingTrivia(Trivia(docTrivia));
        }

        return decl;
    }

    private static AttributeSyntax? GetObsoleteDescriptorPropertyAttribute(ref readonly DescriptorPropertyModel descriptorPropertyModel)
    {
        if (!descriptorPropertyModel.VariableModel.IsDeprecated)
        {
            return null;
        }

        string? deprecatingVariableName = descriptorPropertyModel.VariableModel.DeprecatingVariableName;

        // Build the attribute
        var messageLiteral = Literal(
            GetObsoleteAttributeMessage(descriptorPropertyModel.VariableName, deprecatingVariableName));

        var messageArgument = AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, messageLiteral));

        return Attribute(IdentifierName("Obsolete"))
            .WithArgumentList(AttributeArgumentList(SingletonSeparatedList(messageArgument)));
    }

    private static string GetObsoleteAttributeMessage(string deprecatedVariableName, string? deprecatingVariableName)
    {
        if (!string.IsNullOrEmpty(deprecatingVariableName))
        {
            return $"Telemetry variable '{deprecatedVariableName}' is deprecated by variable '{deprecatingVariableName}'.";
        }

        return $"Telemetry variable '{deprecatedVariableName}' is deprecated.";
    }
}
