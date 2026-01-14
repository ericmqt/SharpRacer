using SharpRacer.Commands.Camera;
using SharpRacer.Commands.Chat;
using SharpRacer.Commands.ForceFeedback;
using SharpRacer.Commands.Interop;
using SharpRacer.Commands.PitService;
using SharpRacer.Commands.Replay;
using SharpRacer.Commands.Telemetry;
using SharpRacer.Commands.Textures;
using SharpRacer.Commands.VideoCapture;

namespace SharpRacer.Commands;

public class CommandClientConstructorTests
{
    [Fact]
    public void CameraCommandClient_CtorTest()
    {
        var client = new CameraCommandClient();

        Assert.Equal(SimulatorCommandSink.Instance, client.CommandSink);
    }

    [Fact]
    public void ChatCommandClient_CtorTest()
    {
        var client = new ChatCommandClient();

        Assert.Equal(SimulatorCommandSink.Instance, client.CommandSink);
    }

    [Fact]
    public void ForceFeedbackCommandClient_CtorTest()
    {
        var client = new ForceFeedbackCommandClient();

        Assert.Equal(SimulatorCommandSink.Instance, client.CommandSink);
    }

    [Fact]
    public void PitServiceCommandClient_CtorTest()
    {
        var client = new PitServiceCommandClient();

        Assert.Equal(SimulatorCommandSink.Instance, client.CommandSink);
    }

    [Fact]
    public void ReplayCommandClient_CtorTest()
    {
        var client = new ReplayCommandClient();

        Assert.Equal(SimulatorCommandSink.Instance, client.CommandSink);
    }

    [Fact]
    public void TelemetryCommandClient_CtorTest()
    {
        var client = new TelemetryCommandClient();

        Assert.Equal(SimulatorCommandSink.Instance, client.CommandSink);
    }

    [Fact]
    public void TexturesCommandClient_CtorTest()
    {
        var client = new TexturesCommandClient();

        Assert.Equal(SimulatorCommandSink.Instance, client.CommandSink);
    }

    [Fact]
    public void VideoCaptureCommandClient_CtorTest()
    {
        var client = new VideoCaptureCommandClient();

        Assert.Equal(SimulatorCommandSink.Instance, client.CommandSink);
    }
}
