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
}
