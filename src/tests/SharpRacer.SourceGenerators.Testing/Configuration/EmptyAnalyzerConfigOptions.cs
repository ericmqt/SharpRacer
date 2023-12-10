using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace SharpRacer.SourceGenerators.Testing.Configuration;
internal class EmptyAnalyzerConfigOptions : AnalyzerConfigOptions
{
    public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
    {
        value = null;
        return false;
    }
}