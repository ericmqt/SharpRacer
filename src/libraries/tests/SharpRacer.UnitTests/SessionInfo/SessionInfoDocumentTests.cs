namespace SharpRacer.SessionInfo;
public class SessionInfoDocumentTests
{
    [Fact]
    public void Ctor_Test()
    {
        var yaml = "test";
        int version = 3;

        var doc = new SessionInfoDocument(yaml, version);

        Assert.Equal(yaml, doc.YamlDocument);
        Assert.Equal(version, doc.Version);
    }

    [Fact]
    public void Ctor_ThrowsOnNullYamlStringTest()
    {
        Assert.Throws<ArgumentNullException>(() => new SessionInfoDocument(null!, 34));
    }
}
