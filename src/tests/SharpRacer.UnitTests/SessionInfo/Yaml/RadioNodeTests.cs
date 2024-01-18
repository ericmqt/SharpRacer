namespace SharpRacer.SessionInfo.Yaml;
public class RadioNodeTests
{
    [Fact]
    public void Ctor_Test()
    {
        var node = new RadioNode();

        Assert.NotNull(node.Frequencies);

        Assert.Equal(default, node.HopCount);
        Assert.Equal(default, node.NumFrequencies);
        Assert.Equal(default, node.RadioNum);
        Assert.Equal(default, node.ScanningIsOn);
        Assert.Equal(default, node.TunedToFrequencyNum);
    }
}
