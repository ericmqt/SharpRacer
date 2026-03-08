namespace SharpRacer.Enums;

/// <summary>
/// Defines the degree of wetness of the track.
/// </summary>
/// <remarks>See irsdk_TrackWetness in the iRacing SDK.</remarks>
public enum TrackWetness : uint
{
    /// <summary>
    /// Unknown.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// The track is dry.
    /// </summary>
    Dry = 1,

    /// <summary>
    /// The track is mostly dry.
    /// </summary>
    MostlyDry = 2,

    /// <summary>
    /// The track is very lightly wet.
    /// </summary>
    VeryLightlyWet = 3,

    /// <summary>
    /// The track is lightly wet.
    /// </summary>
    LightlyWet = 4,

    /// <summary>
    /// The track is very wet.
    /// </summary>
    VeryWet = 5,

    /// <summary>
    /// The track is extremely wet.
    /// </summary>
    ExtremelyWet = 6
}
