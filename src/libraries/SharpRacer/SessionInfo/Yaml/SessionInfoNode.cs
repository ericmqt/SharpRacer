namespace SharpRacer.SessionInfo.Yaml;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public sealed class SessionInfoNode
{
    public List<SessionNode> Sessions { get; set; } = new List<SessionNode>();
}
