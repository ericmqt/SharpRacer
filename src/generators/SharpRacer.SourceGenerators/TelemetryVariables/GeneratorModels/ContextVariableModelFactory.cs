using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
internal class ContextVariableModelFactory
{
    private readonly ImmutableArray<ContextVariableModel>.Builder _builder;
    private readonly ContextClassInfo _contextClass;

    public ContextVariableModelFactory(ContextClassInfo contextClass)
    {
        _builder = ImmutableArray.CreateBuilder<ContextVariableModel>();
        _contextClass = contextClass;
    }

    public ImmutableArray<ContextVariableModel> Build()
    {
        return _builder.ToImmutable();
    }

    public bool TryAdd(
        VariableModel variableModel,
        DescriptorPropertyReference? descriptorReference,
        VariableClassReference? variableClassReference,
        out ImmutableArray<Diagnostic> diagnostics)
    {
        if (variableModel == default)
        {
            diagnostics = ImmutableArray<Diagnostic>.Empty;
            return false;
        }

        if (!TryGetPropertyName(variableModel, out var propertyName, out var propertyNameDiagnostic))
        {
            diagnostics = ImmutableArray.Create(propertyNameDiagnostic);
            return false;
        }

        diagnostics = ImmutableArray<Diagnostic>.Empty;

        var model = new ContextVariableModel(
            variableModel,
            propertyName,
            variableModel.Description,
            variableClassReference,
            descriptorReference);

        _builder.Add(model);

        return true;
    }

    private bool TryGetPropertyName(VariableModel variableModel, out string propertyName, out Diagnostic diagnostic)
    {
        ContextVariableModel existing = default;

        if (!string.IsNullOrWhiteSpace(variableModel.Options.Name))
        {
            var configuredPropertyName = variableModel.Options.Name!;

            existing = _builder.FirstOrDefault(x => x.PropertyName.Equals(configuredPropertyName, StringComparison.Ordinal));

            if (existing != default)
            {
                diagnostic = GeneratorDiagnostics.ContextClassConfiguredPropertyNameConflict(
                    _contextClass.ClassName,
                    variableModel.VariableName,
                    configuredPropertyName,
                    existing.VariableModel.VariableName,
                    existing.PropertyName);

                propertyName = string.Empty;

                return false;
            }

            propertyName = configuredPropertyName;
            diagnostic = null!;
            return true;
        }

        existing = _builder.FirstOrDefault(x => x.PropertyName.Equals(variableModel.VariableName, StringComparison.Ordinal));

        if (existing != default)
        {
            diagnostic = GeneratorDiagnostics.ContextClassVariableNameCreatesPropertyNameConflict(
                _contextClass.ClassName,
                variableModel.VariableName,
                existing.VariableModel.VariableName,
                existing.PropertyName);

            propertyName = string.Empty;

            return false;
        }

        propertyName = variableModel.VariableName;
        diagnostic = null!;

        return true;
    }
}
