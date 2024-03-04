namespace SharpRacer;
public class ContentVersionTests
{
    [Fact]
    public void Parse_Test()
    {
        var version = ContentVersion.Parse("2023.04.18.02", null);

        Assert.Equal(2023, version.Major);
        Assert.Equal(4, version.Minor);
        Assert.Equal(18, version.Build);
        Assert.Equal(2, version.Patch);
    }

    [Fact]
    public void ToString_Test()
    {
        var version = new ContentVersion(2023, 4, 18, 2);

        Assert.Equal("2023.04.18.02", version.ToString());
    }
}
