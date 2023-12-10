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

    public void Add(IncludedVariableNameValue value)
    {
        if (value == default)
        {
            return;
        }

        var include = new IncludedVariableName(
            value.Value,
            _file.SourceLocationFactory.GetLocation(value.ValueSpan),
            GetDiagnostics(value));

        _builder.Add(include);
    }

    public ImmutableArray<IncludedVariableName> Build()
    {
        return _builder.ToImmutable();
    }

    private ImmutableArray<Diagnostic> GetDiagnostics(IncludedVariableNameValue value)
    {
        if (value == default)
        {
            return ImmutableArray<Diagnostic>.Empty;
        }

        var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();

        if (string.IsNullOrEmpty(value.Value))
        {
            var emptyDiagnostic = IncludedVariablesDiagnostics.EmptyVariableName(_file.SourceLocationFactory.GetLocation(value.ValueSpan));
            diagnosticsBuilder.Add(emptyDiagnostic);
        }

        if (!_builder.Any(x => string.Equals(x.Value, value.Value, StringComparison.Ordinal)))
        {
            var reincludeDiagnostic = IncludedVariablesDiagnostics.VariableAlreadyIncluded(
                value.Value,
                _file.SourceLocationFactory.GetLocation(value.ValueSpan));

            diagnosticsBuilder.Add(reincludeDiagnostic);
        }

        return diagnosticsBuilder.ToImmutable();
    }
}
