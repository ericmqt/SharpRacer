namespace SharpRacer.SessionInfo.Yaml;
public class SessionResultNodeTests
{
    [Fact]
    public void Ctor_Test()
    {
        var node = new SessionResultNode();

        Assert.Equal(default, node.CarIdx);
        Assert.Equal(default, node.ClassPosition);
        Assert.Equal(default, node.FastestLap);
        Assert.Equal(default, node.FastestTime);
        Assert.Equal(default, node.Incidents);
        Assert.Equal(default, node.JokerLapsComplete);
        Assert.Equal(default, node.Lap);
        Assert.Equal(default, node.LapsComplete);
        Assert.Equal(default, node.LapsDriven);
        Assert.Equal(default, node.LapsLed);
        Assert.Equal(default, node.LastTime);
        Assert.Equal(default, node.Position);
        Assert.Equal(default, node.ReasonOutId);
        Assert.Equal(string.Empty, node.ReasonOutStr);
        Assert.Equal(default, node.Time);
    }
}
