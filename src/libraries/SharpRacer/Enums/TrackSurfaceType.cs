namespace SharpRacer;

/// <summary>
/// Defines track surface types.
/// </summary>
/// <remarks>
/// See irsdk_TrkSurf in the iRacing SDK.
/// </remarks>
public enum TrackSurfaceType : int
{
    /// <summary>
    /// Track surface is not in the world.
    /// </summary>
    SurfaceNotInWorld = -1,

    /// <summary>
    /// Track surface is an undefined material.
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// Asphalt surface (type 1).
    /// </summary>
    Asphalt1 = 1,

    /// <summary>
    /// Asphalt surface (type 2).
    /// </summary>
    Asphalt2 = 2,

    /// <summary>
    /// Asphalt surface (type 3).
    /// </summary>
    Asphalt3 = 3,

    /// <summary>
    /// Asphalt surface (type 4).
    /// </summary>
    Asphalt4 = 4,

    /// <summary>
    /// Concrete surface (type 1).
    /// </summary>
    Concrete1 = 5,

    /// <summary>
    /// Concrete surface (type 2).
    /// </summary>
    Concrete2 = 6,

    /// <summary>
    /// Racing dirt surface (type 1).
    /// </summary>
    RacingDirt1 = 7,

    /// <summary>
    /// Racing dirt surface (type 2).
    /// </summary>
    RacingDirt2 = 8,

    /// <summary>
    /// Paint surface (type 1).
    /// </summary>
    Paint1 = 9,

    /// <summary>
    /// Paint surface (type 2).
    /// </summary>
    Paint2 = 10,

    /// <summary>
    /// Rumble surface (type 1).
    /// </summary>
    Rumble1 = 11,

    /// <summary>
    /// Rumble surface (type 2).
    /// </summary>
    Rumble2 = 12,

    /// <summary>
    /// Rumble surface (type 3).
    /// </summary>
    Rumble3 = 13,

    /// <summary>
    /// Rumble surface (type 4).
    /// </summary>
    Rumble4 = 14,

    /// <summary>
    /// Grass surface (type 1).
    /// </summary>
    Grass1 = 15,

    /// <summary>
    /// Grass surface (type 2).
    /// </summary>
    Grass2 = 16,

    /// <summary>
    /// Grass surface (type 3).
    /// </summary>
    Grass3 = 17,

    /// <summary>
    /// Grass surface (type 4).
    /// </summary>
    Grass4 = 18,

    /// <summary>
    /// Dirt surface (type 1).
    /// </summary>
    Dirt1 = 19,

    /// <summary>
    /// Dirt surface (type 2).
    /// </summary>
    Dirt2 = 20,

    /// <summary>
    /// Dirt surface (type 3).
    /// </summary>
    Dirt3 = 21,

    /// <summary>
    /// Dirt surface (type 4).
    /// </summary>
    Dirt4 = 22,

    /// <summary>
    /// Sand surface.
    /// </summary>
    Sand = 23,

    /// <summary>
    /// Gravel surface (type 1).
    /// </summary>
    Gravel1 = 24,

    /// <summary>
    /// Gravel surface (type 2).
    /// </summary>
    Gravel2 = 25,

    /// <summary>
    /// Grasscrete surface.
    /// </summary>
    Grasscrete = 26,

    /// <summary>
    /// Astroturf surface.
    /// </summary>
    Astroturf = 27
}
