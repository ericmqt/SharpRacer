namespace SharpRacer.SessionInfo.Yaml;
public class TelemetryOptionsNodeTests
{
    [Fact]
    public void Ctor_Test()
    {
        var node = new TelemetryOptionsNode();

        Assert.Equal(string.Empty, node.TelemetryDiskFile);
    }
}
