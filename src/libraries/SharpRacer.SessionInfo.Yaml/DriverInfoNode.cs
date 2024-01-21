namespace SharpRacer.SessionInfo.Yaml;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public sealed class DriverInfoNode
{
    public int DriverCarEngCylinderCount { get; set; }
    public double DriverCarEstLapTime { get; set; }
    public double DriverCarFuelKgPerLtr { get; set; }
    public double DriverCarFuelMaxLtr { get; set; }
    public double DriverHeadPosX { get; set; }
    public double DriverHeadPosY { get; set; }
    public double DriverHeadPosZ { get; set; }
    public double DriverCarIdleRPM { get; set; }
    public int DriverCarIdx { get; set; }
    public int DriverCarIsElectric { get; set; }
    public double DriverCarMaxFuelPct { get; set; }
    public int DriverCarGearNeutral { get; set; }
    public int DriverCarGearNumForward { get; set; }
    public double DriverCarRedLine { get; set; }
    public int DriverCarGearReverse { get; set; }
    public double DriverCarSLFirstRPM { get; set; }
    public double DriverCarSLShiftRPM { get; set; }
    public double DriverCarSLLastRPM { get; set; }
    public double DriverCarSLBlinkRPM { get; set; }
    public string DriverCarVersion { get; set; } = string.Empty;
    public int DriverIncidentCount { get; set; }
    public double DriverPitTrkPct { get; set; }
    public string DriverSetupName { get; set; } = string.Empty;
    public int DriverSetupIsModified { get; set; }
    public string DriverSetupLoadTypeName { get; set; } = string.Empty;
    public int DriverSetupPassedTech { get; set; }
    public int DriverUserID { get; set; }
    public List<DriverNode> Drivers { get; set; } = new List<DriverNode>();
    public int PaceCarIdx { get; set; }
}