namespace SharpRacer.SessionInfo.Yaml;
public class SessionInfoDocumentModelTests
{
    [Fact]
    public void Ctor_Test()
    {
        var sessionInfo = new SessionInfoDocumentModel();

        Assert.NotNull(sessionInfo.CameraInfo);
        Assert.NotNull(sessionInfo.DriverInfo);
        Assert.Null(sessionInfo.QualifyResultsInfo);
        Assert.NotNull(sessionInfo.RadioInfo);
        Assert.NotNull(sessionInfo.SessionInfo);
        Assert.NotNull(sessionInfo.SplitTimeInfo);
        Assert.NotNull(sessionInfo.WeekendInfo);
    }

    [Fact]
    public void FromYaml_ThrowsOnNullOrEmptyString()
    {
        Assert.Throws<ArgumentException>(() => SessionInfoDocumentModel.FromYaml(string.Empty));
        Assert.Throws<ArgumentException>(() => SessionInfoDocumentModel.FromYaml(null!));
    }
}
