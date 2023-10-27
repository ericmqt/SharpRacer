namespace SharpRacer.SessionInfo.Yaml;
public class SessionInfoDocumentTests
{
    [Fact]
    public void Ctor_Test()
    {
        var sessionInfo = new SessionInfoDocument();

        Assert.NotNull(sessionInfo.CameraInfo);
        Assert.NotNull(sessionInfo.DriverInfo);
        Assert.NotNull(sessionInfo.RadioInfo);
        Assert.NotNull(sessionInfo.SessionInfo);
        Assert.NotNull(sessionInfo.SplitTimeInfo);
        Assert.NotNull(sessionInfo.WeekendInfo);
    }

    [Fact]
    public void FromYaml_ThrowsOnNullOrEmptyString()
    {
        Assert.Throws<ArgumentException>(() => SessionInfoDocument.FromYaml(string.Empty));
        Assert.Throws<ArgumentException>(() => SessionInfoDocument.FromYaml(null!));
    }
}
