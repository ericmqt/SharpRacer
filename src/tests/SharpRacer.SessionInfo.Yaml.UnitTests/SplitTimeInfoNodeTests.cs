namespace SharpRacer.SessionInfo.Yaml;
public class SplitTimeInfoNodeTests
{
    [Fact]
    public void Ctor_Test()
    {
        var node = new SplitTimeInfoNode();

        Assert.NotNull(node.Sectors);
    }
}
