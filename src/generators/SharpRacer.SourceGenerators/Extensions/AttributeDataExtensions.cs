using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators;

internal static class AttributeDataExtensions
{
    public static Location GetLocation(this AttributeData attributeData)
    {
        if (attributeData is null)
        {
            return Location.None;
        }

        if (attributeData.ApplicationSyntaxReference is null)
        {
            return Location.None;
        }

        return Location.Create(
            attributeData.ApplicationSyntaxReference.SyntaxTree,
            attributeData.ApplicationSyntaxReference.Span);
    }
}
