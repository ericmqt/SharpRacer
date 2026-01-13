using Moq;
using SharpRacer.Commands.Interop;

namespace SharpRacer.Commands.Replay;

public class ReplayCommandClientTests : CommandClientUnitTests
{
    [Fact]
    public void Ctor_CommandSinkTest()
    {
        var commandSinkMock = Mocks.Create<ISimulatorCommandSink>();

        var client = new ReplayCommandClient(commandSinkMock.Object);

        Assert.Equal(commandSinkMock.Object, client.CommandSink);
    }

    [Fact]
    public void Ctor_ThrowIfCommandSinkIsNullTest()
    {
        Assert.Throws<ArgumentNullException>(() => new ReplayCommandClient(null!));
    }

    [Fact]
    public void EraseTape_Test()
    {
        var command = new EraseReplayTapeCommand();
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new ReplayCommandClient(CommandSink);
        client.EraseTape();

        Mocks.Verify();
    }

    [Fact]
    public void Pause_Test()
    {
        const ushort playSpeed = 0;
        const bool isSlowMotion = false;

        var command = new SetPlaySpeedCommand(playSpeed, isSlowMotion);
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new ReplayCommandClient(CommandSink);
        client.Pause();

        Mocks.Verify();
    }

    [Fact]
    public void Play_Test()
    {
        const ushort playSpeed = 1;
        const bool isSlowMotion = false;

        var command = new SetPlaySpeedCommand(playSpeed, isSlowMotion);
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new ReplayCommandClient(CommandSink);
        client.Play();

        Mocks.Verify();
    }

    [Fact]
    public void Search_Test()
    {
        const ReplaySearchMode searchMode = ReplaySearchMode.NextLap;

        var command = new SearchCommand(searchMode);
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new ReplayCommandClient(CommandSink);
        client.Search(searchMode);

        Mocks.Verify();
    }

    [Fact]
    public void SearchSessionTime_Test()
    {
        const ushort sessionNumber = 4;
        const int sessionTimeMs = 123456;

        var command = new SearchSessionTimeCommand(sessionNumber, sessionTimeMs);
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new ReplayCommandClient(CommandSink);
        client.SearchSessionTime(sessionNumber, sessionTimeMs);

        Mocks.Verify();
    }

    [Fact]
    public void SeekFrame_Test()
    {
        const int frame = 123456;
        const ReplaySeekOrigin seekOrigin = ReplaySeekOrigin.Current;

        var command = new SeekFrameCommand(frame, seekOrigin);
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new ReplayCommandClient(CommandSink);
        client.SeekFrame(frame, seekOrigin);

        Mocks.Verify();
    }

    [Fact]
    public void SetPlaySpeed_Test()
    {
        const ushort playSpeed = 4;
        const bool isSlowMotion = false;

        var command = new SetPlaySpeedCommand(playSpeed, isSlowMotion);
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new ReplayCommandClient(CommandSink);
        client.SetPlaySpeed(playSpeed, isSlowMotion);

        Mocks.Verify();
    }

    [Fact]
    public void SetPlaySpeed_SlowMotionTest()
    {
        const ushort playSpeed = 4;
        const bool isSlowMotion = true;

        var command = new SetPlaySpeedCommand(playSpeed, isSlowMotion);
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new ReplayCommandClient(CommandSink);
        client.SetPlaySpeed(playSpeed, isSlowMotion);

        Mocks.Verify();
    }
}
