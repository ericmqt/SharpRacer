using SharpRacer.SourceGenerators.Configuration;

namespace SharpRacer.SourceGenerators.Testing.Configuration;
internal class MSBuildProperty
{
    public MSBuildProperty(MSBuildPropertyKey propertyKey)
        : this(propertyKey, null)
    {

    }

    public MSBuildProperty(MSBuildPropertyKey propertyKey, string? value)
    {
        PropertyKey = propertyKey;
        PropertyValue = new MSBuildPropertyValue(propertyKey, value);
    }

    public MSBuildProperty(MSBuildPropertyKey propertyKey, MSBuildPropertyValue propertyValue)
    {
        PropertyKey = propertyKey;
        PropertyValue = propertyValue;
    }

    public MSBuildPropertyKey PropertyKey { get; }
    public MSBuildPropertyValue PropertyValue { get; }
}
