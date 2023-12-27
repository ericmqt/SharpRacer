using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators;
internal static class HashCodeExtensions
{
    public static void AddDiagnosticArray(this HashCode hashCode, ImmutableArray<Diagnostic> diagnostics)
    {
        if (diagnostics.IsDefault)
        {
            return;
        }

        for (int i = 0; i < diagnostics.Length; i++)
        {
            hashCode.Add(diagnostics[i]);
        }
    }

    public static void AddImmutableArray<T>(this HashCode hashCode, ImmutableArray<T> collection)
    {
        if (collection.IsDefaultOrEmpty)
        {
            return;
        }

        for (int i = 0; i < collection.Length; i++)
        {
            hashCode.Add(collection[i]);
        }
    }
}
