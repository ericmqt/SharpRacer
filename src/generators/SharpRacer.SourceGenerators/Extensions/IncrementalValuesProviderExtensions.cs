using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators;

public static class IncrementalValuesProviderExtensions
{
    public static IncrementalValuesProvider<T> WhereNotNull<T>(this IncrementalValuesProvider<T?> source)
        where T : class
    {
        return source.Where(static x => x != null)!;
    }
}