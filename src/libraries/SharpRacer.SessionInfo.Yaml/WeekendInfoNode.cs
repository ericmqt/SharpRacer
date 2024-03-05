namespace SharpRacer.SessionInfo.Yaml;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public sealed class WeekendInfoNode
{
    public string BuildTarget { get; set; } = string.Empty;
    public string BuildType { get; set; } = string.Empty;
    public ContentVersion BuildVersion { get; set; } = default;
    public string Category { get; set; } = string.Empty;
    public string DCRuleSet { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public int HeatRacing { get; set; }
    public int LeagueID { get; set; }
    public int MaxDrivers { get; set; }
    public int MinDrivers { get; set; }
    public int NumCarClasses { get; set; }
    public int NumCarTypes { get; set; }
    public int Official { get; set; }
    public int QualifierMustStartRace { get; set; }
    public int RaceWeek { get; set; }
    public int SeasonID { get; set; }
    public int SeriesID { get; set; }
    public int SessionID { get; set; }

    /// <summary>
    /// Gets or sets the SimMode value.
    /// </summary>
    /// <remarks>
    /// Value is "full" or "replay".
    /// </remarks>
    public string SimMode { get; set; } = string.Empty;
    public int SubSessionID { get; set; }
    public int TeamRacing { get; set; }
    public TelemetryOptionsNode TelemetryOptions { get; set; } = new TelemetryOptionsNode();
    public string TrackAirPressure { get; set; } = string.Empty;
    public string TrackAirTemp { get; set; } = string.Empty;
    public string TrackAltitude { get; set; } = string.Empty;
    public string TrackCity { get; set; } = string.Empty;
    public int TrackCleanup { get; set; }
    public string TrackConfigName { get; set; } = string.Empty;
    public string TrackCountry { get; set; } = string.Empty;
    public string TrackDirection { get; set; } = string.Empty;
    public string TrackDisplayName { get; set; } = string.Empty;
    public string TrackDisplayShortName { get; set; } = string.Empty;
    public int TrackDynamicTrack { get; set; }
    public string TrackFogLevel { get; set; } = string.Empty;
    public int TrackID { get; set; }
    public string TrackLatitude { get; set; } = string.Empty;
    public string TrackLength { get; set; } = string.Empty;
    public string TrackLengthOfficial { get; set; } = string.Empty;
    public string TrackLongitude { get; set; } = string.Empty;
    public string TrackName { get; set; } = string.Empty;
    public string TrackNorthOffset { get; set; } = string.Empty;
    public int TrackNumTurns { get; set; }
    public string TrackPitSpeedLimit { get; set; } = string.Empty;
    public string TrackRelativeHumidity { get; set; } = string.Empty;
    public string TrackSkies { get; set; } = string.Empty;
    public string TrackSurfaceTemp { get; set; } = string.Empty;
    public string TrackType { get; set; } = string.Empty;
    public string TrackVersion { get; set; } = string.Empty;
    public string TrackWeatherType { get; set; } = string.Empty;
    public string TrackWindDir { get; set; } = string.Empty;
    public string TrackWindVel { get; set; } = string.Empty;
    public WeekendOptionsNode WeekendOptions { get; set; } = new WeekendOptionsNode();
}
