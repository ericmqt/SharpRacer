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

        var namespaceDecl = NamespaceDeclaration(ParseName(model.ClassNamespace))
            .WithLeadingTrivia(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true)))
            .WithMembers(List(new MemberDeclarationSyntax[] { classDecl }));

        return CompilationUnit()
            .AddMembers(namespaceDecl);
    }

    public static ClassDeclarationSyntax CreateClassDeclaration(ref readonly VariableClassModel model, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var baseTypeList = BaseList(SeparatedList([model.BaseClassType(TypeNameFormat.GlobalQualified)]));

        var classMembers = CreateClassMembers(in model, cancellationToken);

        return ClassDeclaration(model.ClassName)
            .WithKeyword(Token(SyntaxKind.ClassKeyword))
            .WithModifiers(model.ClassAccessibility(), false, model.IsClassPartial)
            .WithBaseList(baseTypeList)
            .WithMembers(classMembers);
    }

    private static SyntaxList<MemberDeclarationSyntax> CreateClassMembers(ref readonly VariableClassModel model, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var members = new List<MemberDeclarationSyntax>()
        {
            model.DescriptorFieldDeclaration(),
            VariableClassSyntaxFactory.ConstructorWithDataVariableInfoParameter(in model, appendXmlDocumentation: true),
            VariableClassSyntaxFactory.ConstructorWithIDataVariableInfoProviderParameter(in model, appendXmlDocumentation: true)
        };

        return List(members);
    }
}
