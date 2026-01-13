using Moq;
using SharpRacer.Commands.Interop;

namespace SharpRacer.Commands.ForceFeedback;

public class ForceFeedbackCommandClientTests : CommandClientUnitTests
{
    [Fact]
    public void Ctor_CommandSinkTest()
    {
        var commandSinkMock = Mocks.Create<ISimulatorCommandSink>();

        var client = new ForceFeedbackCommandClient(commandSinkMock.Object);

        Assert.Equal(commandSinkMock.Object, client.CommandSink);
    }

    [Fact]
    public void Ctor_ThrowIfCommandSinkIsNullTest()
    {
        Assert.Throws<ArgumentNullException>(() => new ForceFeedbackCommandClient(null!));
    }

    [Fact]
    public void SetMaxForce_Test()
    {
        float maxForceNm = NormalizePackedFloat(32.0f);
        var command = new SetMaxForceCommand(maxForceNm);
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new ForceFeedbackCommandClient(CommandSink);
        client.SetMaxForce(maxForceNm);

        Mocks.Verify();
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
