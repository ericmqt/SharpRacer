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
    public void TargetCar_CarNumberTest()
    {
        var carNumber = new CarNumber(12);
        ushort cameraGroup = 3;
        ushort cameraIndex = 42;

        var command = new TargetCarCommand(carNumber, cameraGroup, cameraIndex);
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new CameraCommandClient(CommandSink);
        client.TargetCar(carNumber, cameraGroup, cameraIndex);

        Mocks.Verify();
    }

    [Fact]
    public void TargetCar_CarNumber_ThrowIfCarNumberEqualsNoneTest()
    {
        ushort cameraGroup = 3;
        ushort cameraIndex = 42;

        var client = new CameraCommandClient(CommandSink);

        Assert.Throws<ArgumentException>(() => client.TargetCar(CarNumber.None, cameraGroup, cameraIndex));
        Assert.Throws<ArgumentException>(() => client.TargetCar(default(CarNumber), cameraGroup, cameraIndex));
    }

    [Fact]
    public void TargetCar_TargetModeTest()
    {
        var targetMode = CameraTargetMode.Leader;
        ushort cameraGroup = 3;
        ushort cameraIndex = 42;

        var command = new TargetCarCommand(targetMode, cameraGroup, cameraIndex);
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new CameraCommandClient(CommandSink);
        client.TargetCar(targetMode, cameraGroup, cameraIndex);

        Mocks.Verify();
    }

    [Fact]
    public void TargetCarByPosition_PositionTest()
    {
        ushort carPosition = 12;
        ushort cameraGroup = 3;
        ushort cameraIndex = 42;

        var command = new TargetRacePositionCommand(carPosition, cameraGroup, cameraIndex);
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new CameraCommandClient(CommandSink);
        client.TargetCarByPosition(carPosition, cameraGroup, cameraIndex);

        Mocks.Verify();
    }

    [Fact]
    public void TargetCarByPosition_TargetModeTest()
    {
        var targetMode = CameraTargetMode.Leader;
        ushort cameraGroup = 3;
        ushort cameraIndex = 42;

        var command = new TargetRacePositionCommand(targetMode, cameraGroup, cameraIndex);
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new CameraCommandClient(CommandSink);
        client.TargetCarByPosition(targetMode, cameraGroup, cameraIndex);

        Mocks.Verify();
    }
}
