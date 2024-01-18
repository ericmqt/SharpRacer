using System.Collections.Immutable;

namespace SharpRacer.SourceGenerators;
internal static class ImmutableArrayExtensions
{
    public static ImmutableArray<T> GetEmptyIfDefault<T>(this ImmutableArray<T> source)
    {
        return source.IsDefault ? ImmutableArray<T>.Empty : source;
    }

    public static bool SequenceEqualDefaultTolerant<T>(this ImmutableArray<T> source, ImmutableArray<T> other)
    {
        if (source.IsDefault && other.IsDefault)
        {
            return true;
        }

        if (source.IsDefault ^ other.IsDefault)
        {
            return false;
        }

        return source.SequenceEqual(other);
    }
}
