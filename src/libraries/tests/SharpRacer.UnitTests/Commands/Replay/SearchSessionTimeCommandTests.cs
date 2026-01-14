namespace SharpRacer.Commands.Replay;

public class SearchSessionTimeCommandTests : CommandUnitTests<SearchSessionTimeCommand, SearchSessionTimeCommandTests>, ICommandUnitTests<SearchSessionTimeCommand>
{
    public static SimulatorCommandId CommandId { get; } = SimulatorCommandId.ReplaySearchSessionTime;

    [Fact]
    public void Ctor_Test()
    {
        const ushort sessionNumber = 4;
        const int sessionTimeMs = 1234567890;

        var cmd = new SearchSessionTimeCommand(sessionNumber, sessionTimeMs);

        Assert.Equal(sessionNumber, cmd.SessionNumber);
        Assert.Equal(sessionTimeMs, cmd.SessionTimeMs);
    }

    [Fact]
    public void ToCommandMessage_Test()
    {
        const ushort sessionNumber = 4;
        const int sessionTimeMs = 1234567890;

        var cmd = new SearchSessionTimeCommand(sessionNumber, sessionTimeMs);
        var msg = cmd.ToCommandMessage();

        CommandMessageAssert.Arg1Equals(sessionNumber, msg);
        CommandMessageAssert.Arg2Equals(sessionTimeMs, msg);
    }

    public static IEnumerable<(SearchSessionTimeCommand Command1, SearchSessionTimeCommand Command2)> EnumerateEqualityValues()
    {
        yield return (new(4, 123445789), new(4, 123445789));
        yield return (default, default);
    }

    public static IEnumerable<(SearchSessionTimeCommand Command1, SearchSessionTimeCommand Command2)> EnumerateInequalityValues()
    {
        yield return (new(1, 123445789), new(2, 123445789));
        yield return (new(4, 12345), new(4, 67890));
        yield return (default, new(1, 123445789));
    }

    public static IEnumerable<SearchSessionTimeCommand> EnumerateValidCommands()
    {
        yield return new(1, 123445789);
        yield return new(2, 123445789);
        yield return new(3, 12345);
    }
}
