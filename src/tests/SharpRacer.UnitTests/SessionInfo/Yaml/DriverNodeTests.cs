namespace SharpRacer.SessionInfo.Yaml;
public class DriverNodeTests
{
    [Fact]
    public void Ctor_Test()
    {
        var node = new DriverNode();

        Assert.Null(node.AbbrevName);
        Assert.Null(node.CarClassShortName);
        Assert.Null(node.Initials);

        Assert.Equal(default, node.CarClassColor);
        Assert.Equal(default, node.CarClassEstLapTime);
        Assert.Equal(default, node.CarClassID);
        Assert.Equal(default, node.CarClassLicenseLevel);
        Assert.Equal(default, node.CarClassRelSpeed);
        Assert.Equal(default, node.CarID);
        Assert.Equal(default, node.CarIdx);
        Assert.Equal(default, node.CarIsAI);
        Assert.Equal(default, node.CarIsElectric);
        Assert.Equal(default, node.CarIsPaceCar);
        Assert.Equal(default, node.CarNumberRaw);
        Assert.Equal(default, node.CarSponsor1);
        Assert.Equal(default, node.CarSponsor2);
        Assert.Equal(default, node.CurDriverIncidentCount);
        Assert.Equal(default, node.IRating);
        Assert.Equal(default, node.IsSpectator);
        Assert.Equal(default, node.LicColor);
        Assert.Equal(default, node.LicLevel);
        Assert.Equal(default, node.LicSubLevel);
        Assert.Equal(default, node.TeamID);
        Assert.Equal(default, node.TeamIncidentCount);
        Assert.Equal(default, node.UserID);
    }
}
