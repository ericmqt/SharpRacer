namespace SharpRacer.SessionInfo.Yaml;
public class DriverInfoNodeTests
{
    [Fact]
    public void Ctor_Test()
    {
        var node = new DriverInfoNode();

        Assert.NotNull(node.Drivers);

        Assert.Equal(default, node.DriverCarVersion);
        Assert.Equal(string.Empty, node.DriverSetupLoadTypeName);
        Assert.Equal(string.Empty, node.DriverSetupName);

        Assert.Equal(default, node.DriverCarEngCylinderCount);
        Assert.Equal(default, node.DriverCarEstLapTime);
        Assert.Equal(default, node.DriverCarFuelKgPerLtr);
        Assert.Equal(default, node.DriverCarFuelMaxLtr);
        Assert.Equal(default, node.DriverCarGearNeutral);
        Assert.Equal(default, node.DriverCarGearNumForward);
        Assert.Equal(default, node.DriverCarGearReverse);
        Assert.Equal(default, node.DriverCarIdleRPM);
        Assert.Equal(default, node.DriverCarIdx);
        Assert.Equal(default, node.DriverCarIsElectric);
        Assert.Equal(default, node.DriverCarMaxFuelPct);
        Assert.Equal(default, node.DriverCarRedLine);
        Assert.Equal(default, node.DriverCarSLBlinkRPM);
        Assert.Equal(default, node.DriverCarSLFirstRPM);
        Assert.Equal(default, node.DriverCarSLLastRPM);
        Assert.Equal(default, node.DriverCarSLShiftRPM);
        Assert.Equal(default, node.DriverHeadPosX);
        Assert.Equal(default, node.DriverHeadPosY);
        Assert.Equal(default, node.DriverHeadPosZ);
        Assert.Equal(default, node.DriverIncidentCount);
        Assert.Equal(default, node.DriverPitTrkPct);
        Assert.Equal(default, node.DriverSetupIsModified);
        Assert.Equal(default, node.DriverSetupPassedTech);
        Assert.Equal(default, node.DriverUserID);
        Assert.Equal(default, node.PaceCarIdx);
    }
}
