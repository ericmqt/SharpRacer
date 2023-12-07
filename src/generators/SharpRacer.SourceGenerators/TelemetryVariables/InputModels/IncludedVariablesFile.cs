using System.Collections.Immutable;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.Json;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
internal readonly struct IncludedVariablesFile : IEquatable<IncludedVariablesFile>
{
    public IncludedVariablesFile(IncludedVariablesFileName fileName, AdditionalText file, SourceText sourceText)
    {
        FileName = fileName;
        File = file ?? throw new ArgumentNullException(nameof(file));
        SourceText = sourceText ?? throw new ArgumentNullException(nameof(sourceText));
        SourceLocationFactory = new JsonLocationFactory(File.Path, SourceText);
    }

    public readonly AdditionalText File { get; }
    public readonly IncludedVariablesFileName FileName { get; }
    public readonly JsonLocationFactory SourceLocationFactory { get; }
    public readonly SourceText SourceText { get; }

    public ImmutableArray<IncludedVariableNameValue> ReadJson(CancellationToken cancellationToken, out Diagnostic? diagnostic)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var json = SourceText.ToString();

        try
        {
            var variableNameValues = JsonSerializer.Deserialize(json, TelemetryGeneratorSerializationContext.Default.ImmutableArrayIncludedVariableNameValue);

            diagnostic = !variableNameValues.Any()
                ? IncludedVariablesDiagnostics.NoIncludedVariableNamesWarning(FileName)
                : null;

            return !variableNameValues.IsDefault ? variableNameValues : ImmutableArray<IncludedVariableNameValue>.Empty;
        }
        catch (JsonException jsonEx)
        {
            var errorLocation = SourceLocationFactory.GetLocation(jsonEx);

            diagnostic = IncludedVariablesDiagnostics.FileReadException(FileName, jsonEx, errorLocation);

            return ImmutableArray<IncludedVariableNameValue>.Empty;
        }
        catch (Exception ex)
        {
            diagnostic = IncludedVariablesDiagnostics.FileReadException(FileName, ex);

            return ImmutableArray<IncludedVariableNameValue>.Empty;
        }
    }

    public override bool Equals(object obj)
    {
        return obj is IncludedVariablesFile other && Equals(other);
    }

    public bool Equals(IncludedVariablesFile other)
    {
        // SourceText can be omitted because it is owned by AdditionalText
        return FileName == other.FileName &&
            File == other.File;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(FileName, File);
    }

    public static bool operator ==(IncludedVariablesFile left, IncludedVariablesFile right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(IncludedVariablesFile left, IncludedVariablesFile right)
    {
        return !left.Equals(right);
    }
}
