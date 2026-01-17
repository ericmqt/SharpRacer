using SharpRacer.Commands.Camera;
using SharpRacer.Commands.Chat;
using SharpRacer.Commands.Telemetry;
using SharpRacer.Extensions.Xunit.Utilities;

namespace SharpRacer.Commands;

public class CommandMessageReaderTests
{
    [Fact]
    public void ReadArg1_Test()
    {
        const ushort commandId = (ushort)SimulatorCommandId.PitService;
        const ushort arg1 = 12;

        var msg = new CommandMessage(commandId, arg1);
        var reader = new CommandMessageReader(ref msg);

        Assert.Equal(arg1, reader.ReadArg1());
    }

    [Theory]
    [InlineData(CameraTargetMode.Incident)]
    [InlineData(CameraTargetMode.Leader)]
    [InlineData(CameraTargetMode.Exiting)]
    [InlineData(CameraTargetMode.Driver)]
    public void ReadArg1_TEnum_ShortTest(CameraTargetMode targetMode)
    {
        const ushort commandId = (ushort)SimulatorCommandId.CameraTargetRacePosition;

        var msg = new CommandMessage(commandId, (ushort)targetMode);

        var reader = new CommandMessageReader(ref msg);

        Assert.Equal(targetMode, reader.ReadArg1<CameraTargetMode>());
    }

    [Theory]
    [InlineData(ChatCommandType.Macro)]
    [InlineData(ChatCommandType.OpenChat)]
    [InlineData(ChatCommandType.ReplyToLastPrivateMessage)]
    [InlineData(ChatCommandType.CloseChat)]
    public void ReadArg1_TEnum_UShortTest(ChatCommandType chatCommandType)
    {
        const ushort commandId = (ushort)SimulatorCommandId.Chat;

        var msg = new CommandMessage(commandId, (ushort)chatCommandType);

        var reader = new CommandMessageReader(ref msg);

        Assert.Equal(chatCommandType, reader.ReadArg1<ChatCommandType>());
    }

    [Fact]
    public void ReadArg1_TEnum_ThrowIfEnumValueNotDefinedTest()
    {
        const ushort commandId = (ushort)SimulatorCommandId.Telemetry;
        var invalidTelemetryCommandTypeVal = (ushort)(EnumTestUtilities.MaxIntegralValue<TelemetryCommandType, ushort>() + 1);

        var msg = new CommandMessage(commandId, invalidTelemetryCommandTypeVal);

        Assert.Throws<CommandMessageReaderException>(() => new CommandMessageReader(ref msg).ReadArg1<TelemetryCommandType>());
    }

    [Fact]
    public void ReadArg1Flags_Test()
    {
        const ushort commandId = (ushort)SimulatorCommandId.CameraSetState;

        var cameraState = CameraState.IsSessionScreenActive
            | CameraState.IsScenicCameraActive
            | CameraState.IsCameraToolActive
            | CameraState.IsUIHidden
            | CameraState.UseMouseAimMode;

        var msg = new CommandMessage(commandId, (ushort)cameraState);

        var reader = new CommandMessageReader(ref msg);
        var arg1Result = reader.ReadArg1Flags<CameraState>();

        Assert.Equal(cameraState, arg1Result);
    }

    [Fact]
    public void ReadArg1Flags_DoesNotThrowIfEnumValueNotDefinedTest()
    {
        const ushort commandId = (ushort)SimulatorCommandId.CameraSetState;
        var invalidCameraStateVal = (ushort)(EnumTestUtilities.FlagsMaxIntegralValue<CameraState, ushort>() + 1);

        var msg = new CommandMessage(commandId, invalidCameraStateVal);

        var reader = new CommandMessageReader(ref msg);
        var arg1Result = reader.ReadArg1Flags<CameraState>();

        Assert.Equal((CameraState)invalidCameraStateVal, arg1Result);
    }

    [Fact]
    public void ReadArg2_Test()
    {
        const ushort commandId = (ushort)SimulatorCommandId.PitService;
        const ushort arg1 = 12;
        const ushort arg2 = 24;
        const ushort arg3 = 36;

        var msg = new CommandMessage(commandId, arg1, arg2, arg3);
        var reader = new CommandMessageReader(ref msg);

        Assert.Equal(arg2, reader.ReadArg2());
    }

