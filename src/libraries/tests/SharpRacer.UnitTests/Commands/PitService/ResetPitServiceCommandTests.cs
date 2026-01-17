namespace SharpRacer.Commands.PitService;

public class ResetPitServiceCommandTests :
    PitServiceCommandUnitTests<ResetPitServiceCommand, ResetPitServiceCommandTests>,
    IPitServiceCommandUnitTests<ResetPitServiceCommand>
{
    public static SimulatorCommandId CommandId { get; } = SimulatorCommandId.PitService;

    public static PitServiceCommandType[] PitServiceCommandTypes { get; } = [
            PitServiceCommandType.Reset,
            PitServiceCommandType.ResetTireChange,
            PitServiceCommandType.ResetWindscreenTearOff,
            PitServiceCommandType.ResetFastRepair,
            PitServiceCommandType.ResetFuel
        ];

    [Fact]
    public void Ctor_Test()
    {
        var service = PitServiceResetType.Fuel;
        var command = new ResetPitServiceCommand(service);

        Assert.Equal(service, command.Service);
    }

    [Fact]
    public void ToCommandMessage_Test()
    {
        var service = PitServiceResetType.Fuel;

        var msg = new ResetPitServiceCommand(service).ToCommandMessage();

        CommandMessageAssert.Arg1Equals(service, msg);
        CommandMessageAssert.Arg2Empty(msg);
        CommandMessageAssert.Arg3Empty(msg);
    }

    public static IEnumerable<(ResetPitServiceCommand Command1, ResetPitServiceCommand Command2)> EnumerateEqualityValues()
    {
        yield return (new(PitServiceResetType.All), new(PitServiceResetType.All));
        yield return (new(PitServiceResetType.Fuel), new(PitServiceResetType.Fuel));
        yield return (default, default);
    }

    public static IEnumerable<(ResetPitServiceCommand Command1, ResetPitServiceCommand Command2)> EnumerateInequalityValues()
    {
        yield return (new(PitServiceResetType.All), new(PitServiceResetType.Fuel));
        yield return (new(PitServiceResetType.FastRepair), new(PitServiceResetType.WindscreenTearOff));
        yield return (new(PitServiceResetType.TireChange), default);
    }

    public static IEnumerable<ResetPitServiceCommand> EnumerateValidCommands()
    {
        yield return new(PitServiceResetType.All);
        yield return new(PitServiceResetType.TireChange);
        yield return new(PitServiceResetType.WindscreenTearOff);
        yield return new(PitServiceResetType.FastRepair);
        yield return new(PitServiceResetType.Fuel);
    }

    public static IEnumerable<PitServiceCommandType> EnumerateValidPitServiceCommandTypes()
    {
        yield return PitServiceCommandType.Reset;
        yield return PitServiceCommandType.ResetTireChange;
        yield return PitServiceCommandType.ResetWindscreenTearOff;
        yield return PitServiceCommandType.ResetFastRepair;
        yield return PitServiceCommandType.ResetFuel;
    }
}
