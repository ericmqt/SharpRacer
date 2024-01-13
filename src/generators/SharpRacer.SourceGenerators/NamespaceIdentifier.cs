using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators;
public readonly struct NamespaceIdentifier : IEquatable<NamespaceIdentifier>, IEquatable<INamespaceSymbol>
{
    private readonly string _namespace;

    public NamespaceIdentifier(string namespaceIdentifier)
    {
        _namespace = !string.IsNullOrEmpty(namespaceIdentifier)
            ? namespaceIdentifier
            : throw new ArgumentException($"'{nameof(namespaceIdentifier)}' cannot be null or empty.", nameof(namespaceIdentifier));
    }

    public readonly TypeIdentifier CreateType(string typeName)
    {
        if (string.IsNullOrEmpty(typeName))
        {
            throw new ArgumentException($"'{nameof(typeName)}' cannot be null or empty.", nameof(typeName));
        }

        return new TypeIdentifier(typeName, this);
    }

    public override bool Equals(object obj)
    {
        return obj is NamespaceIdentifier other && Equals(other);
    }

    public bool Equals(NamespaceIdentifier other)
    {
        return StringComparer.Ordinal.Equals(_namespace, other._namespace);
    }

    public bool Equals(INamespaceSymbol namespaceSymbol)
    {
        if (_namespace == null)
        {
            return false;
        }

        return namespaceSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Equals(ToGlobalQualifiedName());
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_namespace);
    }

    public readonly string ToGlobalQualifiedName()
    {
        if (_namespace == null)
        {
            return string.Empty;
        }

        return $"global::{_namespace}";
    }

    public override string ToString()
    {
        return _namespace ?? string.Empty;
    }

    public readonly string ToString(bool qualifyGlobal)
    {
        if (_namespace == null)
        {
            return string.Empty;
        }

        if (qualifyGlobal)
        {
            return $"global::{_namespace}";
        }

        return _namespace;
    }

    public static implicit operator string(NamespaceIdentifier namespaceIdentifier)
    {
        return namespaceIdentifier._namespace ?? string.Empty;
    }

    public static bool operator ==(NamespaceIdentifier left, NamespaceIdentifier right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(NamespaceIdentifier left, NamespaceIdentifier right)
    {
        return !left.Equals(right);
    }
}
