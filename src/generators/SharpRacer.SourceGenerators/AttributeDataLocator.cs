using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators;
public static class AttributeDataLocator
{
    public static AttributeData? Find(
        INamedTypeSymbol targetClassSymbol,
        INamedTypeSymbol attributeSymbol,
        out Location? attributeLocation)
    {
        if (targetClassSymbol is null)
        {
            throw new ArgumentNullException(nameof(targetClassSymbol));
        }

        if (attributeSymbol is null)
        {
            throw new ArgumentNullException(nameof(attributeSymbol));
        }

        attributeLocation = null;

        // Examine attribute
        var targetAttributeData = targetClassSymbol.GetAttributes()
            .SingleOrDefault(x => attributeSymbol.Equals(x.AttributeClass, SymbolEqualityComparer.Default));

        if (targetAttributeData is null)
        {
            return null;
        }

        // Set the attribute source location, if possible
        if (targetAttributeData.ApplicationSyntaxReference != null)
        {
            attributeLocation = Location.Create(
                targetAttributeData.ApplicationSyntaxReference.SyntaxTree,
                targetAttributeData.ApplicationSyntaxReference.Span);
        }

        return targetAttributeData;
    }

    public static bool TryFind(
        INamedTypeSymbol targetClassSymbol,
        INamedTypeSymbol attributeSymbol,
        out AttributeData? attributeData,
        out Location? attributeLocation)
    {
        attributeData = Find(targetClassSymbol, attributeSymbol, out attributeLocation);

        return attributeData != null;
    }
}
