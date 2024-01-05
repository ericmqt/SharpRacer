using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
internal class DescriptorPropertyModelFactory
{
    private readonly ImmutableArray<DescriptorPropertyModel>.Builder _builder;

    public DescriptorPropertyModelFactory()
    {
        _builder = ImmutableArray.CreateBuilder<DescriptorPropertyModel>();
    }

    public ImmutableArray<DescriptorPropertyModel> Build()
    {
        return _builder.ToImmutable();
    }

    public bool TryAdd(VariableModel variableModel, out Diagnostic? diagnostic)
    {
        if (variableModel == default)
        {
            diagnostic = null;
            return false;
        }

        var propertyName = variableModel.DescriptorPropertyName();

        if (TryGetPropertyNameConflictsWithExistingVariableDiagnostic(variableModel, propertyName, out var propertyNameDiagnostic))
        {
            diagnostic = propertyNameDiagnostic;

            return false;
        }

        diagnostic = null;
        var model = new DescriptorPropertyModel(propertyName, variableModel.Description, variableModel);
        _builder.Add(model);

        return true;
    }

    private bool TryGetPropertyNameConflictsWithExistingVariableDiagnostic(VariableModel variableModel, string propertyName, out Diagnostic? diagnostic)
    {
        var conflictingModel = _builder.FirstOrDefault(x => x.PropertyName.Equals(propertyName));

        if (conflictingModel == default)
        {
            diagnostic = null;
            return false;
        }

        diagnostic = GeneratorDiagnostics.DescriptorNameConflictsWithExistingVariable(
            variableModel.VariableName,
            propertyName,
            conflictingModel.VariableName,
            variableModel.Options.ValueLocation);

        return true;
    }
}
