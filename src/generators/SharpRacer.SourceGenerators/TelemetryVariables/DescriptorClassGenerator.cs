using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
using SharpRacer.SourceGenerators.TelemetryVariables.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.TelemetryVariables;

internal class DescriptorClassGenerator
{
    private readonly List<string> _usingNamespaces;

    public DescriptorClassGenerator(DescriptorClassGeneratorModel model)
    {
        Model = model ?? throw new ArgumentNullException(nameof(model));

        _usingNamespaces = new List<string>()
        {
            "System",
            "System.Collections.Generic",
            "System.Text",
            "SharpRacer.Telemetry.Variables"
        };
    }

    public DescriptorClassGeneratorModel Model { get; }

    public ClassDeclarationSyntax CreateClassDeclaration(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return ClassDeclaration(Model.TypeName)
            .WithKeyword(Token(SyntaxKind.ClassKeyword))
            .WithModifiers(Accessibility.NotApplicable, isStatic: true, isPartial: true)
            .WithMembers(List(EnumerateMembers(cancellationToken)));
        //.WithBaseList(CreateBaseList(cancellationToken));
    }

    public CompilationUnitSyntax CreateCompilationUnit(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var classDecl = CreateClassDeclaration(cancellationToken);

        var namespaceDecl = NamespaceDeclaration(IdentifierName(Model.TypeNamespace))
            .WithMembers(List(new MemberDeclarationSyntax[] { classDecl }));

        var usingDirectives = _usingNamespaces.Select(x => UsingDirective(IdentifierName(x)));

        return CompilationUnit()
            .WithUsings(new SyntaxList<UsingDirectiveSyntax>(usingDirectives))
            .AddMembers(namespaceDecl);
    }

    private IEnumerable<MemberDeclarationSyntax> EnumerateMembers(CancellationToken cancellationToken = default)
    {
        foreach (var variable in Model.Variables)
        {
            cancellationToken.ThrowIfCancellationRequested();

            yield return DataVariableDescriptorSyntaxFactory.ReadOnlyStaticPropertyWithInitializer(variable);
        }
    }

}
