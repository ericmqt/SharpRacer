namespace SharpRacer.Telemetry;

/// <summary>
/// Describes the active pacing mode.
/// </summary>
/// <remarks>See irsdk_PaceMode</remarks>
public enum PaceMode : uint
{
    /// <summary>
    /// Single-file start.
    /// </summary>
    SingleFileStart = 0,

    /// <summary>
    /// Double-file start.
    /// </summary>
    DoubleFileStart = 1,

    /// <summary>
    /// Single-file restart.
    /// </summary>
    SingleFileRestart = 2,

    /// <summary>
    /// Double-file restart.
    /// </summary>
    DoubleFileRestart = 3,

    /// <summary>
    /// Not pacing.
    /// </summary>
    NotPacing = 4
}