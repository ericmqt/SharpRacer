using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators;
internal static class DiagnosticExtensions
{
    public static bool IsError(this Diagnostic diagnostic)
    {
        return diagnostic.Severity == DiagnosticSeverity.Error || diagnostic.IsWarningAsError;
    }

    public static bool HasErrors(this ImmutableArray<Diagnostic> source)
    {
        if (source.IsDefaultOrEmpty || source.Length == 0)
        {
            return false;
        }

        return source.Any(x => x.Severity == DiagnosticSeverity.Error || x.IsWarningAsError);
    }
}
