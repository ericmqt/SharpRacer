using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.Syntax;

namespace SharpRacer.SourceGenerators;
public readonly struct TypeIdentifier : IEquatable<TypeIdentifier>
{
    private readonly string _globalQualifiedName;
    private readonly string _qualifiedName;

    public TypeIdentifier(string typeName, NamespaceIdentifier typeNamespace)
    {
        TypeName = !string.IsNullOrEmpty(typeName)
            ? typeName
            : throw new ArgumentException($"'{nameof(typeName)}' cannot be null or empty.", nameof(typeName));

        Namespace = typeNamespace != default
            ? typeNamespace
            : throw new ArgumentException($"'{nameof(typeNamespace)}' cannot be a default value.", nameof(typeNamespace));

        _qualifiedName = $"{Namespace}.{TypeName}";
        _globalQualifiedName = $"{Namespace.ToGlobalQualifiedName()}.{TypeName}";
    }

    public readonly NamespaceIdentifier Namespace { get; }
    public readonly string TypeName { get; }

    public override bool Equals(object obj)
    {
        return obj is TypeIdentifier other && Equals(other);
    }

    public bool Equals(TypeIdentifier other)
    {
        return StringComparer.Ordinal.Equals(TypeName, other.TypeName) && Namespace == other.Namespace;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(TypeName, Namespace);
    }

    public readonly TypeSyntax ToGenericTypeSyntax(TypeArgumentListSyntax typeArgumentList, TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        var genericName = SyntaxFactory.GenericName(TypeName).WithTypeArgumentList(typeArgumentList);

        return typeNameFormat switch
        {
            TypeNameFormat.Qualified => TypeIdentifierSyntaxFactory.QualifiedTypeName(Namespace, genericName),
            TypeNameFormat.GlobalQualified => TypeIdentifierSyntaxFactory.GlobalQualifiedTypeName(Namespace, genericName),

            _ => genericName
        };
    }

    public readonly string ToGlobalQualifiedName()
    {
        return _globalQualifiedName ?? string.Empty;
    }

    public readonly string ToQualifiedName()
    {
        return _qualifiedName ?? string.Empty;
    }

    public override string ToString()
    {
        return _qualifiedName ?? string.Empty;
    }

    public readonly string ToString(TypeNameFormat typeNameFormat)
    {
        return typeNameFormat switch
        {
            TypeNameFormat.Qualified => _qualifiedName ?? string.Empty,
            TypeNameFormat.GlobalQualified => _globalQualifiedName ?? string.Empty,

            _ => TypeName ?? string.Empty
        };
    }

    public readonly TypeSyntax ToTypeSyntax(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        return typeNameFormat switch
        {
            TypeNameFormat.Qualified => TypeIdentifierSyntaxFactory.QualifiedTypeName(this),
            TypeNameFormat.GlobalQualified => TypeIdentifierSyntaxFactory.GlobalQualifiedTypeName(this),

            _ => SyntaxFactory.IdentifierName(TypeName)
        };
    }

    public static implicit operator string(TypeIdentifier identifier)
    {
        return identifier.TypeName ?? string.Empty;
    }

    public static bool operator ==(TypeIdentifier left, TypeIdentifier right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(TypeIdentifier left, TypeIdentifier right)
    {
        return !left.Equals(right);
    }
}
