namespace SharpRacer.SourceGenerators.Internal;
public class AssemblyInfoProviderTests
{
    [Fact]
    public void GetAssemblyTest()
    {
        var asm = AssemblyInfoProvider.GetAssembly();

        var expectedAsm = typeof(TelemetryVariablesGenerator).Assembly;

        Assert.Equal(expectedAsm, asm);
    }

    [Fact]
    public void GetFileVersionTest()
    {
        var fileVersion = AssemblyInfoProvider.GetFileVersion();

        Assert.NotNull(fileVersion);
        Assert.NotEqual(string.Empty, fileVersion);
    }

    [Fact]
    public void GetInformationalVersionTest()
    {
        var infoVersion = AssemblyInfoProvider.GetInformationalVersion();

        Assert.NotNull(infoVersion);
        Assert.NotEqual(string.Empty, infoVersion);
    }

    [Fact]
    public void GetProduct()
    {
        var product = AssemblyInfoProvider.GetProduct();

        Assert.NotNull(product);
        Assert.Equal("SharpRacer.SourceGenerators", product);
    }

    [Fact]
    public void GetVersionTest()
    {
        var version = AssemblyInfoProvider.GetVersion();

        Assert.NotNull(version);
    }
}
