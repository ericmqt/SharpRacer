﻿using Microsoft.CodeAnalysis.Diagnostics;
using SharpRacer.SourceGenerators.Configuration;

namespace SharpRacer.SourceGenerators;
internal static class AnalyzerConfigOptionsExtensions
{
    public static MSBuildPropertyValue GetMSBuildProperty(this AnalyzerConfigOptions source, MSBuildPropertyKey propertyKey)
    {
        if (propertyKey == default)
        {
            throw new ArgumentException($"'{nameof(propertyKey)}' cannot be a default value.", nameof(propertyKey));
        }

        source.TryGetValue(propertyKey.Key, out var value);

        return new MSBuildPropertyValue(propertyKey, value);
    }

    public static bool TryGetMSBuildProperty(this AnalyzerConfigOptions source, MSBuildPropertyKey propertyKey, out MSBuildPropertyValue propertyValue)
    {
        if (propertyKey == default)
        {
            throw new ArgumentException($"'{nameof(propertyKey)}' cannot be a default value.", nameof(propertyKey));
        }

        if (source.TryGetValue(propertyKey.Key, out var value))
        {
            propertyValue = new MSBuildPropertyValue(propertyKey, value);
            return true;
        }

        propertyValue = new MSBuildPropertyValue(propertyKey, null);
        return false;
    }
}
