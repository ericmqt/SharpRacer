using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators;
internal sealed class AttributeDataComparer : IEqualityComparer<AttributeData?>
{
    private readonly SymbolEqualityComparer _attributeClassComparer;
    private readonly SymbolEqualityComparer _constructorComparer;
    private readonly AttributeNamedArgumentsComparer _namedArgumentsComparer;

    public AttributeDataComparer(
        SymbolEqualityComparer attributeClassComparer,
        SymbolEqualityComparer constructorComparer,
        AttributeNamedArgumentsComparer namedArgumentsComparer)
    {
        _attributeClassComparer = attributeClassComparer ?? throw new ArgumentNullException(nameof(attributeClassComparer));
        _constructorComparer = constructorComparer ?? throw new ArgumentNullException(nameof(constructorComparer));
        _namedArgumentsComparer = namedArgumentsComparer ?? throw new ArgumentNullException(nameof(namedArgumentsComparer));
    }

    public static IEqualityComparer<AttributeData?> Default { get; }
        = new AttributeDataComparer(
            attributeClassComparer: SymbolEqualityComparer.Default,
            constructorComparer: SymbolEqualityComparer.Default,
            namedArgumentsComparer: AttributeNamedArgumentsComparer.Unordered);

    public bool Equals(AttributeData? x, AttributeData? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }

        if (!_attributeClassComparer.Equals(x.AttributeClass, y.AttributeClass))
        {
            return false;
        }

        if (!_constructorComparer.Equals(x.AttributeConstructor, y.AttributeConstructor))
        {
            return false;
        }

        if (!x.ConstructorArguments.SequenceEqual(y.ConstructorArguments))
        {
            return false;
        }

        return _namedArgumentsComparer.Equals(x.NamedArguments, y.NamedArguments);
    }

    public int GetHashCode(AttributeData? obj)
    {
        var hc = new HashCode();

        if (obj is null)
        {
            return hc.ToHashCode();
        }

        hc.Add(obj.AttributeClass, _attributeClassComparer);
        hc.Add(obj.AttributeConstructor, _constructorComparer);
        hc.Add(obj.NamedArguments, _namedArgumentsComparer);

        return hc.ToHashCode();
    }
}
