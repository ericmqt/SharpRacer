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

    public ImmutableArray<VariableInfo> Read(CancellationToken cancellationToken, out Diagnostic? diagnostic)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var json = SourceText.ToString();

        try
        {
            var jsonVariables = JsonSerializer.Deserialize(json, TelemetryGeneratorSerializationContext.Default.ImmutableArrayVariableInfo);

            diagnostic = !jsonVariables.Any()
                ? GeneratorDiagnostics.TelemetryVariablesFileContainsNoVariables(FileName)
                : null;

            return jsonVariables;
        }
        catch (JsonException jsonEx)
        {
            diagnostic = GeneratorDiagnostics.AdditionalTextFileReadException(File, jsonEx, SourceLocationFactory.GetLocation(jsonEx));

            return ImmutableArray<VariableInfo>.Empty;
        }
        catch (Exception ex)
        {
            diagnostic = GeneratorDiagnostics.AdditionalTextFileReadException(File, ex);

            return ImmutableArray<VariableInfo>.Empty;
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
        var hc = new HashCode();

        hc.Add(FileName);
        hc.Add(File);

        return hc.ToHashCode();
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
