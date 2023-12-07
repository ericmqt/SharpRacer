using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators;
internal static class NamedTypeSymbolExtensions
{
    public static string ToFullTypeName(this INamedTypeSymbol source)
    {
        if (source.ContainingNamespace != null)
        {
            return $"{source.ContainingNamespace}.{source.Name}";
        }

        return source.Name;
    }
}
