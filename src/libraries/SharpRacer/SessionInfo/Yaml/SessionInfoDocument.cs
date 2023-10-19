using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SharpRacer.SessionInfo.Yaml;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public sealed class SessionInfoDocument
{
    public CameraInfoNode CameraInfo { get; set; } = new CameraInfoNode();
    public DriverInfoNode DriverInfo { get; set; } = new DriverInfoNode();

    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public QualifyResultsInfoNode? QualifyResultsInfo { get; set; }

    public RadioInfoNode RadioInfo { get; set; } = new RadioInfoNode();
    public SessionInfoNode SessionInfo { get; set; } = new SessionInfoNode();
    public SplitTimeInfoNode SplitTimeInfo { get; set; } = new SplitTimeInfoNode();
    public WeekendInfoNode WeekendInfo { get; set; } = new WeekendInfoNode();

    public static SessionInfoDocument FromYaml(string yaml)
    {
        if (string.IsNullOrEmpty(yaml))
        {
            throw new ArgumentException($"'{nameof(yaml)}' cannot be null or empty.", nameof(yaml));
        }

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        return deserializer.Deserialize<SessionInfoDocument>(yaml);
    }
}
