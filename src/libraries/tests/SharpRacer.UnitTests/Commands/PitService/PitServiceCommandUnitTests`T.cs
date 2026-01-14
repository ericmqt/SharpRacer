using SharpRacer.Extensions.Xunit.Utilities;

namespace SharpRacer.Commands.PitService;

public abstract class PitServiceCommandUnitTests<TCommand, TSelf> : CommandUnitTests<TCommand, TSelf>
    where TCommand : struct, ISimulatorCommand<TCommand>
    where TSelf : class, IPitServiceCommandUnitTests<TCommand>
{

    public static TheoryData<PitServiceCommandType> InvalidPitServiceCommandTypesTestData { get; }
        = GetInvalidPitServiceCommandTypesTestData();

    [Theory]
    [MemberData(nameof(InvalidPitServiceCommandTypesTestData))]
    public void Parse_ThrowIfPitServiceCommandTypeNotValidTest(PitServiceCommandType pitServiceCommandType)
    {
        var msg = ModifyValidCommandMessage(commandId: (ushort)TSelf.CommandId, (ushort)pitServiceCommandType);

        Assert.Throws<CommandMessageParseException>(() => TCommand.Parse(msg));
    }

    [Fact]
    public void Parse_ThrowIfPitServiceCommandTypeUndefinedTest()
    {
        var msg = ModifyValidCommandMessage(commandId: (ushort)TSelf.CommandId, (ushort)GetUndefinedPitServiceCommandType());

        Assert.Throws<CommandMessageParseException>(() => TCommand.Parse(msg));
    }

    [Theory]
    [MemberData(nameof(InvalidPitServiceCommandTypesTestData))]
    public void TryParse_ReturnFalseIfPitServiceCommandTypeNotValidTest(PitServiceCommandType pitServiceCommandType)
    {
        var msg = ModifyValidCommandMessage(commandId: (ushort)TSelf.CommandId, (ushort)pitServiceCommandType);

        Assert.False(TCommand.TryParse(msg, out var parsedCommand));
        Assert.Equal(default, parsedCommand);
    }

    [Fact]
    public void TryParse_ReturnFalseIfPitServiceCommandTypeUndefinedTest()
    {
        var msg = ModifyValidCommandMessage(commandId: (ushort)TSelf.CommandId, arg1: (ushort)GetUndefinedPitServiceCommandType());

        Assert.False(TCommand.TryParse(msg, out var parsedCommand));
        Assert.Equal(default, parsedCommand);
    }

    private static TheoryData<PitServiceCommandType> GetInvalidPitServiceCommandTypesTestData()
    {
        var invalidCommandTypes = Enum.GetValues<PitServiceCommandType>().Where(x => !TSelf.PitServiceCommandTypes.Contains(x));

        return new TheoryData<PitServiceCommandType>(invalidCommandTypes);
    }

    protected static PitServiceCommandType GetUndefinedPitServiceCommandType()
    {
        var undefinedIntegralValue = EnumTestUtilities.MaxIntegralValue<PitServiceCommandType, ushort>() + 1;

        return (PitServiceCommandType)undefinedIntegralValue;
    }
}
