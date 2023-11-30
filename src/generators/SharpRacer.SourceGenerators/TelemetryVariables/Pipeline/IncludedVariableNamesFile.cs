using System.Collections.Immutable;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.Configuration;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
internal class IncludedVariableNamesFile
{
    private readonly Diagnostic? _diagnostic;
    private readonly string _fileName;
    private readonly ImmutableArray<string> _variableNames;

    private IncludedVariableNamesFile(string fileName, ImmutableArray<string> variableNames)
    {
        _fileName = !string.IsNullOrEmpty(fileName)
            ? fileName
            : throw new ArgumentException($"'{nameof(fileName)}' cannot be null or empty.", nameof(fileName));

        _variableNames = variableNames;
    }

    private IncludedVariableNamesFile(string fileName, Diagnostic diagnostic)
    {
        _fileName = !string.IsNullOrEmpty(fileName)
            ? fileName
            : throw new ArgumentException($"'{nameof(fileName)}' cannot be null or empty.", nameof(fileName));

        _diagnostic = diagnostic ?? throw new ArgumentNullException(nameof(diagnostic));
    }

    public ImmutableArray<string> GetVariableNamesOrDefault(out Diagnostic? diagnostic)
    {
        if (!_variableNames.IsDefault)
        {
            diagnostic = null;
            return _variableNames;
        }

        diagnostic = _diagnostic;
        return ImmutableArray<string>.Empty;
    }

    public static IncludedVariableNamesFile? Create(
        VariableContextClassGeneratorInfo contextTypeInfo,
        ImmutableArray<AdditionalText> additionalTexts,
        CancellationToken cancellationToken = default)
    {
        if (contextTypeInfo is null)
        {
            throw new ArgumentNullException(nameof(contextTypeInfo));
        }

        if (string.IsNullOrEmpty(contextTypeInfo.IncludedVariableNamesArgumentValue))
        {
            return null;
        }

        return Create(contextTypeInfo.IncludedVariableNamesArgumentValue!, contextTypeInfo, additionalTexts, cancellationToken);
    }

    public static IncludedVariableNamesFile Create(
        string includedVariableNamesArgumentValue,
        VariableContextClassGeneratorInfo contextTypeInfo,
        ImmutableArray<AdditionalText> additionalTexts,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(includedVariableNamesArgumentValue))
        {
            throw new ArgumentException($"'{nameof(includedVariableNamesArgumentValue)}' cannot be null or empty.", nameof(includedVariableNamesArgumentValue));
        }

        if (contextTypeInfo is null)
        {
            throw new ArgumentNullException(nameof(contextTypeInfo));
        }

        var matches = additionalTexts.Where(x => x.Path.EndsWith(includedVariableNamesArgumentValue, StringComparison.OrdinalIgnoreCase));

        if (!matches.Any())
        {
            return new IncludedVariableNamesFile(
                includedVariableNamesArgumentValue,
                IncludedVariableNamesFileDiagnostics.FileNotFound(
                    contextTypeInfo.GetTargetClassFullTypeName(),
                    includedVariableNamesArgumentValue,
                    contextTypeInfo.GeneratorAttributeLocation));
        }

        if (matches.Count() > 1)
        {
            return new IncludedVariableNamesFile(
                includedVariableNamesArgumentValue,
                IncludedVariableNamesFileDiagnostics.AmbiguousFileName(
                    contextTypeInfo.GetTargetClassFullTypeName(),
                    includedVariableNamesArgumentValue,
                    contextTypeInfo.GeneratorAttributeLocation));
        }

        cancellationToken.ThrowIfCancellationRequested();

        // Read AdditionalText contents
        var fileText = matches.Single();

        var sourceText = fileText.GetText(cancellationToken);

        if (sourceText is null || !sourceText.TryGetString(out var json))
        {
            return new IncludedVariableNamesFile(
                includedVariableNamesArgumentValue,
                IncludedVariableNamesFileDiagnostics.FileContentReadFailure(
                    contextTypeInfo.GetTargetClassFullTypeName(),
                    fileText,
                    contextTypeInfo.GeneratorAttributeLocation));
        }

        cancellationToken.ThrowIfCancellationRequested();

        // Deserialize contents
        try
        {
            var variableNames = JsonSerializer.Deserialize<List<string>>(json, JsonSerializerConfiguration.SerializerOptions);

            if (variableNames is null)
            {
                return new IncludedVariableNamesFile(
                    includedVariableNamesArgumentValue,
                    IncludedVariableNamesFileDiagnostics.FileContentReadFailure(
                        contextTypeInfo.GetTargetClassFullTypeName(),
                        fileText,
                        contextTypeInfo.GeneratorAttributeLocation));
            }

            return new IncludedVariableNamesFile(includedVariableNamesArgumentValue, variableNames.ToImmutableArray());
        }
        catch (JsonException jsonEx)
        {
            var errorLocation = jsonEx.GetJsonTextLocation(fileText, sourceText);

            return new IncludedVariableNamesFile(
                includedVariableNamesArgumentValue,
                IncludedVariableNamesFileDiagnostics.FileReadException(contextTypeInfo.GetTargetClassFullTypeName(), fileText, jsonEx, errorLocation));
        }
        catch (Exception ex)
        {
            return new IncludedVariableNamesFile(
                includedVariableNamesArgumentValue,
                IncludedVariableNamesFileDiagnostics.FileReadException(contextTypeInfo.GetTargetClassFullTypeName(), fileText, ex));
        }
    }

    internal class EqualityComparer : IEqualityComparer<IncludedVariableNamesFile?>
    {
        public static IEqualityComparer<IncludedVariableNamesFile?> Default { get; } = new EqualityComparer();

        public bool Equals(IncludedVariableNamesFile? x, IncludedVariableNamesFile? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            if (!StringComparer.Ordinal.Equals(x._fileName, y._fileName))
            {
                return false;
            }

            if (!DiagnosticComparer.IdAndLocationComparer.Default.Equals(x._diagnostic, y._diagnostic))
            {
                return false;
            }

            if (x._variableNames.Length != y._variableNames.Length)
            {
                return false;
            }

            for (int i = 0; i < x._variableNames.Length; i++)
            {
                if (!StringComparer.Ordinal.Equals(x._variableNames[i], y._variableNames[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public int GetHashCode(IncludedVariableNamesFile? obj)
        {
            var hc = new HashCode();

            if (obj is null)
            {
                return hc.ToHashCode();
            }

            hc.Add(obj._fileName, StringComparer.Ordinal);
            hc.Add(obj._diagnostic, DiagnosticComparer.IdAndLocationComparer.Default);

            for (int i = 0; i < obj._variableNames.Length; i++)
            {
                hc.Add(obj._variableNames[i], StringComparer.Ordinal);
            }

            return hc.ToHashCode();
        }
    }
}
