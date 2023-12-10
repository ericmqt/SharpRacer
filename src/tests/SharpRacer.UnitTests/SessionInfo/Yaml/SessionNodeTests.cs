namespace SharpRacer.SessionInfo.Yaml;
public class SessionNodeTests
{
    [Fact]
    public void Ctor_Test()
    {
        var node = new SessionNode();

        Assert.NotNull(node.ResultsFastestLap);
        Assert.NotNull(node.ResultsPositions);

        Assert.Equal(default, node.ResultsAverageLapTime);
        Assert.Equal(default, node.ResultsLapsComplete);
        Assert.Equal(default, node.ResultsNumCautionFlags);
        Assert.Equal(default, node.ResultsNumCautionLaps);
        Assert.Equal(default, node.ResultsNumLeadChanges);
        Assert.Equal(default, node.ResultsOfficial);
        Assert.Equal(default, node.SessionEnforceTireCompoundChange);
        Assert.Equal(default, node.SessionNum);
        Assert.Equal(default, node.SessionNumLapsToAvg);
        Assert.Equal(default, node.SessionRunGroupsUsed);
        Assert.Equal(default, node.SessionSkipped);

        Assert.Null(node.SessionLaps);
        Assert.Null(node.SessionName);
        Assert.Null(node.SessionSubType);
        Assert.Null(node.SessionTime);
        Assert.Null(node.SessionTrackRubberState);
        Assert.Null(node.SessionType);
    }
}
