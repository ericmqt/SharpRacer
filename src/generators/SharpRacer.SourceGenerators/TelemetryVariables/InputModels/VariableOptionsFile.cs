using System.Collections.Immutable;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.Json;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
public readonly struct VariableOptionsFile : IEquatable<VariableOptionsFile>
{
    public VariableOptionsFile(VariableOptionsFileName fileName, AdditionalText file, SourceText sourceText)
    {
        FileName = fileName;
        File = file ?? throw new ArgumentNullException(nameof(file));
        SourceText = sourceText ?? throw new ArgumentNullException(nameof(sourceText));
        JsonLocationFactory = new JsonLocationFactory(File.Path, SourceText);
    }

    public AdditionalText File { get; }
    public VariableOptionsFileName FileName { get; }
    public JsonLocationFactory JsonLocationFactory { get; }
    public SourceText SourceText { get; }

    public ImmutableArray<JsonVariableOptions> Read(CancellationToken cancellationToken, out Diagnostic? diagnostic)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var json = SourceText.ToString();

        try
        {
            diagnostic = null;

            var options = JsonSerializer.Deserialize(json, TelemetryGeneratorSerializationContext.Default.ImmutableArrayJsonVariableOptions);

            return options;
        }
        catch (JsonException jsonEx)
        {
            var errorLocation = JsonLocationFactory.GetLocation(jsonEx);

            diagnostic = VariableOptionsDiagnostics.FileReadException(File.Path, jsonEx, errorLocation);

            return ImmutableArray<JsonVariableOptions>.Empty;
        }
        catch (Exception ex)
        {
            diagnostic = VariableOptionsDiagnostics.FileReadException(File.Path, ex);

            return ImmutableArray<JsonVariableOptions>.Empty;
        }
    }

    public override bool Equals(object obj)
    {
        return obj is VariableOptionsFile other && Equals(other);
    }

    public bool Equals(VariableOptionsFile other)
    {
        // SourceText can be omitted because it is owned by AdditionalText
        return FileName == other.FileName &&
            File == other.File;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(FileName, File);
    }

    public static bool operator ==(VariableOptionsFile left, VariableOptionsFile right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(VariableOptionsFile left, VariableOptionsFile right)
    {
        return !left.Equals(right);
    }
}
