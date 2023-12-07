using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
internal readonly struct ClassWithGeneratorAttribute : IEquatable<ClassWithGeneratorAttribute>
{
    public ClassWithGeneratorAttribute(INamedTypeSymbol classSymbol, AttributeData attributeData, Location? attributeLocation)
    {
        ClassSymbol = classSymbol;
        AttributeData = attributeData;
        AttributeLocation = attributeLocation;
    }

    public AttributeData AttributeData { get; }
    public Location? AttributeLocation { get; }
    public INamedTypeSymbol ClassSymbol { get; }

    public override bool Equals(object obj)
    {
        return obj is ClassWithGeneratorAttribute other && Equals(other);
    }

    public bool Equals(ClassWithGeneratorAttribute other)
    {
        return SymbolEqualityComparer.Default.Equals(ClassSymbol, other.ClassSymbol) &&
            AttributeDataComparer.Default.Equals(AttributeData, other.AttributeData) &&
            AttributeLocation == other.AttributeLocation;
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(ClassSymbol, SymbolEqualityComparer.Default);
        hc.Add(AttributeData, AttributeDataComparer.Default);
        hc.Add(AttributeLocation);

        return hc.ToHashCode();
    }

    public static bool operator ==(ClassWithGeneratorAttribute lhs, ClassWithGeneratorAttribute rhs)
    {
        return lhs.Equals(rhs);
    }

    public static bool operator !=(ClassWithGeneratorAttribute lhs, ClassWithGeneratorAttribute rhs)
    {
        return !lhs.Equals(rhs);
    }
}
