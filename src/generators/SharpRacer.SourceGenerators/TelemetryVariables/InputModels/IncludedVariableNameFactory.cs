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

    public static ImmutableArray<IncludedVariableName> Create(IncludedVariablesFile file, CancellationToken cancellationToken, out ImmutableArray<Diagnostic> diagnostics)
    {
        diagnostics = ImmutableArray<Diagnostic>.Empty;

        if (file == default)
        {
            return ImmutableArray<IncludedVariableName>.Empty;
        }

        var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();

        var includedVariableNameValues = file.ReadJson(cancellationToken, out var readJsonDiagnostic);

        if (readJsonDiagnostic != null)
        {
            diagnostics = ImmutableArray.Create(readJsonDiagnostic);

            return ImmutableArray<IncludedVariableName>.Empty;
        }

        var factory = new IncludedVariableNameFactory(file);

        foreach (var value in includedVariableNameValues)
        {
            factory.TryAdd(value, out var valueDiagnostics);

            diagnosticsBuilder.AddRange(valueDiagnostics);
        }

        diagnostics = diagnosticsBuilder.ToImmutable();

        return factory.Build();
    }

    public ImmutableArray<IncludedVariableName> Build()
    {
        return _builder.ToImmutable();
    }

    public bool TryAdd(IncludedVariableNameValue value, out ImmutableArray<Diagnostic> diagnostics)
    {
        if (value == default)
        {
            diagnostics = ImmutableArray<Diagnostic>.Empty;
            return false;
        }

        diagnostics = GetDiagnostics(value);

        if (diagnostics.HasErrors())
        {
            return false;
        }

        var include = new IncludedVariableName(value.Value, _file.SourceLocationFactory.GetLocation(value.ValueSpan));

        _builder.Add(include);

        return true;
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
