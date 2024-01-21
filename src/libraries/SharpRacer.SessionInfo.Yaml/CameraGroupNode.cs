using YamlDotNet.Serialization;

namespace SharpRacer.SessionInfo.Yaml;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public sealed class CameraGroupNode
{
    public List<CameraNode> Cameras { get; set; } = new List<CameraNode>();
    public string GroupName { get; set; } = default!;
    public int GroupNum { get; set; }

    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull & DefaultValuesHandling.OmitDefaults)]
    public bool IsScenic { get; set; } = false;
}
