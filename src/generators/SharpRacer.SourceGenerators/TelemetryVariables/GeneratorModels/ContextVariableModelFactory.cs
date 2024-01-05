using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
internal class ContextVariableModelFactory
{
    private readonly ImmutableArray<ContextVariableModel>.Builder _builder;
    private readonly ImmutableArray<DescriptorPropertyReference> _descriptorPropertyReferences;
    private readonly ImmutableArray<VariableClassReference> _variableClassReferences;

    public ContextVariableModelFactory(
        ImmutableArray<VariableClassReference> variableClassReferences,
        ImmutableArray<DescriptorPropertyReference> descriptorPropertyReferences)
    {
        _variableClassReferences = variableClassReferences.GetEmptyIfDefault();
        _descriptorPropertyReferences = descriptorPropertyReferences.GetEmptyIfDefault();

        _builder = ImmutableArray.CreateBuilder<ContextVariableModel>();
    }

    public ImmutableArray<ContextVariableModel> Build()
    {
        return _builder.ToImmutable();
    }

    public bool TryAdd(VariableModel variableModel, out ImmutableArray<Diagnostic> diagnostics)
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

        DescriptorPropertyReference? descriptorReference = null;
        VariableClassReference? variableClassReference = null;

        if (_variableClassReferences.Any(x => x.VariableName.Equals(variableModel.VariableName)))
        {
            variableClassReference = _variableClassReferences.First(x => x.VariableName.Equals(variableModel.VariableName));
        }

        if (_descriptorPropertyReferences.Any(x => x.VariableName.Equals(variableModel.VariableName)))
        {
            descriptorReference = _descriptorPropertyReferences.First(x => x.VariableName.Equals(variableModel.VariableName));
        }

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
