using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

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

        diagnostics = GetDiagnostics(variableModel);

        if (diagnostics.HasErrors())
        {
            return false;
        }

        var propertyName = GetPropertyName(variableModel);

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

    private string GetPropertyName(VariableModel variableModel)
    {
        if (!string.IsNullOrWhiteSpace(variableModel.Options.Name))
        {
            return variableModel.Options.Name!;
        }

        return variableModel.VariableName;
    }

    private ImmutableArray<Diagnostic> GetDiagnostics(VariableModel variableModel)
    {
        var builder = ImmutableArray.CreateBuilder<Diagnostic>();

        // TODO: Diagnostics

        return builder.ToImmutable();
    }
}
