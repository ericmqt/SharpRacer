namespace SharpRacer.SessionInfo.Yaml;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public sealed class SessionResultsFastestLapNode
{
    public int CarIdx { get; set; }
    public int FastestLap { get; set; }
    public double FastestTime { get; set; }
}
