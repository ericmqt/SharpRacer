namespace SharpRacer.SessionInfo.Yaml;
public class SessionInfoNodeTests
{
    [Fact]
    public void Ctor_Test()
    {
        var node = new SessionInfoNode();

        Assert.NotNull(node.Sessions);
    }
}
