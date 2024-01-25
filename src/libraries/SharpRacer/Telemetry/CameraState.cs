﻿namespace SharpRacer.Telemetry;

/// <summary>
/// Camera state flags.
/// </summary>
/// <remarks>See: irsdk_CameraState</remarks>
[Flags]
public enum CameraState : uint
{
#pragma warning disable CA1008 // Enums should have zero value
    Unknown = 0,
#pragma warning restore CA1008 // Enums should have zero value
    /// <summary>
    /// The camera tool can only be activated if viewing the session screen (out of car).
    /// </summary>
    IsSessionScreen = 0x0001,

    /// <summary>
    /// The scenic camera is active (no focus car).
    /// </summary>
    IsScenicActive = 0x0002,

    //these can be changed with a broadcast message
    CamToolActive = 0x0004,
    UIHidden = 0x0008,
    UseAutoShotSelection = 0x0010,
    UseTemporaryEdits = 0x0020,
    UseKeyAcceleration = 0x0040,
    UseKey10xAcceleration = 0x0080,
    UseMouseAimMode = 0x0100
}
