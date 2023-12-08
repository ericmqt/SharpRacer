using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
using SharpRacer.SourceGenerators.TelemetryVariables.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.TelemetryVariables;
internal static class TypedVariableClassGenerator
{
    public static CompilationUnitSyntax Create(TypedVariableClassGeneratorModel model, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var classDecl = CreateClassDeclaration(model, cancellationToken);
        var requiredUsings = GetRequiredUsingNamespaces(model);

        var namespaceDecl = NamespaceDeclaration(IdentifierName(model.ClassNamespace))
            .WithLeadingTrivia(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true)))
            .WithMembers(List(new MemberDeclarationSyntax[] { classDecl }));

        var usingDirectives = requiredUsings.OrderBy(x => x).Select(x => UsingDirective(IdentifierName(x)));

        return CompilationUnit()
            .WithUsings(new SyntaxList<UsingDirectiveSyntax>(usingDirectives))
            .AddMembers(namespaceDecl);
    }

    public static ClassDeclarationSyntax CreateClassDeclaration(TypedVariableClassGeneratorModel model, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var baseTypeList = CreateClassBaseTypeList(model, cancellationToken);
        var classMembers = CreateClassMembers(model, cancellationToken);

        return ClassDeclaration(model.ClassName)
            .WithKeyword(Token(SyntaxKind.ClassKeyword))
            .WithModifiers(model.ClassAccesibility(), false, model.IsClassPartial)
            .WithBaseList(baseTypeList)
            .WithMembers(classMembers);
    }

    private static SyntaxList<MemberDeclarationSyntax> CreateClassMembers(TypedVariableClassGeneratorModel model, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var members = new List<MemberDeclarationSyntax>()
        {
            model.DescriptorFieldDeclaration(),
            VariableClassSyntaxFactory.ConstructorFromDescriptor(model),
            VariableClassSyntaxFactory.ConstructorFromDescriptorWithDataVariableInfoParameter(model)
        };

        if (model.ImplementCreateDataVariableInterface)
        {
            var createMethodDecl = VariableClassSyntaxFactory.ICreateDataVariableCreateMethodDeclaration(model.ClassName);

            members.Add(createMethodDecl);
        }

        return List(members);
    }

    private static BaseListSyntax CreateClassBaseTypeList(TypedVariableClassGeneratorModel model, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (model.AddCreateDataVariableInterfaceBaseType)
        {
            var interfaceBaseType = VariableClassSyntaxFactory.ICreateDataVariableInterfaceBaseType(model.ClassIdentifierName());

            return BaseList(
                SeparatedList([model.BaseClassType(), interfaceBaseType]));
        }

        return BaseList(SingletonSeparatedList(model.BaseClassType()));
    }

    private static string[] GetRequiredUsingNamespaces(TypedVariableClassGeneratorModel model)
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
