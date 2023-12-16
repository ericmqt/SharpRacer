using System.Collections.Immutable;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.Json;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
public readonly struct VariableInfoFile : IEquatable<VariableInfoFile>
{
    public VariableInfoFile(VariableInfoFileName fileName, AdditionalText file, SourceText sourceText)
    {
        FileName = fileName;
        File = file ?? throw new ArgumentNullException(nameof(file));
        SourceText = sourceText ?? throw new ArgumentNullException(nameof(sourceText));
        SourceLocationFactory = new JsonLocationFactory(File.Path, SourceText);
    }

    public readonly AdditionalText File { get; }
    public readonly VariableInfoFileName FileName { get; }
    public readonly JsonLocationFactory SourceLocationFactory { get; }
    public readonly SourceText SourceText { get; }

    public ImmutableArray<JsonVariableInfo> Read(CancellationToken cancellationToken, out Diagnostic? diagnostic)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var json = SourceText.ToString();

        try
        {
            var jsonVariables = JsonSerializer.Deserialize(json, TelemetryGeneratorSerializationContext.Default.ImmutableArrayJsonVariableInfo);

            diagnostic = !jsonVariables.Any()
                ? VariableInfoDiagnostics.NoVariablesDefinedInFile(FileName)
                : null;

            return !jsonVariables.IsDefault
                ? jsonVariables
                : ImmutableArray<JsonVariableInfo>.Empty;
        }
        catch (JsonException jsonEx)
        {
            diagnostic = VariableInfoDiagnostics.FileReadException(File.Path, jsonEx, SourceLocationFactory.GetLocation(jsonEx));

            return ImmutableArray<JsonVariableInfo>.Empty;
        }
        catch (Exception ex)
        {
            diagnostic = VariableInfoDiagnostics.FileReadException(File.Path, ex);

            return ImmutableArray<JsonVariableInfo>.Empty;
        }
    }

    public override bool Equals(object obj)
    {
        return obj is VariableInfoFile other && Equals(other);
    }

    public bool Equals(VariableInfoFile other)
    {
        // SourceText can be omitted because it is owned by AdditionalText
        return FileName == other.FileName &&
            File == other.File;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(FileName, File);
    }

    public static bool operator ==(VariableInfoFile left, VariableInfoFile right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(VariableInfoFile left, VariableInfoFile right)
    {
        return !left.Equals(right);
    }
}
