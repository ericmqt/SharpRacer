using Moq;
using SharpRacer.Commands.Interop;

namespace SharpRacer.Commands.Telemetry;

public class TelemetryCommandClientTests : CommandClientUnitTests
{
    [Fact]
    public void Ctor_CommandSinkTest()
    {
        var commandSinkMock = Mocks.Create<ISimulatorCommandSink>();

        var client = new TelemetryCommandClient(commandSinkMock.Object);

        Assert.Equal(commandSinkMock.Object, client.CommandSink);
    }

    [Fact]
    public void Ctor_ThrowIfCommandSinkIsNullTest()
    {
        Assert.Throws<ArgumentNullException>(() => new TelemetryCommandClient(null!));
    }

    [Fact]
    public void Restart_Test()
    {
        var command = new TelemetryCommand(TelemetryCommandType.Restart);
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new TelemetryCommandClient(CommandSink);
        client.Restart();

        Mocks.Verify();
    }

    [Fact]
    public void Start_Test()
    {
        var command = new TelemetryCommand(TelemetryCommandType.Start);
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new TelemetryCommandClient(CommandSink);
        client.Start();

        Mocks.Verify();
    }

    [Fact]
    public void Stop_Test()
    {
        var command = new TelemetryCommand(TelemetryCommandType.Stop);
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new TelemetryCommandClient(CommandSink);
        client.Stop();

        Mocks.Verify();
    }
}
