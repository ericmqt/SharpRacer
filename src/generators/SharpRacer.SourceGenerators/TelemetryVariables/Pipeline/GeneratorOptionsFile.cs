using System.Text.Json;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.Configuration;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
internal class GeneratorOptionsFile
{
    private readonly Diagnostic? _diagnostic;
    private readonly GeneratorOptions? _options;

    private GeneratorOptionsFile(string path, GeneratorOptions generatorOptions)
    {
        Path = !string.IsNullOrEmpty(path)
            ? path
            : throw new ArgumentException($"'{nameof(path)}' cannot be null or empty.", nameof(path));

        _options = generatorOptions ?? throw new ArgumentNullException(nameof(generatorOptions));
    }

    private GeneratorOptionsFile(string path, Diagnostic diagnostic)
    {
        Path = !string.IsNullOrEmpty(path)
            ? path
            : throw new ArgumentException($"'{nameof(path)}' cannot be null or empty.", nameof(path));

        _diagnostic = diagnostic ?? throw new ArgumentNullException(nameof(diagnostic));
    }

    public string Path { get; }

    public GeneratorOptions GetOptionsOrDefault(out Diagnostic? diagnostic)
    {
        if (_options != null)
        {
            diagnostic = null;

            return _options;
        }

        diagnostic = _diagnostic;

        return new GeneratorOptions();
    }

    public static GeneratorOptionsFile Create(AdditionalText additionalText, CancellationToken cancellationToken = default)
    {
        if (additionalText is null)
        {
            throw new ArgumentNullException(nameof(additionalText));
        }

        var sourceText = additionalText.GetText(cancellationToken);

        if (sourceText is null || !sourceText.TryGetString(out var optionsJson))
        {
            return new GeneratorOptionsFile(
                additionalText.Path,
                GeneratorOptionsFileDiagnostics.FileContentReadFailure(additionalText));
        }

        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var deserializedOptions = JsonSerializer.Deserialize<GeneratorOptions>(optionsJson, JsonSerializerConfiguration.SerializerOptions);

            if (deserializedOptions is null)
            {
                return new GeneratorOptionsFile(
                    additionalText.Path,
                    GeneratorOptionsFileDiagnostics.FileContentReadFailure(additionalText));
            }

            return new GeneratorOptionsFile(additionalText.Path, deserializedOptions);
        }
        catch (JsonException jsonEx)
        {
            var errorLocation = jsonEx.GetJsonTextLocation(additionalText, sourceText);

            return new GeneratorOptionsFile(
                additionalText.Path,
                GeneratorOptionsFileDiagnostics.FileReadException(additionalText, jsonEx, errorLocation));
        }
        catch (Exception ex)
        {
            return new GeneratorOptionsFile(
                additionalText.Path,
                GeneratorOptionsFileDiagnostics.FileReadException(additionalText, ex));
        }
    }

    internal class EqualityComparer : IEqualityComparer<GeneratorOptionsFile?>
    {
        public static IEqualityComparer<GeneratorOptionsFile?> Default { get; } = new EqualityComparer();

        private EqualityComparer() { }

        public bool Equals(GeneratorOptionsFile? x, GeneratorOptionsFile? y)
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

            if (!GeneratorOptionsComparer.Default.Equals(x._options, y._options))
            {
                return false;
            }

            return DiagnosticComparer.IdAndLocationComparer.Default.Equals(x._diagnostic, y._diagnostic);
        }

        public int GetHashCode(GeneratorOptionsFile? obj)
        {
            var hc = new HashCode();

            if (obj is null)
            {
                return hc.ToHashCode();
            }

            hc.Add(obj.Path, StringComparer.Ordinal);
            hc.Add(obj._options, GeneratorOptionsComparer.Default);
            hc.Add(obj._diagnostic, DiagnosticComparer.IdAndLocationComparer.Default);

            return hc.ToHashCode();
        }
    }

}
