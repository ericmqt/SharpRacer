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

        if (!TryGetDescriptorPropertyName(variableModel, out var propertyName, out var propertyNameDiagnostic))
        {
            diagnostic = propertyNameDiagnostic;

            return false;
        }

        diagnostic = null;
        var model = new DescriptorPropertyModel(propertyName, variableModel.Description, variableModel.VariableInfo);
        _builder.Add(model);

        return true;
    }

    private bool TryGetDescriptorPropertyName(VariableModel variableModel, out string propertyName, out Diagnostic? diagnostic)
    {
        if (variableModel.Options != default && !string.IsNullOrWhiteSpace(variableModel.Options.Name))
        {
            propertyName = variableModel.Options.Name!;

            if (IsDescriptorNameInUse(propertyName, out var configuredNameConflictingModel))
            {
                diagnostic = DescriptorClassDiagnostics.DescriptorNameConflictsWithExistingVariable(
                    variableModel.VariableName,
                    propertyName,
                    configuredNameConflictingModel.VariableInfo.Name,
                    variableModel.Options.ValueLocation);

                return false;
            }

            diagnostic = null;
            return true;
        }

        propertyName = variableModel.VariableName;

        if (IsDescriptorNameInUse(propertyName, out var conflictingModel))
        {
            diagnostic = DescriptorClassDiagnostics.DescriptorNameConflictsWithExistingVariable(
                variableModel.VariableName,
                propertyName,
                conflictingModel.VariableInfo.Name,
                variableModel.Options.ValueLocation);

            return false;
        }

        diagnostic = null;
        return true;
    }

    private bool IsDescriptorNameInUse(string descriptorPropertyName, out DescriptorPropertyModel conflictingModel)
    {
        return TryFindModel(x => x.PropertyName.Equals(descriptorPropertyName), out conflictingModel);
    }

    private bool TryFindModel(Func<DescriptorPropertyModel, bool> predicate, out DescriptorPropertyModel model)
    {
        model = _builder.FirstOrDefault(predicate);

        return model != default;
    }
}
