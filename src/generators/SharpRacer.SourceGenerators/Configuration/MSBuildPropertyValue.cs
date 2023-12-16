namespace SharpRacer.SourceGenerators.Configuration;

public readonly struct MSBuildPropertyValue : IEquatable<MSBuildPropertyValue>
{
    private readonly bool _isInitialized;

    public MSBuildPropertyValue(MSBuildPropertyKey propertyKey, string? value)
    {
        PropertyKey = propertyKey;
        Value = value;

        _isInitialized = true;
    }

    public bool Exists => !string.IsNullOrWhiteSpace(Value);
    public readonly MSBuildPropertyKey PropertyKey { get; }
    public readonly string? Value { get; }

    public bool GetBooleanOrDefault(bool defaultValue = false)
    {
        if (Exists && bool.TryParse(Value, out var result))
        {
            return result;
        }

        return defaultValue;
    }

    public string GetValueOrDefault(string defaultValue)
    {
        return Exists ? Value! : defaultValue;
    }

    public override bool Equals(object obj)
    {
        return obj is MSBuildPropertyValue other && Equals(other);
    }

    public bool Equals(MSBuildPropertyValue other)
    {
        if (!_isInitialized)
        {
            return !other._isInitialized;
        }

        return PropertyKey == other.PropertyKey && StringComparer.Ordinal.Equals(Value, other.Value);
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        if (!_isInitialized)
        {
            return hc.ToHashCode();
        }

        hc.Add(PropertyKey);
        hc.Add(Value, StringComparer.Ordinal);

        return hc.ToHashCode();
    }

    public static bool operator ==(MSBuildPropertyValue left, MSBuildPropertyValue right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(MSBuildPropertyValue left, MSBuildPropertyValue right)
    {
        return !left.Equals(right);
    }
}
