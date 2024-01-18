namespace SharpRacer.SessionInfo.Yaml;
public class QualifyResultNodeTests
{
    [Fact]
    public void Ctor_Test()
    {
        var node = new QualifyResultNode();

        Assert.Equal(default, node.CarIdx);
        Assert.Equal(default, node.ClassPosition);
        Assert.Equal(default, node.FastestLap);
        Assert.Equal(default, node.FastestTime);
        Assert.Equal(default, node.Position);
    }
}
