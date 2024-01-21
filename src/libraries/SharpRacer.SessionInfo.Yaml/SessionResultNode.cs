using YamlDotNet.Serialization;

namespace SharpRacer.SessionInfo.Yaml;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public sealed class SessionResultNode
{
    public int CarIdx { get; set; }
    public int ClassPosition { get; set; }
    public int Position { get; set; }
    public int Lap { get; set; }
    public double Time { get; set; }
    public int FastestLap { get; set; }
    public double FastestTime { get; set; }
    public double LastTime { get; set; }
    public int LapsLed { get; set; }
    public int LapsComplete { get; set; }
    public int JokerLapsComplete { get; set; }
    public double LapsDriven { get; set; }
    public int Incidents { get; set; }

    [YamlMember(Alias = "ReasonOutStr")]
    public string ReasonOutStr { get; set; } = string.Empty;
    public int ReasonOutId { get; set; }
}
