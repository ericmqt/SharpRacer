using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.Json;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
internal class VariableInfoFactory
{
    private readonly ImmutableArray<VariableInfo>.Builder _builder;
    private readonly JsonLocationFactory _locationFactory;

    public VariableInfoFactory(VariableInfoFile variableInfoFile, int initialCapacity)
    {
        _locationFactory = variableInfoFile.SourceLocationFactory;

        _builder = ImmutableArray.CreateBuilder<VariableInfo>(initialCapacity);
    }

    public bool TryAdd(VariableInfo variableInfo, out ImmutableArray<Diagnostic> diagnostics)
    {
        if (variableInfo == default)
        {
            diagnostics = ImmutableArray<Diagnostic>.Empty;
            return false;
        }

        diagnostics = GetDiagnostics(variableInfo);

        if (diagnostics.HasErrors())
        {
            return false;
        }

        variableInfo = variableInfo.WithJsonLocation(GetLocation(variableInfo)!);

        _builder.Add(variableInfo);

        return true;
    }

    private ImmutableArray<Diagnostic> GetDiagnostics(VariableInfo variableInfo)
    {
        var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();

        if (TryGetDuplicateNameDiagnostic(variableInfo, out var duplicateNameDiagnostic))
        {
            diagnosticsBuilder.Add(duplicateNameDiagnostic!);
        }

        return diagnosticsBuilder.ToImmutable();
    }

    public ImmutableArray<VariableInfo> Build()
    {
        return _builder.ToImmutable();
    }

    private bool TryGetDuplicateNameDiagnostic(VariableInfo variableInfo, out Diagnostic? diagnostic)
    {
        var duplicate = _builder.FirstOrDefault(x => x.Name.Equals(variableInfo.Name, StringComparison.Ordinal));

        if (duplicate == default)
        {
            diagnostic = null;
            return false;
        }

        diagnostic = VariableInfoDiagnostics.VariableAlreadyDefined(
            variableInfo.Name,
            GetLocation(variableInfo));

        return true;
    }

    /*private bool TryGetDeprecatingVariableNotFound(JsonVariableInfo jsonVariableInfo, out Diagnostic? diagnostic)
    {
        if (string.IsNullOrEmpty(jsonVariableInfo.DeprecatedBy))
        {
            diagnostic = null;
            return false;
        }

        if (!_source.Any(x => x.Name.Equals(jsonVariableInfo.DeprecatedBy, StringComparison.Ordinal)))
        {
            diagnostic = VariableInfoDiagnostics.VariableInfoDeprecatingVariableNotFoundWarning(
                jsonVariableInfo.Name,
                jsonVariableInfo.DeprecatedBy!,
                GetLocation(jsonVariableInfo));
        }

        diagnostic = null;
        return false;
    }*/

    private Location? GetLocation(VariableInfo variableInfo)
    {
        return _locationFactory.GetLocation(variableInfo.JsonSpan);
    }
}
