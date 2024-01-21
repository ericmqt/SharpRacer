namespace SharpRacer.SessionInfo.Yaml;
public class QualifyResultsInfoNodeTests
{
    [Fact]
    public void Ctor_Test()
    {
        var node = new QualifyResultsInfoNode();

        Assert.NotNull(node.Results);
    }
}
