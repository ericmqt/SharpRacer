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

        return ClassDeclaration(model.TypeName)
            .WithKeyword(Token(SyntaxKind.ClassKeyword))
            .WithModifiers(Accessibility.NotApplicable, isStatic: true, isPartial: true)
            .WithMembers(GetClassMemberDeclarations(in model, cancellationToken));
    }

    public static CompilationUnitSyntax CreateCompilationUnit(
        ref readonly DescriptorClassModel model,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var classDecl = CreateClassDeclaration(in model, cancellationToken);

        var namespaceDecl = NamespaceDeclaration(IdentifierName(model.TypeNamespace))
            .WithMembers(List(new MemberDeclarationSyntax[] { classDecl }));

        var usingDirectives = new SyntaxList<UsingDirectiveSyntax>(UsingDirective(ParseName("SharpRacer.Telemetry.Variables")));

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
        if (descriptorPropertyModel == default)
        {
            throw new ArgumentException($"'{nameof(descriptorPropertyModel)}' cannot be a default value.", nameof(descriptorPropertyModel));
        }

        var objectCreationExpr = DataVariableDescriptorSyntaxFactory.CreateNewInstanceExpression(
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

        // TODO: XML documentation
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
