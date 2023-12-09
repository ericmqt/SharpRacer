using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

internal readonly struct VariableInfoFileName : IEquatable<VariableInfoFileName>
{
    private readonly static string _Default = "TelemetryVariables.json";
    private readonly string _fileName;

    public VariableInfoFileName()
    {
        _fileName = _Default;
    }

    public VariableInfoFileName(string fileName)
    {
        _fileName = !string.IsNullOrEmpty(fileName) ? fileName : _Default;
    }

    public static VariableInfoFileName GetFromConfigurationOrDefault(AnalyzerConfigOptionsProvider analyzerOptionsProvider)
    {
        if (analyzerOptionsProvider.GlobalOptions.TryGetValue(BuildPropertyKeys.TelemetryVariablesFileNameProperty, out var value))
        {
            if (string.IsNullOrEmpty(value))
            {
                return new VariableInfoFileName();
            }

            return new VariableInfoFileName(value);
        }

        return new VariableInfoFileName();
    }

    public bool IsMatch(AdditionalText additionalText)
    {
        return additionalText.Path.EndsWith(_fileName, StringComparison.Ordinal);
    }

    public static implicit operator string(VariableInfoFileName fileName)
    {
        return fileName._fileName;
    }

    #region IEquatable Implementation

    public override bool Equals(object obj)
    {
        if (obj is VariableInfoFileName other)
        {
            return Equals(other);
        }

        return false;
    }

    public bool Equals(VariableInfoFileName other)
    {
        return StringComparer.Ordinal.Equals(_fileName, other._fileName);
    }

    public override int GetHashCode()
    {
        return _fileName.GetHashCode();
    }

    public static bool operator ==(VariableInfoFileName lhs, VariableInfoFileName rhs)
    {
        return lhs.Equals(rhs);
    }

    public static bool operator !=(VariableInfoFileName lhs, VariableInfoFileName rhs)
    {
        return !lhs.Equals(rhs);
    }

    #endregion IEquatable Implementation
}
