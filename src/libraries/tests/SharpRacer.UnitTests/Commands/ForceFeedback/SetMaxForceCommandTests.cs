namespace SharpRacer.Commands.ForceFeedback;

public class SetMaxForceCommandTests : CommandUnitTests<SetMaxForceCommand, SetMaxForceCommandTests>, ICommandUnitTests<SetMaxForceCommand>
{
    public static SimulatorCommandId CommandId { get; } = SimulatorCommandId.ForceFeedback;

    [Fact]
    public void Ctor_Test()
    {
        float maxForceNm = NormalizePackedFloat(42.8f);

        var command = new SetMaxForceCommand(maxForceNm);

        Assert.Equal(maxForceNm, command.MaxForceNm);
    }

    [Fact]
    public void ToCommandMessage_Test()
    {
        float maxForceNm = NormalizePackedFloat(42.8f);
        var maxForceReal = (int)(maxForceNm * CommandMessageConstants.FloatArgument.ScaleFactor);

        var msg = new SetMaxForceCommand(maxForceNm).ToCommandMessage();

        CommandMessageAssert.Arg1Empty(msg);
        CommandMessageAssert.Arg2Equals(maxForceNm, msg);
    }

    public static IEnumerable<(SetMaxForceCommand Command1, SetMaxForceCommand Command2)> EnumerateEqualityValues()
    {
        float maxForceNm = NormalizePackedFloat(42.8f);

        yield return (new(maxForceNm), new(maxForceNm));
        yield return (default, default);
    }

    public static IEnumerable<(SetMaxForceCommand Command1, SetMaxForceCommand Command2)> EnumerateInequalityValues()
    {
        yield return (new(NormalizePackedFloat(21.4f)), new(NormalizePackedFloat(42.8f)));
        yield return (new(NormalizePackedFloat(21.4f)), default);
    }

    public static IEnumerable<SetMaxForceCommand> EnumerateValidCommands()
    {
        yield return new SetMaxForceCommand(0.0f);
        yield return new SetMaxForceCommand(NormalizePackedFloat(42.8f));
    }

    private static float NormalizePackedFloat(float value)
    {
        // Apply the simulator command floating-point argument packing scheme to the specified value and return it, allowing the returned
        // value to be passed to a command

        // When equality testing with float values that get packed into command args, unpacking the float for equality checks can return
        // a slightly different value due to the loss of precision from the packing process. To ensure a float passed into a command will
        // equal the one extracted from its SimulatorCommandArguments equivalent, we convert the specified float 

        int real = (int)(value * CommandMessageConstants.FloatArgument.ScaleFactor);

        return real / CommandMessageConstants.FloatArgument.ScaleFactor;
    }
}
