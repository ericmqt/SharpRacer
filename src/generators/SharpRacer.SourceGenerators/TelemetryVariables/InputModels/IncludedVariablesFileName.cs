using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
internal readonly struct IncludedVariablesFileName : IEquatable<IncludedVariablesFileName>
{
    private readonly string _fileName;

    public IncludedVariablesFileName()
    {
        _fileName = string.Empty;

        IsDefault = true;
    }

    public IncludedVariablesFileName(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentException($"'{nameof(fileName)}' cannot be null or empty.", nameof(fileName));
        }

        _fileName = fileName;
    }

    public bool IsDefault { get; }

    public bool IsMatch(AdditionalText additionalText)
    {
        if (IsDefault)
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
        return StringComparer.Ordinal.Equals(_fileName, other._fileName);
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

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
        return fileName._fileName ?? string.Empty;
    }
}
