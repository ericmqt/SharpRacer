using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
internal readonly struct ContextClassInfo : IEquatable<ContextClassInfo>
{
    public ContextClassInfo(INamedTypeSymbol classSymbol, Location generatorAttributeLocation)
    {
        ClassName = classSymbol.Name;
        ClassNamespace = classSymbol.ContainingNamespace.ToString();
        GeneratorAttributeLocation = generatorAttributeLocation;
    }

    private ContextClassInfo(
        string className,
        string classNamespace,
        Location generatorAttributeLocation,
        IncludedVariables includedVariables)
    {
        ClassName = className;
        ClassNamespace = classNamespace;
        GeneratorAttributeLocation = generatorAttributeLocation;
        IncludedVariables = includedVariables;
    }

    public readonly string ClassName { get; }
    public readonly string ClassNamespace { get; }
    public readonly Location GeneratorAttributeLocation { get; }
    public readonly IncludedVariables IncludedVariables { get; }

    public readonly string ToFullyQualifiedName()
    {
        return $"{ClassNamespace}.{ClassName}";
    }

    public ContextClassInfo WithIncludedVariables(IncludedVariables includedVariables)
    {
        return new ContextClassInfo(
            ClassName,
            ClassNamespace,
            GeneratorAttributeLocation,
            includedVariables);
    }

    public override bool Equals(object obj)
    {
        return obj is ContextClassInfo other && Equals(other);
    }

    public bool Equals(ContextClassInfo other)
    {
        return StringComparer.Ordinal.Equals(ClassName, other.ClassName) &&
            StringComparer.Ordinal.Equals(ClassNamespace, other.ClassNamespace) &&
            GeneratorAttributeLocation == other.GeneratorAttributeLocation &&
            IncludedVariables == other.IncludedVariables;
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(ClassName);
        hc.Add(ClassNamespace);
        hc.Add(GeneratorAttributeLocation);
        hc.Add(IncludedVariables);

        return hc.ToHashCode();
    }

    public static bool operator ==(ContextClassInfo left, ContextClassInfo right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ContextClassInfo left, ContextClassInfo right)
    {
        return !left.Equals(right);
    }
}
