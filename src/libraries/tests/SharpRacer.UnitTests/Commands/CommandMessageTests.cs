using SharpRacer.Extensions.Xunit;
using SharpRacer.Interop;

namespace SharpRacer.Commands;

public class CommandMessageTests
{
    public static TheoryData<CommandMessage, CommandMessage> InequalityTestData { get; } = GetInequalityTestData();

    [Fact]
    public void Ctor_CommandIdTest()
    {
        const ushort commandId = (ushort)SimulatorCommandId.PitService;

        var msg = new CommandMessage(commandId);

        Assert.Equal(commandId, msg.CommandId);
        Assert.Equal(0, msg.Arg1);
        Assert.Equal(0, msg.Arg2);
        Assert.Equal(0, msg.Arg3);
    }

    [Fact]
    public void Ctor_Arg1Test()
    {
        const ushort commandId = (ushort)SimulatorCommandId.PitService;
        const ushort arg1 = 12;

        var msg = new CommandMessage(commandId, arg1);

        Assert.Equal(commandId, msg.CommandId);
        Assert.Equal(arg1, msg.Arg1);
        Assert.Equal(0, msg.Arg2);
        Assert.Equal(0, msg.Arg3);
    }

    [Fact]
    public void Ctor_Arg2BoolTest()
    {
        const ushort commandId = (ushort)SimulatorCommandId.PitService;
        const ushort arg1 = 12;

        // arg2: true
        var msg = new CommandMessage(commandId, arg1, true);

        Assert.Equal(commandId, msg.CommandId);
        Assert.Equal(arg1, msg.Arg1);
        Assert.NotEqual(0, msg.Arg2);
        Assert.Equal(0, msg.Arg3);

        // arg2: false
        msg = new CommandMessage(commandId, arg1, false);

        Assert.Equal(commandId, msg.CommandId);
        Assert.Equal(arg1, msg.Arg1);
        Assert.Equal(0, msg.Arg2);
        Assert.Equal(0, msg.Arg3);
    }

    [Fact]
    public void Ctor_Arg2FloatTest()
    {
        const ushort commandId = (ushort)SimulatorCommandId.PitService;
        const ushort arg1 = 12;

        // 12345.0f * ScaleFactor = 809041920 = (0011 0000 0011 1001) (0000 0000 0000 0000)
        const float arg2F = 12345.0f;
        const int arg2Real = 809041920;

        const ushort expectedLower = 0;
        const ushort expectedUpper = 0b_0011_0000_0011_1001;

        var msg = new CommandMessage(commandId, arg1, arg2F);

        Assert.Equal(commandId, msg.CommandId);
        Assert.Equal(arg1, msg.Arg1);
        Assert.Equal(expectedUpper, msg.Arg2);
        Assert.Equal(expectedLower, msg.Arg3);

        var real = msg.Arg2 << 16 | msg.Arg3;

        Assert.Equal(arg2Real, real);
    }

    [Fact]
    public void Ctor_Arg2IntTest()
    {
        const ushort commandId = (ushort)SimulatorCommandId.PitService;
        const ushort arg1 = 12;

        // 1234567890 = (0100 1001 1001 0110) (0000 0010 1101 0010)
        const int arg2 = 1234567890;

        const ushort expectedLower = 0b_0000_0010_1101_0010;
        const ushort expectedUpper = 0b_0100_1001_1001_0110;

        var msg = new CommandMessage(commandId, arg1, arg2);

        Assert.Equal(commandId, msg.CommandId);
        Assert.Equal(arg1, msg.Arg1);
        Assert.Equal(expectedUpper, msg.Arg2);
        Assert.Equal(expectedLower, msg.Arg3);
    }

    [Fact]
    public void Ctor_Arg3Test()
    {
        const ushort commandId = (ushort)SimulatorCommandId.PitService;
        const ushort arg1 = 12;
        const ushort arg2 = 24;
        const ushort arg3 = 36;

        var msg = new CommandMessage(commandId, arg1, arg2, arg3);

        Assert.Equal(commandId, msg.CommandId);
        Assert.Equal(arg1, msg.Arg1);
        Assert.Equal(arg2, msg.Arg2);
        Assert.Equal(arg3, msg.Arg3);
    }

    [Fact]
    public void Ctor_SimulatorNotifyMessageDataTest()
    {
        const ushort commandId = (ushort)SimulatorCommandId.PitService;
        const ushort arg1 = 12;

        // 1234567890 = (0100 1001 1001 0110) (0000 0010 1101 0010)
        const int arg2 = 1234567890;

        const ushort expectedLower = 0b_0000_0010_1101_0010;
        const ushort expectedUpper = 0b_0100_1001_1001_0110;

        var notifyData = new SimulatorNotifyMessageData(
            wParam: (arg1 << 16) | commandId,
            lParam: arg2);

        var msg = new CommandMessage(notifyData);

        Assert.Equal(commandId, msg.CommandId);
        Assert.Equal(arg1, msg.Arg1);
        Assert.Equal(expectedUpper, msg.Arg2);
        Assert.Equal(expectedLower, msg.Arg3);
    }

    [Fact]
    public void ToNotifyMessageTest()
    {
        const ushort commandId = (ushort)SimulatorCommandId.PitService;
        const ushort arg1 = 12;
        const int arg2 = 1234567890;

        var expectedNotifyData = new SimulatorNotifyMessageData(
            wParam: (arg1 << 16) | commandId,
            lParam: arg2);

        var msg = new CommandMessage(commandId, arg1, arg2);

        var notifyMsg = msg.ToNotifyMessage();

        Assert.Equal(expectedNotifyData, notifyMsg);
    }

    [Fact]
    public void Equals_Test()
    {
        var command1 = new CommandMessage((ushort)SimulatorCommandId.CameraTargetDriver, 1, 2, 3);
        var command2 = new CommandMessage((ushort)SimulatorCommandId.CameraTargetDriver, 1, 2, 3);

        EquatableStructAssert.Equal(command1, command2);
    }

    [Theory]
    [MemberData(nameof(InequalityTestData))]
    public void Equals_InequalityTest(CommandMessage message1, CommandMessage message2)
    {
        EquatableStructAssert.NotEqual(message1, message2);
    }

    [Fact]
    public void Equals_ObjectInequalityTest()
    {
        var command1 = new CommandMessage((ushort)SimulatorCommandId.CameraTargetDriver, 123);
        var command2 = new object();

        EquatableStructAssert.ObjectEqualsMethod(false, command1, command2);
    }

    private static TheoryData<CommandMessage, CommandMessage> GetInequalityTestData()
    {
        var commandId1 = (ushort)SimulatorCommandId.CameraTargetDriver;
        var commandId2 = (ushort)SimulatorCommandId.CameraTargetRacePosition;

        return new TheoryData<CommandMessage, CommandMessage>()
        {
            // CommandId
            { new(commandId1, 1, 2, 3), new(commandId2, 1, 2, 3) },

            // Arg1
            { new(commandId1, 123, 2, 3), new(commandId1, 456, 2, 3) },

            // Arg2
            { new(commandId1, 1, 234, 3), new(commandId1, 1, 567, 3) },

            // Arg3
            { new(commandId1, 1, 2, 345), new(commandId1, 1, 2, 678) },
        };
    }
}
