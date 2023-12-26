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

            members.Add(DescriptorProperty(ref property));
        }

        return List(members);
    }

    private static PropertyDeclarationSyntax DescriptorProperty(
        ref readonly DescriptorPropertyModel descriptorPropertyModel,
        Accessibility accessibility = Accessibility.Public)
    {
        if (descriptorPropertyModel == default)
        {
            throw new ArgumentException($"'{nameof(descriptorPropertyModel)}' cannot be a default value.", nameof(descriptorPropertyModel));
        }

        var decl = PropertyDeclaration(SharpRacerTypes.DataVariableDescriptor(), descriptorPropertyModel.PropertyIdentifier())
            .WithModifiers(accessibility, isStatic: true)
            .WithGetOnlyAutoAccessor();

        var objectCreationExpr = DataVariableDescriptorSyntaxFactory.CreateNewInstanceExpression(
            descriptorPropertyModel.VariableInfo.Name,
            descriptorPropertyModel.VariableInfo.ValueType,
            descriptorPropertyModel.VariableInfo.ValueCount);

        return decl.WithInitializer(EqualsValueClause(objectCreationExpr))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
    }
}
