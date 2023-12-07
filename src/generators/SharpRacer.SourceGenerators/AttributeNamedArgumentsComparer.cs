using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators;
internal class AttributeNamedArgumentsComparer : IEqualityComparer<ImmutableArray<KeyValuePair<string, TypedConstant>>>
{
    private readonly StringComparer _keyComparer;
    private readonly bool _unordered;

    private AttributeNamedArgumentsComparer(StringComparer keyComparer, bool unordered)
    {
        _keyComparer = keyComparer ?? throw new ArgumentNullException(nameof(keyComparer));
        _unordered = unordered;
    }

    public static AttributeNamedArgumentsComparer Ordered { get; }
        = new AttributeNamedArgumentsComparer(StringComparer.Ordinal, unordered: false);

    public static AttributeNamedArgumentsComparer Unordered { get; }
        = new AttributeNamedArgumentsComparer(StringComparer.Ordinal, unordered: true);

    public bool Equals(ImmutableArray<KeyValuePair<string, TypedConstant>> x, ImmutableArray<KeyValuePair<string, TypedConstant>> y)
    {
        return _unordered ? UnorderedEquals(x, y) : SequenceEquals(x, y);
    }

    private bool SequenceEquals(ImmutableArray<KeyValuePair<string, TypedConstant>> x, ImmutableArray<KeyValuePair<string, TypedConstant>> y)
    {
        if (x.Length != y.Length)
        {
            return false;
        }

        for (int i = 0; i < x.Length; i++)
        {
            if (!_keyComparer.Equals(x[i].Key, y[i].Key))
            {
                return false;
            }

            if (x[i].Value.Equals(y[i].Value))
            {
                return false;
            }
        }

        return true;
    }

    private bool UnorderedEquals(ImmutableArray<KeyValuePair<string, TypedConstant>> x, ImmutableArray<KeyValuePair<string, TypedConstant>> y)
    {
        if (x.Length != y.Length)
        {
            return false;
        }

        for (int i = 0; i < x.Length; i++)
        {
            var xValue = x[i].Value;
            var yValues = y.Where(kvp => _keyComparer.Equals(x[i].Key, kvp.Key));

            if (!yValues.Any(kvpY => xValue.Equals(kvpY.Value)))
            {
                return false;
            }
        }

        return true;
    }

    public int GetHashCode(ImmutableArray<KeyValuePair<string, TypedConstant>> obj)
    {
        var hc = new HashCode();

        for (int i = 0; i < obj.Length; i++)
        {
            hc.Add(obj[i].Key, _keyComparer);
            hc.Add(obj[i].Value);
        }

        return hc.ToHashCode();
    }
}
