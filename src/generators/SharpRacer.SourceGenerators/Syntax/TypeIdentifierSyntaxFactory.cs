using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.Syntax;
internal static class TypeIdentifierSyntaxFactory
{
    public static QualifiedNameSyntax GlobalQualifiedTypeName(TypeIdentifier typeIdentifier)
    {
        return GlobalQualifiedTypeName(typeIdentifier.Namespace, IdentifierName(typeIdentifier.TypeName));
    }

    public static QualifiedNameSyntax GlobalQualifiedTypeName(NamespaceIdentifier namespaceIdentifier, SimpleNameSyntax typeName)
    {
        var namespaceParts = namespaceIdentifier.ToString()
            .Split('.')
            .Select(IdentifierName)
            .ToList();

        return QualifiedTypeName(
            AliasQualifiedName(
                IdentifierName(Token(SyntaxKind.GlobalKeyword)),
                namespaceParts.First()),
            namespaceParts.Skip(1),
            typeName);
    }

    public static QualifiedNameSyntax QualifiedTypeName(TypeIdentifier typeIdentifier)
    {
        return QualifiedTypeName(typeIdentifier.Namespace, IdentifierName(typeIdentifier.TypeName));
    }

    public static QualifiedNameSyntax QualifiedTypeName(NamespaceIdentifier namespaceIdentifier, SimpleNameSyntax typeName)
    {
        var namespaceParts = namespaceIdentifier.ToString()
            .Split('.')
            .Select(IdentifierName)
            .ToList();

        if (namespaceParts.Count == 1)
        {
            return QualifiedName(namespaceParts[0], typeName);
        }

        return QualifiedTypeName(
            QualifiedName(namespaceParts[0], namespaceParts[1]),
            namespaceParts.Skip(2),
            typeName);
    }

    private static QualifiedNameSyntax QualifiedTypeName(
        NameSyntax parentName,
        IEnumerable<IdentifierNameSyntax> descendantIdentifiers,
        SimpleNameSyntax typeName)
    {
        if (!descendantIdentifiers.Any())
        {
            return QualifiedName(parentName, typeName);
        }

        NameSyntax last = QualifiedName(parentName, descendantIdentifiers.First());

        foreach (var idPart in descendantIdentifiers.Skip(1))
        {
            last = QualifiedName(last, idPart);
        }

        return QualifiedName(last, typeName);
    }
}
