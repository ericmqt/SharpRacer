namespace SharpRacer.SessionInfo.Yaml;
public class WeekendInfoNodeTests
{
    [Fact]
    public void Ctor_Tests()
    {
        var node = new WeekendInfoNode();

        Assert.NotNull(node.TelemetryOptions);
        Assert.NotNull(node.WeekendOptions);

        Assert.Equal(default, node.HeatRacing);
        Assert.Equal(default, node.LeagueID);
        Assert.Equal(default, node.MaxDrivers);
        Assert.Equal(default, node.MinDrivers);
        Assert.Equal(default, node.NumCarClasses);
        Assert.Equal(default, node.NumCarTypes);
        Assert.Equal(default, node.Official);
        Assert.Equal(default, node.QualifierMustStartRace);
        Assert.Equal(default, node.RaceWeek);
        Assert.Equal(default, node.SeasonID);
        Assert.Equal(default, node.SeriesID);
        Assert.Equal(default, node.SessionID);
        Assert.Equal(default, node.SubSessionID);
        Assert.Equal(default, node.TeamRacing);
        Assert.Equal(default, node.TrackCleanup);
        Assert.Equal(default, node.TrackDynamicTrack);
        Assert.Equal(default, node.TrackID);
        Assert.Equal(default, node.TrackNumTurns);
    }
}
