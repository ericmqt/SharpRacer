namespace SharpRacer.SessionInfo.Yaml;
public class CameraNodeTests
{
    [Fact]
    public void Ctor_Test()
    {
        var node = new CameraNode();

        Assert.Equal(default, node.CameraNum);
        Assert.Equal(string.Empty, node.CameraName);
    }
}
