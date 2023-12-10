using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
internal readonly struct IncludedVariablesFileName : IEquatable<IncludedVariablesFileName>
{
    private readonly string _fileName;
    private readonly bool _isInitialized;

    public IncludedVariablesFileName(string fileName)
    {
        _fileName = !string.IsNullOrEmpty(fileName)
            ? fileName
            : throw new ArgumentException($"'{nameof(fileName)}' cannot be null or empty.", nameof(fileName));

        _isInitialized = true;
    }

    public bool IsMatch(AdditionalText additionalText)
    {
        if (!_isInitialized)
        {
            return false;
        }

        return additionalText.Path.EndsWith(_fileName, StringComparison.Ordinal);
    }

    public override bool Equals(object obj)
    {
        return obj is IncludedVariablesFileName other && Equals(other);
    }

    public bool Equals(IncludedVariablesFileName other)
    {
        if (!_isInitialized)
        {
            return !other._isInitialized;
        }

        return StringComparer.Ordinal.Equals(_fileName, other._fileName);
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        if (!_isInitialized)
        {
            return hc.ToHashCode();
        }

        hc.Add(_fileName, StringComparer.Ordinal);

        return hc.ToHashCode();
    }

    public static bool operator ==(IncludedVariablesFileName lhs, IncludedVariablesFileName rhs)
    {
        return lhs.Equals(rhs);
    }

    public static bool operator !=(IncludedVariablesFileName lhs, IncludedVariablesFileName rhs)
    {
        return !lhs.Equals(rhs);
    }

    public static implicit operator string(IncludedVariablesFileName fileName)
    {
        if (!fileName._isInitialized)
        {
            return string.Empty;
        }

        return fileName._fileName ?? string.Empty;
    }
}
