using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.Json;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
internal class VariableOptionsFactory
{
    private readonly ImmutableArray<VariableOptions>.Builder _builder;
    private readonly JsonLocationFactory _locationFactory;

    public VariableOptionsFactory(VariableOptionsFile variableOptionsFile, int initialCapacity)
    {
        _locationFactory = variableOptionsFile.JsonLocationFactory;

        _builder = ImmutableArray.CreateBuilder<VariableOptions>(initialCapacity);
    }

    public ImmutableArray<VariableOptions> Build()
    {
        return _builder.ToImmutable();
    }

    public bool TryAdd(JsonVariableOptions jsonVariableOptions, out ImmutableArray<Diagnostic> diagnostics)
    {
        if (jsonVariableOptions == default)
        {
            diagnostics = ImmutableArray<Diagnostic>.Empty;
            return false;
        }

        diagnostics = GetDiagnostics(jsonVariableOptions);

        if (diagnostics.HasErrors())
        {
            return false;
        }

        var variableOptions = new VariableOptions(
            jsonVariableOptions,
            GetKeyLocation(jsonVariableOptions)!,
            GetValueLocation(jsonVariableOptions)!);

        _builder.Add(variableOptions);

        return true;
    }

    private ImmutableArray<Diagnostic> GetDiagnostics(JsonVariableOptions jsonVariableOptions)
    {
        var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>(initialCapacity: 4);

        if (TryGetDuplicateKeyDiagnostic(jsonVariableOptions, out var duplicateKeyDiagnostic))
        {
            diagnosticsBuilder.Add(duplicateKeyDiagnostic!);
        }

        if (TryGetDuplicatedClassNameDiagnostic(jsonVariableOptions, out var duplicateContextPropertyNameDiagnostic))
        {
            diagnosticsBuilder.Add(duplicateContextPropertyNameDiagnostic!);
        }

        if (TryGetDuplicatedNameDiagnostic(jsonVariableOptions, out var duplicateNameDiagnostic))
        {
            diagnosticsBuilder.Add(duplicateNameDiagnostic!);
        }

        return diagnosticsBuilder.ToImmutable();
    }

    private bool TryGetDuplicateKeyDiagnostic(JsonVariableOptions jsonVariableOptions, out Diagnostic? diagnostic)
    {
        var duplicate = _builder.FirstOrDefault(x => x.VariableKey.Equals(jsonVariableOptions.Key, StringComparison.Ordinal));

        if (duplicate == default)
        {
            diagnostic = null;
            return false;
        }

        var location = GetKeyLocation(jsonVariableOptions);
        diagnostic = VariableOptionsDiagnostics.DuplicateKey(jsonVariableOptions.Key, location);

        return true;
    }

    private bool TryGetDuplicatedClassNameDiagnostic(JsonVariableOptions jsonVariableOptions, out Diagnostic? diagnostic)
    {
        var optionsValue = jsonVariableOptions.Value;

        if (string.IsNullOrEmpty(optionsValue.ClassName))
        {
            diagnostic = null;
            return false;
        }

        var existing = _builder
            .Where(x => !string.IsNullOrEmpty(x.ClassName) && x.ClassName!.Equals(optionsValue.ClassName))
            .FirstOrDefault();

        if (existing == default)
        {
            diagnostic = null;
            return false;
        }

        var location = GetValueLocation(jsonVariableOptions);

        diagnostic = VariableOptionsDiagnostics.DuplicateClassName(
            jsonVariableOptions.Key,
            optionsValue.ClassName!,
            existing.VariableKey,
            location);

        return true;
    }

    private bool TryGetDuplicatedNameDiagnostic(JsonVariableOptions jsonVariableOptions, out Diagnostic? diagnostic)
    {
        var optionsValue = jsonVariableOptions.Value;

        if (string.IsNullOrEmpty(optionsValue.Name))
        {
            diagnostic = null;
            return false;
        }

        var existing = _builder
            .Where(x => !string.IsNullOrEmpty(x.Name) && x.Name!.Equals(optionsValue.Name))
            .FirstOrDefault();

        if (existing == default)
        {
            diagnostic = null;
            return false;
        }

        var location = GetValueLocation(jsonVariableOptions);

        diagnostic = VariableOptionsDiagnostics.DuplicateVariableName(
            jsonVariableOptions.Key,
            optionsValue.Name!,
            existing.VariableKey,
            location);

        return true;
    }

    private Location GetKeyLocation(JsonVariableOptions jsonVariableOptions)
    {
        return _locationFactory.GetLocation(jsonVariableOptions.KeySpan);
    }

    private Location GetValueLocation(JsonVariableOptions jsonVariableOptions)
    {
        return _locationFactory.GetLocation(jsonVariableOptions.ValueSpan);
    }
}
