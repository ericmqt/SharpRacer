namespace SharpRacer.Enums;

public class EnumExtensionsTests
{
    [Fact]
    public void GetIncidentPenalty_Test()
    {
        foreach (var (report, penalty) in EnumerateIncidentReportAndPenaltyCombinations())
        {
            var flags = (IncidentFlags)report | (IncidentFlags)penalty;

            Assert.Equal(penalty, flags.GetIncidentPenalty());
        }
    }

    [Fact]
    public void GetIncidentType_Test()
    {
        foreach (var (report, penalty) in EnumerateIncidentReportAndPenaltyCombinations())
        {
            var flags = (IncidentFlags)report | (IncidentFlags)penalty;

            Assert.Equal(report, flags.GetIncidentType());
        }
    }

    private static IEnumerable<(IncidentType Report, IncidentPenalty Penalty)> EnumerateIncidentReportAndPenaltyCombinations()
    {
        foreach (var report in Enum.GetValues<IncidentType>())
        {
            foreach (var penalty in Enum.GetValues<IncidentPenalty>())
            {
                yield return new(report, penalty);
            }
        }
    }
}
