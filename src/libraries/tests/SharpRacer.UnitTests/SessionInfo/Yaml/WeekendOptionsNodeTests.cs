namespace SharpRacer.SessionInfo.Yaml;
public class WeekendOptionsNodeTests
{
    [Fact]
    public void Ctor_Test()
    {
        var node = new WeekendOptionsNode();

        Assert.Equal(default, node.GreenWhiteCheckeredLimit);
        Assert.Equal(default, node.HardcoreLevel);
        Assert.Equal(default, node.HasOpenRegistration);
        Assert.Equal(default, node.IsFixedSetup);
        Assert.Equal(default, node.NumJokerLaps);
        Assert.Equal(default, node.NumStarters);
        Assert.Equal(default, node.ShortParadeLap);
        Assert.Equal(default, node.StandingStart);
        Assert.Equal(default, node.Unofficial);
    }
}
