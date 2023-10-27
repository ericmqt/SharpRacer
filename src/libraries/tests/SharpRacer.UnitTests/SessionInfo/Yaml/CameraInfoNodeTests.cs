namespace SharpRacer.SessionInfo.Yaml;
public class CameraInfoNodeTests
{
    [Fact]
    public void Ctor_Test()
    {
        var node = new CameraInfoNode();

        Assert.NotNull(node.Groups);
    }
}
