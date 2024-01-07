using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;

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
        var className = variableModel.VariableClassName();
        var classNamespace = _variableClassOptions.TargetNamespace;

        // Get any diagnostics
        var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();

        if (TryGetDuplicateClassNameDiagnostic(variableModel, className, classNamespace, out var duplicateClassNameDiagnostic))
        {
            diagnosticsBuilder.Add(duplicateClassNameDiagnostic!);
        }

        // Build and add model
        var model = new VariableClassGeneratorModel(
            className,
            classNamespace,
            variableModel,
            diagnosticsBuilder.ToImmutable(),
            _variableClassOptions.GetDescriptorPropertyReference(ref variableModel),
            isClassInternal: false,
            isClassPartial: true);

        _builder.Add(model);
    }

    private bool TryGetDuplicateClassNameDiagnostic(VariableModel variableModel, string className, string classNamespace, out Diagnostic? diagnostic)
    {
        var existing = _builder.FirstOrDefault(x =>
            x.ClassName.Equals(className, StringComparison.Ordinal) &&
            x.ClassNamespace.Equals(classNamespace, StringComparison.Ordinal));

        if (existing == default)
        {
            diagnostic = null;
            return false;
        }

        var modelTypeName = $"{classNamespace}.{className}";
        var existingTypeName = $"{existing.ClassNamespace}.{existing.ClassName}";

        diagnostic = GeneratorDiagnostics.VariableClassConfiguredClassNameInUse(modelTypeName, variableModel.VariableName, existingTypeName, existing.VariableName);
        return true;
    }
}
