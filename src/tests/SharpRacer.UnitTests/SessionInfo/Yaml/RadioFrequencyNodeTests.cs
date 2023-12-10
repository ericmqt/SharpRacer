namespace SharpRacer.SessionInfo.Yaml;
public class RadioFrequencyNodeTests
{
    [Fact]
    public void Ctor_Test()
    {
        var node = new RadioFrequencyNode();

        Assert.Null(node.FrequencyName);

        Assert.Equal(default, node.CanScan);
        Assert.Equal(default, node.CanSquawk);
        Assert.Equal(default, node.CarIdx);
        Assert.Equal(default, node.ClubID);
        Assert.Equal(default, node.EntryIdx);
        Assert.Equal(default, node.FrequencyNum);
        Assert.Equal(default, node.IsDeletable);
        Assert.Equal(default, node.IsMutable);
        Assert.Equal(default, node.Muted);
        Assert.Equal(default, node.Priority);
    }
}
