using SharpRacer.Extensions.Xunit.Utilities;

namespace SharpRacer.Commands.Replay;

public class SearchCommandTests : CommandUnitTests<SearchCommand, SearchCommandTests>, ICommandUnitTests<SearchCommand>
{
    public static SimulatorCommandId CommandId { get; } = SimulatorCommandId.ReplaySearch;

    [Fact]
    public void Ctor_Test()
    {
        const ReplaySearchMode searchMode = ReplaySearchMode.End;

        var cmd = new SearchCommand(searchMode);

        Assert.Equal(searchMode, cmd.SearchMode);
    }

    [Fact]
    public void ToCommandMessage_Test()
    {
        const ReplaySearchMode searchMode = ReplaySearchMode.End;

        var cmd = new SearchCommand(searchMode);
        var msg = cmd.ToCommandMessage();

        CommandMessageAssert.Arg1Equals(searchMode, msg);
        CommandMessageAssert.Arg2Empty(msg);
        CommandMessageAssert.Arg3Empty(msg);
    }

    [Fact]
    public void Parse_ThrowIfSearchModeNotValidTest()
    {
        var invalidSearchModeVal = (ushort)(EnumTestUtilities.MaxIntegralValue<ReplaySearchMode, ushort>() + 1);

        var msg = ModifyValidCommandMessage(arg1: invalidSearchModeVal);

        Assert.Throws<CommandMessageParseException>(() => SearchCommand.Parse(msg));
    }

    [Fact]
    public void TryParse_ReturnFalseIfSearchModeNotValidTest()
    {
        var invalidSearchModeVal = (ushort)(EnumTestUtilities.MaxIntegralValue<ReplaySearchMode, ushort>() + 1);

        var msg = ModifyValidCommandMessage(arg1: invalidSearchModeVal);

        Assert.False(SearchCommand.TryParse(msg, out var parsedCommand));
        Assert.Equal(default, parsedCommand);
    }

    public static IEnumerable<(SearchCommand Command1, SearchCommand Command2)> EnumerateEqualityValues()
    {
        yield return (new(ReplaySearchMode.Start), new(ReplaySearchMode.Start));
        yield return (new(ReplaySearchMode.PreviousIncident), new(ReplaySearchMode.PreviousIncident));
        yield return (default, default);
    }

    public static IEnumerable<(SearchCommand Command1, SearchCommand Command2)> EnumerateInequalityValues()
    {
        yield return (new(ReplaySearchMode.Start), new(ReplaySearchMode.End));
        yield return (new(ReplaySearchMode.NextIncident), new(ReplaySearchMode.PreviousSession));
        yield return (default, new(ReplaySearchMode.End));
    }

    public static IEnumerable<SearchCommand> EnumerateValidCommands()
    {
        yield return new(ReplaySearchMode.Start);
        yield return new(ReplaySearchMode.End);
        yield return new(ReplaySearchMode.NextIncident);
    }
}
