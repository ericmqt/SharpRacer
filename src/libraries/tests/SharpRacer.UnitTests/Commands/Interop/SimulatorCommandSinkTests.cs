using Moq;
using SharpRacer.Commands.Camera;
using SharpRacer.Interop;

namespace SharpRacer.Commands.Interop;

public class SimulatorCommandSinkTests
{
    [Fact]
    public void Ctor_NotifyMessageSinkTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);
        var notifyMessageSinkMock = mocks.Create<ISimulatorNotifyMessageSink>();

        var commandSink = new SimulatorCommandSink(notifyMessageSinkMock.Object);
        Assert.Equal(notifyMessageSinkMock.Object, commandSink.MessageSink);
    }

    [Fact]
    public void Ctor_ThrowIfNotifyMessageSinkIsNullTest()
    {
        Assert.Throws<ArgumentNullException>(() => new SimulatorCommandSink(null!));
    }

    [Fact]
    public void Send_CommandMessage_Test()
    {
        var cameraState = CameraState.IsSessionScreenActive
            | CameraState.IsScenicCameraActive
            | CameraState.IsCameraToolActive
            | CameraState.IsUIHidden
            | CameraState.UseAutoShotSelection
            | CameraState.UseTemporaryEdits
            | CameraState.UseKeyAcceleration
            | CameraState.UseKey10xAcceleration
            | CameraState.UseMouseAimMode;

        var commandMessage = new SetCameraStateCommand(cameraState).ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        var mocks = new MockRepository(MockBehavior.Strict);
        var notifyMessageSinkMock = mocks.Create<ISimulatorNotifyMessageSink>();

        notifyMessageSinkMock.Setup(x => x.Send(It.IsAny<SimulatorNotifyMessageData>()))
            .Callback<SimulatorNotifyMessageData>(msg => Assert.Equal(notifyMessage, msg))
            .Verifiable(Times.Once());

        var commandSink = new SimulatorCommandSink(notifyMessageSinkMock.Object);
        commandSink.Send(commandMessage);

        mocks.Verify();
    }

    [Fact]
    public void Send_TCommand_Test()
    {
        var cameraState = CameraState.IsSessionScreenActive
            | CameraState.IsScenicCameraActive
            | CameraState.IsCameraToolActive
            | CameraState.IsUIHidden
            | CameraState.UseAutoShotSelection
            | CameraState.UseTemporaryEdits
            | CameraState.UseKeyAcceleration
            | CameraState.UseKey10xAcceleration
            | CameraState.UseMouseAimMode;

        var command = new SetCameraStateCommand(cameraState);
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        var mocks = new MockRepository(MockBehavior.Strict);
        var notifyMessageSinkMock = mocks.Create<ISimulatorNotifyMessageSink>();

        notifyMessageSinkMock.Setup(x => x.Send(It.IsAny<SimulatorNotifyMessageData>()))
            .Callback<SimulatorNotifyMessageData>(msg => Assert.Equal(notifyMessage, msg))
            .Verifiable(Times.Once());

        var commandSink = new SimulatorCommandSink(notifyMessageSinkMock.Object);
        commandSink.Send<SetCameraStateCommand>(command);

        mocks.Verify();
    }
}
