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

    public bool TryAdd(JsonVariableInfo jsonVariableInfo, out ImmutableArray<Diagnostic> diagnostics)
    {
        if (jsonVariableInfo == default)
        {
            diagnostics = ImmutableArray<Diagnostic>.Empty;
            return false;
        }

        diagnostics = GetDiagnostics(jsonVariableInfo);

        if (diagnostics.HasErrors())
        {
            return false;
        }

        var variableInfo = new VariableInfo(jsonVariableInfo, GetLocation(jsonVariableInfo)!);

        _builder.Add(variableInfo);

        return true;
    }

    private ImmutableArray<Diagnostic> GetDiagnostics(JsonVariableInfo jsonVariableInfo)
    {
        var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();

        if (TryGetDuplicateNameDiagnostic(jsonVariableInfo, out var duplicateNameDiagnostic))
        {
            diagnosticsBuilder.Add(duplicateNameDiagnostic!);
        }

        return diagnosticsBuilder.ToImmutable();
    }

    public ImmutableArray<VariableInfo> Build()
    {
        return _builder.ToImmutable();
    }

    private bool TryGetDuplicateNameDiagnostic(JsonVariableInfo jsonVariableInfo, out Diagnostic? diagnostic)
    {
        var duplicate = _builder.FirstOrDefault(x => x.Name.Equals(jsonVariableInfo.Name, StringComparison.Ordinal));

        if (duplicate == default)
        {
            diagnostic = null;
            return false;
        }

        diagnostic = VariableInfoDiagnostics.VariableAlreadyDefined(
            jsonVariableInfo.Name,
            GetLocation(jsonVariableInfo));

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

    private Location? GetLocation(JsonVariableInfo jsonVariableInfo)
    {
        return _locationFactory.GetLocation(jsonVariableInfo.JsonSpan);
    }
}
