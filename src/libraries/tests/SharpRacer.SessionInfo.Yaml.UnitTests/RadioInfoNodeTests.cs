namespace SharpRacer.SessionInfo.Yaml;
public class RadioInfoNodeTests
{
    [Fact]
    public void Ctor_Test()
    {
        var node = new RadioInfoNode();

        Assert.NotNull(node.Radios);
        Assert.Equal(default, node.SelectedRadioNum);
    }
}
