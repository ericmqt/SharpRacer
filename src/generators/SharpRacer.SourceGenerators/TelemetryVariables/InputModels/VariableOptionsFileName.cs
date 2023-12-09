using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
internal readonly struct VariableOptionsFileName : IEquatable<VariableOptionsFileName>
{
    private readonly static string _Default = "TelemetryVariables.config.json";
    private readonly string _fileName;

    public VariableOptionsFileName()
    {
        _fileName = _Default;
    }

    public VariableOptionsFileName(string fileName)
    {
        _fileName = !string.IsNullOrEmpty(fileName) ? fileName : _Default;
    }

    public static VariableOptionsFileName FromConfigurationOrDefault(AnalyzerConfigOptionsProvider analyzerOptionsProvider)
    {
        if (analyzerOptionsProvider.GlobalOptions.TryGetValue(BuildPropertyKeys.VariableOptionsFileNameProperty, out var fileName))
        {
            return new VariableOptionsFileName(fileName);
        }

        return new VariableOptionsFileName();
    }

    public bool IsMatch(AdditionalText additionalText)
    {
        return additionalText.Path.EndsWith(_fileName, StringComparison.Ordinal);
    }

    public override bool Equals(object obj)
    {
        if (obj is VariableOptionsFileName other)
        {
            return Equals(other);
        }

        return false;
    }

    public bool Equals(VariableOptionsFileName other)
    {
        return StringComparer.Ordinal.Equals(_fileName, other._fileName);
    }

    public override int GetHashCode()
    {
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

    public static implicit operator string(VariableOptionsFileName variableOptionsFileName)
    {
        return variableOptionsFileName._fileName;
    }
}
