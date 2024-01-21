namespace SharpRacer.SessionInfo.Yaml;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public sealed class CameraInfoNode
{
    public List<CameraGroupNode> Groups { get; set; } = new List<CameraGroupNode>();
}
