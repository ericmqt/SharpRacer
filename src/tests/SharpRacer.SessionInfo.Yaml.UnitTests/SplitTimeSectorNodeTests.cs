namespace SharpRacer.SessionInfo.Yaml;
public class SplitTimeSectorNodeTests
{
    [Fact]
    public void Ctor_Test()
    {
        var node = new SplitTimeSectorNode();

        Assert.Equal(default, node.SectorNum);
        Assert.Equal(default, node.SectorStartPct);
    }
}
