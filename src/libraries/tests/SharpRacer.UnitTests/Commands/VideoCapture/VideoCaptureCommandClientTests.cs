using Moq;
using SharpRacer.Commands.Interop;

namespace SharpRacer.Commands.VideoCapture;

public class VideoCaptureCommandClientTests : CommandClientUnitTests
{
    [Fact]
    public void Ctor_CommandSinkTest()
    {
        var commandSinkMock = Mocks.Create<ISimulatorCommandSink>();

        var client = new VideoCaptureCommandClient(commandSinkMock.Object);

        Assert.Equal(commandSinkMock.Object, client.CommandSink);
    }

    [Fact]
    public void Ctor_ThrowIfCommandSinkIsNullTest()
    {
        Assert.Throws<ArgumentNullException>(() => new VideoCaptureCommandClient(null!));
    }

    [Fact]
    public void HideTimer_Test()
    {
        var command = new VideoCaptureCommand(VideoCaptureCommandType.HideVideoTimer);
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new VideoCaptureCommandClient(CommandSink);
        client.HideTimer();

        Mocks.Verify();
    }

    [Fact]
    public void Screenshot_Test()
    {
        var command = new VideoCaptureCommand(VideoCaptureCommandType.CaptureScreenshot);
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new VideoCaptureCommandClient(CommandSink);
        client.Screenshot();

        Mocks.Verify();
    }

    [Fact]
    public void ShowTimer_Test()
    {
        var command = new VideoCaptureCommand(VideoCaptureCommandType.ShowVideoTimer);
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new VideoCaptureCommandClient(CommandSink);
        client.ShowTimer();

        Mocks.Verify();
    }

    [Fact]
    public void Start_Test()
    {
        var command = new VideoCaptureCommand(VideoCaptureCommandType.BeginVideoCapture);
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new VideoCaptureCommandClient(CommandSink);
        client.Start();

        Mocks.Verify();
    }

    [Fact]
    public void Stop_Test()
    {
        var command = new VideoCaptureCommand(VideoCaptureCommandType.EndVideoCapture);
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new VideoCaptureCommandClient(CommandSink);
        client.Stop();

        Mocks.Verify();
    }

    [Fact]
    public void ToggleVideoCapture_Test()
    {
        var command = new VideoCaptureCommand(VideoCaptureCommandType.ToggleVideoCapture);
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new VideoCaptureCommandClient(CommandSink);
        client.ToggleVideoCapture();

        Mocks.Verify();
    }
}
