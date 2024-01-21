namespace SharpRacer.SessionInfo.Yaml;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public sealed class RadioInfoNode
{
    public List<RadioNode> Radios { get; set; } = new List<RadioNode>();
    public int SelectedRadioNum { get; set; }
}
