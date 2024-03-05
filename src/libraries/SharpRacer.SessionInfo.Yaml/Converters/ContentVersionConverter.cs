using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace SharpRacer.SessionInfo.Yaml.Converters;
internal class ContentVersionConverter : IYamlTypeConverter
{
    public static readonly IYamlTypeConverter Instance = new ContentVersionConverter();

    public bool Accepts(Type type)
    {
        return type == typeof(ContentVersion);
    }

    public object? ReadYaml(IParser parser, Type type)
    {
        var valueStr = parser.Consume<Scalar>().Value;

        return ContentVersion.Parse(valueStr, null);
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type)
    {
        throw new NotImplementedException();
    }
}
