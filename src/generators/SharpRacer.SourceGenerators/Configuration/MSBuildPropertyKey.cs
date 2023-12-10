namespace SharpRacer.SourceGenerators.Configuration;

public readonly struct MSBuildPropertyKey : IEquatable<MSBuildPropertyKey>
{
    private readonly bool _isInitialized;

    private MSBuildPropertyKey(string key, string propertyName)
    {
        Key = key;
        PropertyName = propertyName;

        _isInitialized = true;
    }

    public readonly string PropertyName { get; }
    public readonly string Key { get; }

    public static MSBuildPropertyKey FromPropertyName(string propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
        {
            throw new ArgumentException($"'{nameof(propertyName)}' cannot be null or empty.", nameof(propertyName));
        }

        return new MSBuildPropertyKey($"build_property.{propertyName}", propertyName);
    }

    public override bool Equals(object obj)
    {
        return obj is MSBuildPropertyKey other && Equals(other);
    }

    public bool Equals(MSBuildPropertyKey other)
    {
        if (!_isInitialized)
        {
            return !other._isInitialized;
        }

        return StringComparer.OrdinalIgnoreCase.Equals(Key, other.Key);
    }

    public override int GetHashCode()
    {
        if (!_isInitialized)
        {
            return 0;
        }

        return Key.GetHashCode();
    }

    public static bool operator ==(MSBuildPropertyKey left, MSBuildPropertyKey right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(MSBuildPropertyKey left, MSBuildPropertyKey right)
    {
        return !left.Equals(right);
    }
}
