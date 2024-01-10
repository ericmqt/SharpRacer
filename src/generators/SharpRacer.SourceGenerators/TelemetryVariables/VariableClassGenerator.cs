using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
using SharpRacer.SourceGenerators.TelemetryVariables.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.TelemetryVariables;
internal static class VariableClassGenerator
{
    public static CompilationUnitSyntax Create(ref readonly VariableClassModel model, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var classDecl = CreateClassDeclaration(in model, cancellationToken);
        var requiredUsings = GetRequiredUsingNamespaces(in model);

        var namespaceDecl = NamespaceDeclaration(IdentifierName(model.ClassNamespace))
            .WithLeadingTrivia(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true)))
            .WithMembers(List(new MemberDeclarationSyntax[] { classDecl }));

        var usingDirectives = requiredUsings.OrderBy(x => x).Select(x => UsingDirective(IdentifierName(x)));

        return CompilationUnit()
            .WithUsings(new SyntaxList<UsingDirectiveSyntax>(usingDirectives))
            .AddMembers(namespaceDecl);
    }

    public static ClassDeclarationSyntax CreateClassDeclaration(ref readonly VariableClassModel model, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var baseTypeList = BaseList(SingletonSeparatedList(model.BaseClassType()));
        var classMembers = CreateClassMembers(in model, cancellationToken);

        return ClassDeclaration(model.ClassName)
            .WithKeyword(Token(SyntaxKind.ClassKeyword))
            .WithModifiers(model.ClassAccesibility(), false, model.IsClassPartial)
            .WithBaseList(baseTypeList)
            .WithMembers(classMembers);
    }

    private static SyntaxList<MemberDeclarationSyntax> CreateClassMembers(ref readonly VariableClassModel model, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var parameterlessCtorXmlDocs = VariableClassSyntaxFactory.ConstructorXmlDocumentation(in model);
        var ctorWithDataVariableInfoXmlDocs = VariableClassSyntaxFactory.ConstructorWithDataVariableInfoParameterXmlDocumentation(in model);

        var members = new List<MemberDeclarationSyntax>()
        {
            model.DescriptorFieldDeclaration(),
            VariableClassSyntaxFactory.Constructor(in model)
                .WithLeadingTrivia(Trivia(parameterlessCtorXmlDocs)),
            VariableClassSyntaxFactory.ConstructorWithDataVariableInfoParameter(in model)
                .WithLeadingTrivia(Trivia(ctorWithDataVariableInfoXmlDocs))
        };

        var createMethodDecl = VariableClassSyntaxFactory.ICreateDataVariableCreateMethodDeclaration(model.ClassName);

        members.Add(createMethodDecl);

        return List(members);
    }

    private static string[] GetRequiredUsingNamespaces(ref readonly VariableClassModel model)
    {
        var telemetryVariablesNamespace = "SharpRacer.Telemetry.Variables";

        if (model.DescriptorPropertyReference.HasValue)
        {
            var descriptorNamespace = model.DescriptorPropertyReference.Value.DescriptorClassNamespace;

            if (!string.Equals(descriptorNamespace, telemetryVariablesNamespace, StringComparison.Ordinal) &&
                !string.Equals(descriptorNamespace, model.ClassNamespace, StringComparison.Ordinal))
            {
                return [telemetryVariablesNamespace, descriptorNamespace];
            }
        }

        return [telemetryVariablesNamespace];
    }
}
