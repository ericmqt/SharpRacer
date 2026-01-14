using Moq;
using SharpRacer.Commands.Interop;

namespace SharpRacer.Commands.Camera;

public class CameraCommandClientTests : CommandClientUnitTests
{
    [Fact]
    public void Ctor_CommandSinkTest()
    {
        var commandSinkMock = Mocks.Create<ISimulatorCommandSink>();

        var client = new CameraCommandClient(commandSinkMock.Object);

        Assert.Equal(commandSinkMock.Object, client.CommandSink);
    }

    [Fact]
    public void Ctor_ThrowIfCommandSinkIsNullTest()
    {
        Assert.Throws<ArgumentNullException>(() => new CameraCommandClient(null!));
    }

    [Fact]
    public void SetState_Test()
    {
        var cameraState = CameraState.UseAutoShotSelection;
        var command = new SetCameraStateCommand(cameraState);
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new CameraCommandClient(CommandSink);
        client.SetState(cameraState);

        Mocks.Verify();
    }

    [Fact]
    public void TargetDriver_DriverNumberTest()
    {
        ushort driverNumber = 12;
        ushort cameraGroup = 3;
        ushort cameraIndex = 42;

        var command = new TargetDriverCommand(driverNumber, cameraGroup, cameraIndex);
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new CameraCommandClient(CommandSink);
        client.TargetDriver(driverNumber, cameraGroup, cameraIndex);

        Mocks.Verify();
    }

    [Fact]
    public void TargetDriver_TargetModeTest()
    {
        var targetMode = CameraTargetMode.Leader;
        ushort cameraGroup = 3;
        ushort cameraIndex = 42;

        var command = new TargetDriverCommand(targetMode, cameraGroup, cameraIndex);
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new CameraCommandClient(CommandSink);
        client.TargetDriver(targetMode, cameraGroup, cameraIndex);

        Mocks.Verify();
    }

    [Fact]
    public void TargetRacePosition_PositionTest()
    {
        ushort carPosition = 12;
        ushort cameraGroup = 3;
        ushort cameraIndex = 42;

        var command = new TargetRacePositionCommand(carPosition, cameraGroup, cameraIndex);
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new CameraCommandClient(CommandSink);
        client.TargetRacePosition(carPosition, cameraGroup, cameraIndex);

        Mocks.Verify();
    }

    [Fact]
    public void TargetRacePosition_TargetModeTest()
    {
        var targetMode = CameraTargetMode.Leader;
        ushort cameraGroup = 3;
        ushort cameraIndex = 42;

        var command = new TargetRacePositionCommand(targetMode, cameraGroup, cameraIndex);
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new CameraCommandClient(CommandSink);
        client.TargetRacePosition(targetMode, cameraGroup, cameraIndex);

        Mocks.Verify();
    }
}
