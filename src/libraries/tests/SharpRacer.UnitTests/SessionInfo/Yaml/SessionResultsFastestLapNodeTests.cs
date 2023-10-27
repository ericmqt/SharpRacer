namespace SharpRacer.SessionInfo.Yaml;
public class SessionResultsFastestLapNodeTests
{
    [Fact]
    public void Ctor_Test()
    {
        var node = new SessionResultsFastestLapNode();

        Assert.Equal(default, node.CarIdx);
        Assert.Equal(default, node.FastestLap);
        Assert.Equal(default, node.FastestTime);
    }
}
