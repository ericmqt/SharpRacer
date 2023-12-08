﻿using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators;

internal static class AttributeDataExtensions
{
    public static Location? GetLocation(this AttributeData attributeData)
    {
        if (attributeData?.ApplicationSyntaxReference is null)
        {
            return null;
        }

        return Location.Create(
            attributeData.ApplicationSyntaxReference.SyntaxTree,
            attributeData.ApplicationSyntaxReference.Span);
    }
}