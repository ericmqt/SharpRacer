using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
public readonly struct VariableOptionsFileName : IEquatable<VariableOptionsFileName>
{
    private readonly string _fileName;

    public VariableOptionsFileName(string fileName)
    {
        _fileName = !string.IsNullOrEmpty(fileName)
            ? fileName
            : throw new ArgumentException($"'{nameof(fileName)}' cannot be null or empty.", nameof(fileName));
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
        return obj is VariableOptionsFileName other && Equals(other);
    }

    public bool Equals(VariableOptionsFileName other)
    {
        return StringComparer.Ordinal.Equals(_fileName, other._fileName);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_fileName);
    }

    public override string ToString()
    {
        return _fileName ?? string.Empty;
    }

    public static bool operator ==(VariableOptionsFileName lhs, VariableOptionsFileName rhs)
    {
        return lhs.Equals(rhs);
    }

    public static bool operator !=(VariableOptionsFileName lhs, VariableOptionsFileName rhs)
    {
        return !lhs.Equals(rhs);
    }

    public static implicit operator string(VariableOptionsFileName fileName)
    {
        return fileName._fileName ?? string.Empty;
    }
}
