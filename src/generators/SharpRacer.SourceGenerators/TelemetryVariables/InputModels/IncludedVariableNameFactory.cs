using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
internal class IncludedVariableNameFactory
{
    private readonly IncludedVariablesFile _file;
    private readonly ImmutableArray<IncludedVariableName>.Builder _builder;

    public IncludedVariableNameFactory(IncludedVariablesFile file)
    {
        _file = file;

        _builder = ImmutableArray.CreateBuilder<IncludedVariableName>();
    }

    public bool TryAdd(IncludedVariableNameValue value, out ImmutableArray<Diagnostic> diagnostics)
    {
        if (value == default)
        {
            diagnostics = ImmutableArray<Diagnostic>.Empty;
            return false;
        }

        diagnostics = GetDiagnostics(value);

        if (diagnostics.Any())
        {
            return false;
        }

        var include = new IncludedVariableName(
            value.Value,
            _file.SourceLocationFactory.GetLocation(value.ValueSpan));

        _builder.Add(include);

        return true;
    }

    public ImmutableArray<IncludedVariableName> Build()
    {
        return _builder.ToImmutable();
    }

    private ImmutableArray<Diagnostic> GetDiagnostics(IncludedVariableNameValue value)
    {
        var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();

        if (string.IsNullOrEmpty(value.Value))
        {
            var emptyDiagnostic = GeneratorDiagnostics.IncludedVariablesFileContainsEmptyVariableName(
                _file.SourceLocationFactory.GetLocation(value.ValueSpan));

            diagnosticsBuilder.Add(emptyDiagnostic);
        }

        if (_builder.Any(x => string.Equals(x.Value, value.Value, StringComparison.Ordinal)))
        {
            var reincludeDiagnostic = GeneratorDiagnostics.IncludedVariablesFileAlreadyIncludesVariable(
                value.Value,
                _file.FileName,
                _file.SourceLocationFactory.GetLocation(value.ValueSpan));

            diagnosticsBuilder.Add(reincludeDiagnostic);
        }

        return diagnosticsBuilder.ToImmutable();
    }
}
