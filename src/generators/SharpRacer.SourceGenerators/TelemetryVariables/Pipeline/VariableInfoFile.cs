using System.Collections.Immutable;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.Configuration;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
internal class VariableInfoFile
{
    private readonly Diagnostic? _diagnostic;
    private readonly ImmutableArray<VariableInfo> _variables;

    private VariableInfoFile(string path, ImmutableArray<VariableInfo> variables)
    {
        Path = !string.IsNullOrEmpty(path)
            ? path
            : throw new ArgumentException($"'{nameof(path)}' cannot be null or empty.", nameof(path));

        _variables = variables;
    }

    private VariableInfoFile(string path, Diagnostic diagnostic)
    {
        Path = !string.IsNullOrEmpty(path)
            ? path
            : throw new ArgumentException($"'{nameof(path)}' cannot be null or empty.", nameof(path));

        _diagnostic = diagnostic ?? throw new ArgumentNullException(nameof(diagnostic));
    }

    public string Path { get; }

    public ImmutableArray<VariableInfo> GetVariablesOrDefault(out Diagnostic? diagnostic)
    {
        if (!_variables.IsDefault)
        {
            diagnostic = null;

            return _variables;
        }

        diagnostic = _diagnostic;

        return ImmutableArray<VariableInfo>.Empty;
    }

    public static VariableInfoFile Create(AdditionalText additionalText, CancellationToken cancellationToken = default)
    {
        if (additionalText is null)
        {
            throw new ArgumentNullException(nameof(additionalText));
        }

        var sourceText = additionalText.GetText(cancellationToken);

        if (sourceText is null || !sourceText.TryGetString(out var variablesJson))
        {
            return new VariableInfoFile(
                additionalText.Path,
                VariablesFileDiagnostics.FileContentReadFailure(additionalText));
        }

        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var variables = JsonSerializer.Deserialize<List<VariableInfo>>(variablesJson, JsonSerializerConfiguration.SerializerOptions);

            if (variables is null)
            {
                return new VariableInfoFile(
                    additionalText.Path,
                    VariablesFileDiagnostics.FileContentReadFailure(additionalText));
            }

            return new VariableInfoFile(additionalText.Path, variables.ToImmutableArray());
        }
        catch (JsonException jsonEx)
        {
            var errorLocation = jsonEx.GetJsonTextLocation(additionalText, sourceText);

            return new VariableInfoFile(
                additionalText.Path,
                VariablesFileDiagnostics.FileReadException(additionalText, jsonEx, errorLocation));
        }
        catch (Exception ex)
        {
            return new VariableInfoFile(
                additionalText.Path,
                VariablesFileDiagnostics.FileReadException(additionalText, ex));
        }
    }

    internal class EqualityComparer : IEqualityComparer<VariableInfoFile?>
    {
        private EqualityComparer() { }

        public static IEqualityComparer<VariableInfoFile?> Default { get; } = new EqualityComparer();

        public bool Equals(VariableInfoFile? x, VariableInfoFile? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            if (!StringComparer.Ordinal.Equals(x.Path, y.Path))
            {
                return false;
            }

            if (!DiagnosticComparer.IdAndLocationComparer.Default.Equals(x._diagnostic, y._diagnostic))
            {
                return false;
            }

            if (x._variables.Length != y._variables.Length)
            {
                return false;
            }

            for (int i = 0; i < x._variables.Length; i++)
            {
                if (!VariableInfo.EqualityComparer.Default.Equals(x._variables[i], y._variables[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public int GetHashCode(VariableInfoFile? obj)
        {
            var hc = new HashCode();

            if (obj is null)
            {
                return hc.ToHashCode();
            }

            hc.Add(obj.Path, StringComparer.Ordinal);
            hc.Add(obj._diagnostic, DiagnosticComparer.IdAndLocationComparer.Default);

            for (int i = 0; i < obj._variables.Length; i++)
            {
                hc.Add(obj._variables[i], VariableInfo.EqualityComparer.Default);
            }

            return hc.ToHashCode();
        }
    }
}
