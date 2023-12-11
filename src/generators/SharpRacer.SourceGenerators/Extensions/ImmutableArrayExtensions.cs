using System.Collections.Immutable;

namespace SharpRacer.SourceGenerators;
internal static class ImmutableArrayExtensions
{
    public static ImmutableArray<T> GetEmptyIfDefault<T>(this ImmutableArray<T> source)
    {
        return source.IsDefault ? ImmutableArray<T>.Empty : source;
    }
}
