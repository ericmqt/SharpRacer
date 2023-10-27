namespace SharpRacer.SessionInfo.Yaml;
public class CameraGroupNodeTests
{
    [Fact]
    public void Ctor_Test()
    {
        var node = new CameraGroupNode();

        Assert.NotNull(node.Cameras);
        Assert.Equal(default, node.GroupNum);
    }
}
