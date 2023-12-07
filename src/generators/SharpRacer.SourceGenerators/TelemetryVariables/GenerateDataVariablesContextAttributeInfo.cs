﻿using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables;
internal static class GenerateDataVariablesContextAttributeInfo
{
    internal static string ConfigurationFileNamePropertyName = "ConfigurationFileName";
    internal static string FullTypeName = "SharpRacer.Telemetry.Variables.GenerateDataVariablesContextAttribute";
    internal static string VariablesFileNamePropertyName = "IncludedVariableNamesFile";

    internal static IncludedVariablesFileName GetIncludedVariablesFileNameOrDefault(AttributeData attributeData)
    {
        if (attributeData is null)
        {
            throw new ArgumentNullException(nameof(attributeData));
        }

        var fileNameArg = FindIncludedVariableNamesFileArgumentValue(attributeData);

        if (fileNameArg is null)
        {
            return default;
        }

        return new IncludedVariablesFileName(fileNameArg);
    }

    internal static string? FindIncludedVariableNamesFileArgumentValue(AttributeData attributeData)
    {
        if (attributeData is null)
        {
            throw new ArgumentNullException(nameof(attributeData));
        }

        var fileNameArg = attributeData.ConstructorArguments.FirstOrDefault();

        if (fileNameArg.Value is string fileNameArgValue)
        {
            // This will resolve paths like 'Assets\TelemetryVariables.json' relative to the project directory so that the path can be
            // easily compared with AdditionalTexts
            var fileName = Path.GetFileName(fileNameArgValue);

            return fileName;
        }

        return null;
    }
}
