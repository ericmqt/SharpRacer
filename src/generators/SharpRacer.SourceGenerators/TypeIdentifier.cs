namespace SharpRacer.SourceGenerators;
public readonly struct TypeIdentifier
{
    private readonly string _globalQualifiedName;
    private readonly string _qualifiedName;

    public TypeIdentifier(string typeName, NamespaceIdentifier typeNamespace)
    {
        TypeName = !string.IsNullOrEmpty(typeName)
            ? typeName
            : throw new ArgumentException($"'{nameof(typeName)}' cannot be null or empty.", nameof(typeName));

        Namespace = typeNamespace;

        _qualifiedName = $"{Namespace}.{TypeName}";
        _globalQualifiedName = $"{Namespace.ToGlobalQualifiedName()}.{TypeName}";
    }

    public readonly NamespaceIdentifier Namespace { get; }
    public readonly string TypeName { get; }

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

    public static implicit operator string(TypeIdentifier identifier)
    {
        return identifier.TypeName ?? string.Empty;
    }
}
