namespace SharpRacer.SourceGenerators.Configuration;

public readonly struct MSBuildPropertyValue : IEquatable<MSBuildPropertyValue>
{
    public MSBuildPropertyValue(MSBuildPropertyKey propertyKey, string? value)
    {
        PropertyKey = propertyKey != default
            ? propertyKey
            : throw new ArgumentException($"'{nameof(propertyKey)}' cannot be a default value.", nameof(propertyKey));

        Value = value;
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
        return PropertyKey == other.PropertyKey && StringComparer.Ordinal.Equals(Value, other.Value);
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(PropertyKey);
        hc.Add(Value);

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
