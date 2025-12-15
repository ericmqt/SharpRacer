namespace SharpRacer.Internal.Connections;
public class ConnectionWorkerThreadFactoryTests
{
    [Fact]
    public void DefaultInstance_Test()
    {
        Assert.NotNull(ConnectionWorkerThreadFactory.Default);
    }
}
