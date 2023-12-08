using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.TelemetryVariables;
internal class VariableClassGenerator
{
    private readonly List<string> _usingNamespaces;

    public VariableClassGenerator(VariableClassGeneratorModel model)
    {
        Model = model ?? throw new ArgumentNullException(nameof(model));

        _usingNamespaces = GetUsingNamespaces(model).ToList();
    }

    public VariableClassGeneratorModel Model { get; }

    public ClassDeclarationSyntax CreateClassDeclaration(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return ClassDeclaration(Model.TypeName)
            .WithKeyword(Token(SyntaxKind.ClassKeyword))
            .WithModifiers(Accessibility.Internal, isStatic: false, isPartial: true)
            .WithMembers(List(EnumerateMembers(cancellationToken)));
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
        cancellationToken.ThrowIfCancellationRequested();

        if (Model.DescriptorPropertyReference.HasValue)
        {
            // Descriptor mode! Emit constructors here
        }
        else
        {
            // Generate the non-descriptor constructors here and also emit the static fields
        }

        yield break;
    }

    private static IEnumerable<string> GetUsingNamespaces(VariableClassGeneratorModel model)
    {
        var telemetryVariablesNamespace = "SharpRacer.Telemetry.Variables";

        yield return "System";
        yield return "System.Collections.Generic";
        yield return "System.Text";
        yield return telemetryVariablesNamespace;

        if (model.DescriptorPropertyReference.HasValue)
        {
            var descriptorNamespace = model.DescriptorPropertyReference.Value.DescriptorClassNamespace;

            if (!string.Equals(descriptorNamespace, telemetryVariablesNamespace, StringComparison.Ordinal))
            {
                yield return descriptorNamespace;
            }
        }
    }
}
