using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
internal class TypedVariableClassGeneratorModelFactory
{
    private ImmutableArray<TypedVariableClassGeneratorModel>.Builder _builder;

    public TypedVariableClassGeneratorModelFactory()
    {
        _builder = ImmutableArray.CreateBuilder<TypedVariableClassGeneratorModel>();
    }

    public ImmutableArray<TypedVariableClassGeneratorModel> Build()
    {
        return _builder.ToImmutable();
    }

    public bool TryAdd(
        VariableModel variableModel,
        TypedVariableClassesGeneratorOptions variableClassOptions,
        out ImmutableArray<Diagnostic> diagnostics)
    {
        if (variableModel == default)
        {
            return false;
        }

        var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();

        var className = GetClassName(variableModel, variableClassOptions);

        if (TryGetDuplicateClassNameDiagnostic(variableModel, className, out var duplicateClassNameDiagnostic))
        {
            diagnosticsBuilder.Add(duplicateClassNameDiagnostic!);
        }

        diagnostics = diagnosticsBuilder.ToImmutable();

        if (diagnostics.HasErrors())
        {
            return false;
        }

        var model = new TypedVariableClassGeneratorModel(
            className,
            variableClassOptions.TargetNamespace,
            variableModel.VariableInfo,
            variableClassOptions.GetDescriptorPropertyReference(ref variableModel),
            isClassInternal: false,
            isClassPartial: true);

        _builder.Add(model);

        return true;
    }

    private string GetClassName(VariableModel variableModel, TypedVariableClassesGeneratorOptions variableClassOptions)
    {
        if (variableModel.Options != default && !string.IsNullOrEmpty(variableModel.Options.ClassName))
        {
            return variableClassOptions.FormatClassName(variableModel.Options.ClassName!);
        }

        return variableClassOptions.FormatClassName(variableModel.VariableInfo.Name);
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
