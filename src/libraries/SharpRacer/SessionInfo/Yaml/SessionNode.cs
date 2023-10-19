namespace SharpRacer.SessionInfo.Yaml;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public sealed class SessionNode
{
    public double ResultsAverageLapTime { get; set; }
    public List<SessionResultsFastestLapNode> ResultsFastestLap { get; set; } = new List<SessionResultsFastestLapNode>();
    public int ResultsLapsComplete { get; set; }
    public int ResultsNumCautionFlags { get; set; }
    public int ResultsNumCautionLaps { get; set; }
    public int ResultsNumLeadChanges { get; set; }
    public int ResultsOfficial { get; set; }
    public List<SessionResultNode> ResultsPositions { get; set; } = new List<SessionResultNode>();
    public int SessionEnforceTireCompoundChange { get; set; }
    public int SessionNum { get; set; }
    public int SessionNumLapsToAvg { get; set; }
    public string SessionLaps { get; set; } = default!;
    public string SessionName { get; set; } = default!;
    public int SessionRunGroupsUsed { get; set; }
    public int SessionSkipped { get; set; }
    public string? SessionSubType { get; set; }
    public string SessionTime { get; set; } = default!;
    public string SessionTrackRubberState { get; set; } = default!;
    public string SessionType { get; set; } = default!;
}
