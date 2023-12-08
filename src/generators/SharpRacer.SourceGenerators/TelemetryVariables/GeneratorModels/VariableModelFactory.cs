using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
internal static class VariableModelFactory
{
    public static ImmutableArray<VariableModel> CreateArray(
        ImmutableArray<VariableInfo> variables,
        ImmutableArray<VariableOptions> variableOptions,
        ImmutableArray<Diagnostic>.Builder diagnosticsBuilder,
        CancellationToken cancellationToken = default)
    {
        var builder = ImmutableArray.CreateBuilder<VariableModel>(variables.Length);

        foreach (var variable in variables)
        {
            cancellationToken.ThrowIfCancellationRequested();
            VariableOptions optionsModel = default;

            if (variableOptions.Any(x => x.VariableKey.Equals(variable.Name, StringComparison.Ordinal)))
            {
                optionsModel = variableOptions.First(x => x.VariableKey.Equals(variable.Name, StringComparison.Ordinal));
            }

            if (TryCreateModel(variable, optionsModel, builder, out var model, out var modelDiagnostics))
            {
                builder.Add(model);
            }
            else
            {
                diagnosticsBuilder.AddRange(modelDiagnostics);
            }
        }

        return builder.ToImmutable();
    }

    private static bool TryCreateModel(
        VariableInfo variableInfo,
        VariableOptions options,
        ImmutableArray<VariableModel>.Builder collectionBuilder,
        out VariableModel model,
        out ImmutableArray<Diagnostic> diagnostics)
    {
        var configuredVariableName = options.GetConfiguredName(variableInfo);
        var configuredContextPropertyName = options.GetConfiguredContextPropertyName(variableInfo);
        var configuredDescriptorName = options.GetConfiguredDescriptorName(variableInfo);

        diagnostics = GetModelDiagnostics(
            variableInfo,
            configuredVariableName,
            configuredContextPropertyName,
            configuredDescriptorName,
            collectionBuilder);

        if (diagnostics.HasErrors())
        {
            model = default;
            return false;
        }

        model = new VariableModel(
            variableInfo,
            configuredVariableName,
            configuredContextPropertyName,
            configuredDescriptorName);

        return true;
    }

    private static ImmutableArray<Diagnostic> GetModelDiagnostics(
        VariableInfo variableInfo,
        string configuredVariableName,
        string configuredContextPropertyName,
        string configuredDescriptorName,
        ImmutableArray<VariableModel>.Builder collectionBuilder)
    {
        var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>(initialCapacity: 3);

        if (TryGetDuplicatedVariableNameDiagnostic(
            variableInfo,
            configuredVariableName,
            collectionBuilder,
            out var duplicateVariableNameDiagnostic))
        {
            diagnosticsBuilder.Add(duplicateVariableNameDiagnostic!);
        }

        if (TryGetDuplicatedContextPropertyNameDiagnostic(
            variableInfo,
            configuredContextPropertyName,
            collectionBuilder,
            out var duplicateContextPropertyNameDiagnostic))
        {
            diagnosticsBuilder.Add(duplicateContextPropertyNameDiagnostic!);
        }

        if (TryGetDuplicatedDescriptorNameDiagnostic(
            variableInfo,
            configuredDescriptorName,
            collectionBuilder,
            out var duplicateDescriptorNameDiagnostic))
        {
            diagnosticsBuilder.Add(duplicateDescriptorNameDiagnostic!);
        }

        return diagnosticsBuilder.ToImmutable();
    }

    public static bool TryGetDuplicatedContextPropertyNameDiagnostic(
        VariableInfo variableInfo,
        string contextPropertyName,
        IEnumerable<VariableModel> modelCollection,
        out Diagnostic? diagnostic)
    {
        var duplicate = modelCollection.FirstOrDefault(x => x.ContextPropertyName.Equals(contextPropertyName, StringComparison.Ordinal));

        if (duplicate == default)
        {
            diagnostic = null;
            return false;
        }

        diagnostic = GeneratorDiagnostics.VariableContextPropertyNameInUseByVariable(variableInfo, contextPropertyName, duplicate);
        return true;
    }

    public static bool TryGetDuplicatedDescriptorNameDiagnostic(
        VariableInfo variableInfo,
        string descriptorName,
        IEnumerable<VariableModel> modelCollection,
        out Diagnostic? diagnostic)
    {
        var duplicate = modelCollection.FirstOrDefault(x => x.DescriptorName.Equals(descriptorName, StringComparison.Ordinal));

        if (duplicate == default)
        {
            diagnostic = null;
            return false;
        }

        diagnostic = GeneratorDiagnostics.VariableDescriptorNameInUseByVariable(variableInfo, descriptorName, duplicate);
        return true;
    }

    public static bool TryGetDuplicatedVariableNameDiagnostic(
        VariableInfo variableInfo,
        string variableName,
        IEnumerable<VariableModel> modelCollection,
        out Diagnostic? diagnostic)
    {
        var duplicate = modelCollection.FirstOrDefault(x => x.Name.Equals(variableName, StringComparison.Ordinal));

        if (duplicate == default)
        {
            diagnostic = null;
            return false;
        }

        diagnostic = GeneratorDiagnostics.VariableNameInUseByVariable(variableInfo, variableName, duplicate);
        return true;
    }
}
