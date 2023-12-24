using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
using SharpRacer.SourceGenerators.TelemetryVariables.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.TelemetryVariables;
internal static class ContextClassGenerator
{
    public static CompilationUnitSyntax Create(ref readonly ContextClassModel model, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var classDecl = CreateClassDeclaration(in model, cancellationToken);

        var usingDirectives = new SyntaxList<UsingDirectiveSyntax>(UsingDirective(ParseName("SharpRacer.Telemetry.Variables")));

        var namespaceDecl = NamespaceDeclaration(IdentifierName(model.TypeNamespace))
            .WithLeadingTrivia(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true)))
            .WithMembers(List(new MemberDeclarationSyntax[] { classDecl }));

        return CompilationUnit()
            .WithUsings(new SyntaxList<UsingDirectiveSyntax>(usingDirectives))
            .AddMembers(namespaceDecl);
    }

    public static ClassDeclarationSyntax CreateClassDeclaration(
        ref readonly ContextClassModel model,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var classMembers = CreateClassMembers(in model, cancellationToken);

        return ClassDeclaration(model.TypeName)
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

            members.Add(propertyDecl);
        }

        // TODO: Interface implementation

        return List(members);
    }
}
