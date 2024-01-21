namespace SharpRacer.SessionInfo.Yaml;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public sealed class RadioFrequencyNode
{
    public int CanScan { get; set; }
    public int CanSquawk { get; set; }
    public int CarIdx { get; set; }
    public int ClubID { get; set; }
    public int EntryIdx { get; set; }
    public int FrequencyNum { get; set; }
    public string FrequencyName { get; set; } = default!;
    public int IsDeletable { get; set; }
    public int IsMutable { get; set; }
    public int Muted { get; set; }
    public int Priority { get; set; }
}
