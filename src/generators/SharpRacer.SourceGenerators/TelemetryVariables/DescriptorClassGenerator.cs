using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
using SharpRacer.SourceGenerators.TelemetryVariables.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static SharpRacer.SourceGenerators.Syntax.SyntaxFactoryHelpers;

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
                b.Text("Provides ").See(SharpRacerTypes.TelemetryVariableDescriptor(TypeNameFormat.GlobalQualified))
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

        return CompilationUnit().AddMembers(namespaceDecl);
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
        var objectCreationExpr = TelemetryVariableDescriptorSyntaxFactory.ImplicitNewInstanceExpression(
            descriptorPropertyModel.VariableModel.VariableName,
            descriptorPropertyModel.VariableModel.ValueType,
            descriptorPropertyModel.VariableModel.ValueCount);

        var decl = PropertyDeclaration(SharpRacerTypes.TelemetryVariableDescriptor(TypeNameFormat.GlobalQualified), descriptorPropertyModel.PropertyIdentifier())
            .WithModifiers(accessibility, isStatic: true)
            .WithGetOnlyAutoAccessor()
            .WithInitializer(EqualsValueClause(objectCreationExpr))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        // Attributes
        if (descriptorPropertyModel.VariableModel.IsDeprecated)
        {
            var obsoleteAttribute = ObsoleteAttribute(GetObsoleteAttributeMessage(
                descriptorPropertyModel.VariableName,
                descriptorPropertyModel.VariableModel.DeprecatingVariableName));

            decl = decl.WithAttributeLists(List([
                AttributeList(SingletonSeparatedList(obsoleteAttribute)),
                AttributeList(SingletonSeparatedList(GeneratedCodeAttribute()))
                ]));
        }
        else
        {
            decl = decl.WithAttributeLists(SingletonList(AttributeList(SingletonSeparatedList(GeneratedCodeAttribute()))));
        }

        // XML docs
        if (descriptorPropertyModel.PropertyXmlSummary != null)
        {
            var docTrivia = new XmlDocumentationTriviaBuilder()
                .Summary(descriptorPropertyModel.PropertyXmlSummary)
                .ToTrivia();

            decl = decl.WithLeadingTrivia(Trivia(docTrivia));
        }

        return decl;
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
