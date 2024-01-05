namespace SharpRacer.SourceGenerators;
public readonly struct NamespaceIdentifier
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
        return new TypeIdentifier(typeName, this);
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
}
