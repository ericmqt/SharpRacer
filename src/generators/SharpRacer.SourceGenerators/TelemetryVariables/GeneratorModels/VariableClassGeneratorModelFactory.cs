using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
internal class VariableClassGeneratorModelFactory
{
    private ImmutableArray<VariableClassGeneratorModel>.Builder _builder;
    private readonly VariableClassGeneratorOptions _variableClassOptions;

    public VariableClassGeneratorModelFactory(VariableClassGeneratorOptions variableClassOptions, int initialCapacity)
    {
        _builder = ImmutableArray.CreateBuilder<VariableClassGeneratorModel>(initialCapacity);
        _variableClassOptions = variableClassOptions;
    }

    public ImmutableArray<VariableClassGeneratorModel> Build()
    {
        return _builder.ToImmutable();
    }

    public void Add(VariableModel variableModel)
    {
        var className = GetClassName(variableModel);

        // Get any diagnostics
        var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();

        if (TryGetDuplicateClassNameDiagnostic(variableModel, className, out var duplicateClassNameDiagnostic))
        {
            diagnosticsBuilder.Add(duplicateClassNameDiagnostic!);
        }

        // Build and add model
        var model = new VariableClassGeneratorModel(
            className,
            _variableClassOptions.TargetNamespace,
            variableModel.VariableInfo,
            diagnosticsBuilder.ToImmutable(),
            _variableClassOptions.GetDescriptorPropertyReference(ref variableModel),
            isClassInternal: false,
            isClassPartial: true);

        _builder.Add(model);
    }

    private string GetClassName(VariableModel variableModel)
    {
        if (variableModel.Options != default && !string.IsNullOrEmpty(variableModel.Options.ClassName))
        {
            return _variableClassOptions.FormatClassName(variableModel.Options.ClassName!);
        }

        return _variableClassOptions.FormatClassName(variableModel.VariableInfo.Name);
    }

    private bool TryGetDuplicateClassNameDiagnostic(VariableModel variableModel, string className, out Diagnostic? diagnostic)
    {
        var existing = _builder.FirstOrDefault(x => x.ClassName.Equals(className, StringComparison.Ordinal));

        if (existing == default)
        {
            diagnostic = null;
            return false;
        }

        diagnostic = GeneratorDiagnostics.VariableClassNameInUse(className, variableModel.VariableInfo.Name, existing.VariableName);
        return true;
    }
}
