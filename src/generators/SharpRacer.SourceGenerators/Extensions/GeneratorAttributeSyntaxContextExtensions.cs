using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators;
internal static class GeneratorAttributeSyntaxContextExtensions
{
    public static bool TryGetClassSymbolWithAttribute(
        this GeneratorAttributeSyntaxContext context,
        string attributeFullTypeName,
        CancellationToken cancellationToken,
        out INamedTypeSymbol? targetClassSymbol,
        out AttributeData? attributeData,
        out Location? attributeLocation)
    {
        if (string.IsNullOrEmpty(attributeFullTypeName))
        {
            throw new ArgumentException($"'{nameof(attributeFullTypeName)}' cannot be null or empty.", nameof(attributeFullTypeName));
        }

        cancellationToken.ThrowIfCancellationRequested();

        // Ensure we have a named type
        targetClassSymbol = context.TargetSymbol as INamedTypeSymbol;

        if (targetClassSymbol is null || targetClassSymbol.ContainingNamespace.IsGlobalNamespace)
        {
            attributeData = null;
            attributeLocation = null;

            return false;
        }

        attributeData = context.Attributes.First();
        attributeLocation = attributeData.GetLocation();

        return true;
    }
}
