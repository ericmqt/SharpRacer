namespace SharpRacer.SessionInfo.Yaml;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public sealed class SplitTimeInfoNode
{
    public List<SplitTimeSectorNode> Sectors { get; set; } = new List<SplitTimeSectorNode>();
}
