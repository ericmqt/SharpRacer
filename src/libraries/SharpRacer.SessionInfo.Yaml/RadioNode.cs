namespace SharpRacer.SessionInfo.Yaml;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public sealed class RadioNode
{
    public List<RadioFrequencyNode> Frequencies { get; set; } = new List<RadioFrequencyNode>();
    public int HopCount { get; set; }
    public int NumFrequencies { get; set; }
    public int RadioNum { get; set; }
    public int ScanningIsOn { get; set; }
    public int TunedToFrequencyNum { get; set; }
}