    [Fact]
    public void ReadArg2Bool_Test()
    {
        const ushort commandId = (ushort)SimulatorCommandId.PitService;
        const ushort arg1 = 12;

        // arg2: true
        var msg = new CommandMessage(commandId, arg1, true);

        var reader = new CommandMessageReader(ref msg);
        Assert.True(reader.ReadArg2Bool());

        // arg2: false
        msg = new CommandMessage(commandId, arg1, false);

        reader = new CommandMessageReader(ref msg);
        Assert.False(reader.ReadArg2Bool());
    }

    [Fact]
    public void ReadArg2Float_Test()
    {
        const ushort commandId = (ushort)SimulatorCommandId.PitService;
        const ushort arg1 = 12;
        const float arg2F = 12345.0f;

        var msg = new CommandMessage(commandId, arg1, arg2F);

        var reader = new CommandMessageReader(ref msg);
        Assert.Equal(arg2F, reader.ReadArg2Float());
    }

    [Fact]
    public void ReadArg2Int_Test()
    {
        const ushort commandId = (ushort)SimulatorCommandId.PitService;
        const ushort arg1 = 12;
        const int arg2 = 1234567890;

        var msg = new CommandMessage(commandId, arg1, arg2);

        var reader = new CommandMessageReader(ref msg);
        Assert.Equal(arg2, reader.ReadArg2Int());
    }

    [Fact]
    public void ReadArg3_Test()
    {
        const ushort commandId = (ushort)SimulatorCommandId.PitService;
        const ushort arg1 = 12;
        const ushort arg2 = 24;
        const ushort arg3 = 36;

        var msg = new CommandMessage(commandId, arg1, arg2, arg3);
        var reader = new CommandMessageReader(ref msg);

        Assert.Equal(arg3, reader.ReadArg3());
    }

    [Fact]
    public void TryReadArg1_TEnum_Test()
    {
        const ushort commandId = (ushort)SimulatorCommandId.CameraTargetDriver;
        const CameraTargetMode arg1 = CameraTargetMode.Incident;
        const ushort arg2 = 24;
        const ushort arg3 = 36;

        var msg = new CommandMessage(commandId, unchecked((ushort)arg1), arg2, arg3);
        var reader = new CommandMessageReader(ref msg);

        Assert.True(reader.TryReadArg1<CameraTargetMode>(out var targetMode));
        Assert.Equal(arg1, targetMode);
    }

    [Fact]
    public void TryReadArg1_TEnum_ReturnsFalseOnUndefinedValueTest()
    {
        const ushort commandId = (ushort)SimulatorCommandId.CameraTargetDriver;
        const CameraTargetMode arg1 = (CameraTargetMode)32;
        const ushort arg2 = 24;
        const ushort arg3 = 36;

        Assert.False(Enum.IsDefined(typeof(CameraTargetMode), arg1));

        var msg = new CommandMessage(commandId, (ushort)arg1, arg2, arg3);
        var reader = new CommandMessageReader(ref msg);

        Assert.False(reader.TryReadArg1<CameraTargetMode>(out var targetMode));
    }

    [Fact]
    public void TryReadArg1Flags_Test()
    {
        const ushort commandId = (ushort)SimulatorCommandId.CameraSetState;

        var cameraState = CameraState.IsSessionScreenActive
            | CameraState.IsScenicCameraActive
            | CameraState.IsCameraToolActive
            | CameraState.IsUIHidden
            | CameraState.UseMouseAimMode;

        var msg = new CommandMessage(commandId, (ushort)cameraState);

        var reader = new CommandMessageReader(ref msg);

        Assert.True(reader.TryReadArg1Flags<CameraState>(out var cameraStateResult));
        Assert.Equal(cameraState, cameraStateResult);
    }

    [Fact]
    public void TryReadArg1Flags_ReturnTrueIfEnumValueNotDefinedTest()
    {
        const ushort commandId = (ushort)SimulatorCommandId.CameraSetState;
        var invalidCameraStateVal = (ushort)(EnumTestUtilities.FlagsMaxIntegralValue<CameraState, ushort>() + 1);

        var msg = new CommandMessage(commandId, invalidCameraStateVal);

        var reader = new CommandMessageReader(ref msg);

        Assert.True(reader.TryReadArg1Flags<CameraState>(out var cameraStateResult));
        Assert.Equal(invalidCameraStateVal, (ushort)cameraStateResult);
    }
}
