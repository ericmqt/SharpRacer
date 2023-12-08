using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.TelemetryVariables;
internal static class VariableClassGenerator
{
    public static CompilationUnitSyntax CreateTypedVariableClassCompilationUnit(
        VariableClassGeneratorModel model,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ClassDeclarationSyntax classDeclaration;
        string[] requiredNamespaces;

        if (model.VariableModel.VariableInfo.ValueCount > 1)
        {
            var typedArrayClassModel = new TypedArrayVariableClassModel(
                model.TypeName,
                model.VariableModel,
                model.DescriptorPropertyReference);

            classDeclaration = TypedArrayVariableClassFactory.CreateClassDeclaration(
                typedArrayClassModel,
                cancellationToken,
                out requiredNamespaces);
        }
        else
        {
            var typedScalarClassModel = new TypedScalarVariableClassModel(
                model.TypeName,
                model.VariableModel,
                model.DescriptorPropertyReference);

            classDeclaration = TypedScalarVariableClassFactory.CreateClassDeclaration(
                typedScalarClassModel,
                cancellationToken,
                out requiredNamespaces);
        }

        var namespaceDecl = NamespaceDeclaration(IdentifierName(model.TypeNamespace))
            .WithMembers(List(new MemberDeclarationSyntax[] { classDeclaration }));

        var usingDirectives = requiredNamespaces.Select(x => UsingDirective(IdentifierName(x)));

        return CompilationUnit()
            .WithUsings(new SyntaxList<UsingDirectiveSyntax>(usingDirectives))
            .AddMembers(namespaceDecl);
    }
}
