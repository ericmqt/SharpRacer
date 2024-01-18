using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;

using VariableModelAndDescriptorRef = (
    SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels.VariableModel VariableModel,
    SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels.DescriptorPropertyReference? DescriptorReference
    );

namespace SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
internal static class VariableClassModelValuesProvider
{
    public static IncrementalValuesProvider<VariableClassModel> GetValuesProvider(
        IncrementalValuesProvider<VariableModel> variableModels,
        IncrementalValueProvider<ImmutableArray<DescriptorPropertyReference>> descriptorPropertyReferences,
        IncrementalValueProvider<VariableClassGeneratorOptions> classGeneratorOptions)
    {
        // Don't emit any VariableModels for further steps if variable class generation is toggled off
        var filteredVariableModels = variableModels
            .Combine(classGeneratorOptions)
            .Select(static (item, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                return item.Right.IsGeneratorEnabled
                    ? item.Left
                    : default;
            })
            .Where(static x => x != default);

        // TODO: Add tracked names here and ensure below steps don't execute

        var variableModelsAndDescriptorRefs = filteredVariableModels
            .Combine(descriptorPropertyReferences)
            .Select<(VariableModel Model, ImmutableArray<DescriptorPropertyReference> DescriptorPropertyReferences), VariableModelAndDescriptorRef>(
                static (item, ct) =>
                {
                    DescriptorPropertyReference? propertyRef =
                        item.DescriptorPropertyReferences.FirstOrDefault(x => x.VariableName == item.Model.VariableName);

                    if (propertyRef.Value == default)
                    {
                        return (item.Model, null);
                    }

                    return (item.Model, propertyRef);
                });

        var classModels = variableModelsAndDescriptorRefs
            .Combine(classGeneratorOptions)
            .Select(static (item, ct) =>
            {
                var className = GetVariableClassName(ref item.Left.VariableModel, ref item.Right);
                var classNamespace = item.Right.TargetNamespace;

                return new VariableClassModel(
                    className,
                    classNamespace,
                    item.Left.VariableModel,
                    item.Left.DescriptorReference,
                    isClassInternal: false,
                    isClassPartial: true);
            });

        return classModels
            .Collect()
            .SelectMany(static (input, cancellationToken) =>
            {
                var builder = ImmutableArray.CreateBuilder<VariableClassModel>();

                for (int i = 0; i < input.Length; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var classModel = input[i];

                    if (TryGetDuplicateClassNameDiagnostic(ref classModel, builder, out var diagnostic))
                    {
                        builder.Add(classModel.WithDiagnostics(diagnostic!));
                    }
                    else
                    {
                        builder.Add(classModel);
                    }
                }

                return builder.ToImmutable();
            })
            .WithTrackingName(TrackingNames.VariableClassModelValuesProvider_GetValuesProvider);
    }

    private static string GetVariableClassName(
        ref readonly VariableModel variableModel,
        ref readonly VariableClassGeneratorOptions classGeneratorOptions)
    {
        if (variableModel.Options != default)
        {
            if (!string.IsNullOrWhiteSpace(variableModel.Options.ClassName))
            {
                return classGeneratorOptions.FormatClassName(variableModel.Options.ClassName!);
            }

            if (!string.IsNullOrWhiteSpace(variableModel.Options.Name))
            {
                return classGeneratorOptions.FormatClassName(variableModel.Options.Name!);
            }
        }

        return classGeneratorOptions.FormatClassName(variableModel.VariableName);
    }

    private static bool TryGetDuplicateClassNameDiagnostic(
        ref readonly VariableClassModel classModel,
        IList<VariableClassModel> existingModels,
        out Diagnostic? diagnostic)
    {
        var className = classModel.ClassName;
        var classNamespace = classModel.ClassNamespace;

        var existing = existingModels.FirstOrDefault(x =>
            x.ClassName.Equals(className, StringComparison.Ordinal) &&
            x.ClassNamespace.Equals(classNamespace, StringComparison.Ordinal));

        if (existing == default)
        {
            diagnostic = null;
            return false;
        }

        var modelTypeName = $"{classNamespace}.{className}";
        var existingTypeName = $"{existing.ClassNamespace}.{existing.ClassName}";

        diagnostic = GeneratorDiagnostics.VariableClassConfiguredClassNameInUse(
            modelTypeName,
            classModel.VariableName,
            existingTypeName,
            existing.VariableName);

        return true;
    }
}
