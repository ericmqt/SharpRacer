namespace SharpRacer.SessionInfo.Yaml;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public sealed class WeekendOptionsNode
{
    public string CommercialMode { get; set; } = string.Empty;
    public string CourseCautions { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string EarthRotationSpeedupFactor { get; set; } = string.Empty;
    public string FastRepairsLimit { get; set; } = string.Empty;
    public string FogLevel { get; set; } = string.Empty;
    public int GreenWhiteCheckeredLimit { get; set; }
    public int HardcoreLevel { get; set; }
    public int HasOpenRegistration { get; set; }
    public string IncidentLimit { get; set; } = string.Empty;
    public int IsFixedSetup { get; set; }
    public string NightMode { get; set; } = string.Empty;
    public int NumJokerLaps { get; set; }
    public int NumStarters { get; set; }
    public string QualifyScoring { get; set; } = string.Empty;
    public string RelativeHumidity { get; set; } = string.Empty;
    public string Restarts { get; set; } = string.Empty;
    public int ShortParadeLap { get; set; }
    public string Skies { get; set; } = string.Empty;
    public int StandingStart { get; set; }
    public string StrictLapsChecking { get; set; } = string.Empty;
    public string StartingGrid { get; set; } = string.Empty;
    public string TimeOfDay { get; set; } = string.Empty;
    public int Unofficial { get; set; }
    public string WeatherTemp { get; set; } = string.Empty;
    public string WeatherType { get; set; } = string.Empty;
    public string WindDirection { get; set; } = string.Empty;
    public string WindSpeed { get; set; } = string.Empty;
}
