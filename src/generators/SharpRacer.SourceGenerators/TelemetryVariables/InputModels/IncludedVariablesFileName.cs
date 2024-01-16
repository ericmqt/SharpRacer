using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
public readonly struct IncludedVariablesFileName : IEquatable<IncludedVariablesFileName>
{
    private readonly string _fileName;

    public IncludedVariablesFileName(string fileName)
    {
        _fileName = !string.IsNullOrEmpty(fileName)
            ? fileName
            : throw new ArgumentException($"'{nameof(fileName)}' cannot be null or empty.", nameof(fileName));
    }

    public static IncludedVariablesFileName CreateOrGetDefault(string? fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return default;
        }

        return new IncludedVariablesFileName(fileName!);
    }

    public bool IsMatch(AdditionalText additionalText)
    {
        // Return false if uninitialized
        if (_fileName == null)
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
        return HashCode.Combine(_fileName);
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
