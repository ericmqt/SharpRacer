using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
public readonly struct VariableOptionsFileName : IEquatable<VariableOptionsFileName>
{
    private readonly string _fileName;
    private readonly bool _isInitialized;

    public VariableOptionsFileName(string fileName)
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
        return obj is VariableOptionsFileName other && Equals(other);
    }

    public bool Equals(VariableOptionsFileName other)
    {
        if (!_isInitialized)
        {
            return !other._isInitialized;
        }

        return StringComparer.Ordinal.Equals(_fileName, other._fileName);
    }

    public override int GetHashCode()
    {
        if (!_isInitialized)
        {
            return 0;
        }

        return _fileName.GetHashCode();
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
        if (!fileName._isInitialized)
        {
            return string.Empty;
        }

        return fileName._fileName;
    }
}
