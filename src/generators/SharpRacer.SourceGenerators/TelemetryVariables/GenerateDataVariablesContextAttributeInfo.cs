using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables;
internal static class GenerateDataVariablesContextAttributeInfo
{
    internal static IncludedVariablesFileName GetIncludedVariablesFileNameOrDefault(AttributeData attributeData)
    {
        var fileNameArg = attributeData.ConstructorArguments.FirstOrDefault();

        if (fileNameArg.Value is not string fileNameArgValue)
        {
            return default;
        }

        if (string.IsNullOrWhiteSpace(fileNameArgValue))
        {
            return default;
        }

        return new IncludedVariablesFileName(fileNameArgValue!);
    }
}
