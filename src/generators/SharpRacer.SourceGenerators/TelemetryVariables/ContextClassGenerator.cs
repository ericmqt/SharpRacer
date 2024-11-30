using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
using SharpRacer.SourceGenerators.TelemetryVariables.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static SharpRacer.SourceGenerators.Syntax.SyntaxFactoryHelpers;

namespace SharpRacer.SourceGenerators.TelemetryVariables;
internal static class ContextClassGenerator
{
    public static CompilationUnitSyntax Create(ref readonly ContextClassModel model, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var classDecl = CreateClassDeclaration(in model, cancellationToken);

        var namespaceDecl = NamespaceDeclaration(ParseName(model.ClassNamespace))
            .WithLeadingTrivia(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true)))
            .WithMembers(List(new MemberDeclarationSyntax[] { classDecl }));

        return CompilationUnit()
            .AddMembers(namespaceDecl);
    }

    public static ClassDeclarationSyntax CreateClassDeclaration(
        ref readonly ContextClassModel model,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var classMembers = CreateClassMembers(in model, cancellationToken);

        return ClassDeclaration(model.ClassName)
            .WithKeyword(Token(SyntaxKind.ClassKeyword))
            .WithModifiers(Accessibility.NotApplicable, false, isPartial: true)
            .WithMembers(classMembers);
    }

    public static SyntaxList<MemberDeclarationSyntax> CreateClassMembers(
        ref readonly ContextClassModel model,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var members = new List<MemberDeclarationSyntax>()
        {
            VariableContextSyntaxFactory.Constructor(in model)
        };

        foreach (var variable in model.Variables)
        {
            var propertyDecl = VariableContextSyntaxFactory.VariablePropertyDeclaration(in variable);
            var propertyXmlDocs = VariableContextSyntaxFactory.VariablePropertyDeclarationXmlDocumentation(in variable);

            var obsoleteAttribute = GetObsoletePropertyAttribute(in variable, model.Variables);

            if (obsoleteAttribute != null)
            {
                var attributeList = AttributeList(SingletonSeparatedList(obsoleteAttribute));
                propertyDecl = propertyDecl.WithAttributeLists(SingletonList(attributeList));
            }

            members.Add(propertyDecl.WithLeadingTrivia(Trivia(propertyXmlDocs)));
        }

        members.Add(VariableContextSyntaxFactory.EnumerateVariablesMethod(in model));

        return List(members);
    }

    private static AttributeSyntax? GetObsoletePropertyAttribute(
        ref readonly ContextVariableModel variable,
        ImmutableArray<ContextVariableModel> contextVariableModels)
    {
        if (!variable.VariableModel.IsDeprecated)
        {
            return null;
        }

        string? deprecatingVariableName = variable.VariableModel.DeprecatingVariableName;
        string? deprecatingContextPropertyName = null;

        if (!string.IsNullOrEmpty(deprecatingVariableName))
        {
            var deprecatingVariable = contextVariableModels.FirstOrDefault(
                x => x.VariableModel.VariableName.Equals(deprecatingVariableName, StringComparison.Ordinal));

            if (deprecatingVariable != default)
            {
                deprecatingContextPropertyName = deprecatingVariable.PropertyName;
            }
        }

        var message = GetObsoleteAttributeMessage(
            variable.VariableModel.VariableName,
            deprecatingVariableName,
            deprecatingContextPropertyName);

        return ObsoleteAttribute(message);
    }

    private static string GetObsoleteAttributeMessage(string deprecatedVariableName, string? deprecatingVariableName, string? deprecatingPropertyName)
    {
        if (!string.IsNullOrEmpty(deprecatingVariableName))
        {
            if (!string.IsNullOrEmpty(deprecatingPropertyName))
            {
                return $"Telemetry variable '{deprecatedVariableName}' is deprecated by variable '{deprecatingVariableName}'. Use context " +
                    $"property '{deprecatingPropertyName}' instead.";
            }

            return $"Telemetry variable '{deprecatedVariableName}' is deprecated by variable '{deprecatingVariableName}'.";
        }

        return $"Telemetry variable '{deprecatedVariableName}' is deprecated.";
    }
}
