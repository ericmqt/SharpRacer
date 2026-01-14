using Moq;
using SharpRacer.Commands.Interop;

namespace SharpRacer.Commands.PitService;

public class PitServiceCommandClientTests : CommandClientUnitTests
{
    [Fact]
    public void Ctor_CommandSinkTest()
    {
        var commandSinkMock = Mocks.Create<ISimulatorCommandSink>();

        var client = new PitServiceCommandClient(commandSinkMock.Object);

        Assert.Equal(commandSinkMock.Object, client.CommandSink);
    }

    [Fact]
    public void Ctor_ThrowIfCommandSinkIsNullTest()
    {
        Assert.Throws<ArgumentNullException>(() => new PitServiceCommandClient(null!));
    }

    [Fact]
    public void Clear_Test()
    {
        var clearedService = PitServiceResetType.FastRepair;
        var command = new ResetPitServiceCommand(clearedService);
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new PitServiceCommandClient(CommandSink);
        client.Clear(clearedService);

        Mocks.Verify();
    }

    [Fact]
    public void ClearAll_Test()
    {
        var clearedService = PitServiceResetType.All;
        var command = new ResetPitServiceCommand(clearedService);
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new PitServiceCommandClient(CommandSink);
        client.ClearAll();

        Mocks.Verify();
    }

    [Fact]
    public void RequestFastRepair_Test()
    {
        var command = new UseFastRepairCommand();
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new PitServiceCommandClient(CommandSink);
        client.RequestFastRepair();

        Mocks.Verify();
    }

    [Fact]
    public void RequestFuel_Test()
    {
        var command = new AddFuelCommand();
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new PitServiceCommandClient(CommandSink);
        client.RequestFuel();

        Mocks.Verify();
    }

    [Fact]
    public void RequestFuel_FuelQuantityTest()
    {
        int fuelQuantity = 6;
        var command = new AddFuelCommand(fuelQuantity);
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new PitServiceCommandClient(CommandSink);
        client.RequestFuel(fuelQuantity);

        Mocks.Verify();
    }

    [Fact]
    public void RequestTireChange_Test()
    {
        var tireTarget = TireChangeTarget.RightRear;

        var command = new ChangeTireCommand(tireTarget);
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new PitServiceCommandClient(CommandSink);
        client.RequestTireChange(tireTarget);

        Mocks.Verify();
    }

    [Fact]
    public void RequestTireChange_PressureTest()
    {
        var tireTarget = TireChangeTarget.RightRear;
        int tirePressure = 40;

        var command = new ChangeTireCommand(tireTarget, tirePressure);
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new PitServiceCommandClient(CommandSink);
        client.RequestTireChange(tireTarget, tirePressure);

        Mocks.Verify();
    }

    [Fact]
    public void RequestWindscreenTearOff_Test()
    {
        var command = new UseWindscreenTearOffCommand();
        var commandMessage = command.ToCommandMessage();
        var notifyMessage = commandMessage.ToNotifyMessage();

        ConfigureExpectedCommand(command, commandMessage, notifyMessage, Times.Once());

        var client = new PitServiceCommandClient(CommandSink);
        client.RequestWindscreenTearOff();

        Mocks.Verify();
    }
}
