using SharpRacer.Extensions.Xunit.Utilities;

namespace SharpRacer.Commands.Camera;

public class SetCameraStateCommandTests : CommandUnitTests<SetCameraStateCommand, SetCameraStateCommandTests>, ICommandUnitTests<SetCameraStateCommand>
{
    public static CameraState AllCameraStateFlags = CameraState.IsSessionScreenActive
            | CameraState.IsScenicCameraActive
            | CameraState.IsCameraToolActive
            | CameraState.IsUIHidden
            | CameraState.UseAutoShotSelection
            | CameraState.UseTemporaryEdits
            | CameraState.UseKeyAcceleration
            | CameraState.UseKey10xAcceleration
            | CameraState.UseMouseAimMode;

    public static SimulatorCommandId CommandId { get; } = SimulatorCommandId.CameraSetState;

    [Fact]
    public void Ctor_Test()
    {
        var cameraState = CameraState.IsScenicCameraActive;

        var cmd = new SetCameraStateCommand(cameraState);

        Assert.Equal(cameraState, cmd.State);
    }

    [Fact]
    public void Ctor_DefaultTest()
    {
        var cmd = default(SetCameraStateCommand);

        Assert.Equal(CameraState.None, cmd.State);
    }

    [Fact]
    public void ToCommandMessage_Test()
    {
        var cameraState = CameraState.IsScenicCameraActive;

        var msg = new SetCameraStateCommand(cameraState).ToCommandMessage();

        CommandMessageAssert.Arg1Equals(cameraState, msg);
        CommandMessageAssert.Arg2Empty(msg);
        CommandMessageAssert.Arg3Empty(msg);
    }

    [Fact]
    public void Parse_ValidUndefinedCameraStateFlagCombinationTest()
    {
        var undefinedValidCameraStateFlagCombinations = EnumTestUtilities.GetFlagCombinations<CameraState, ushort>(
            excludeZeroValue: true, excludeDefinedFlagValues: true);

        foreach (var cameraStateFlag in undefinedValidCameraStateFlagCombinations)
        {
            var cmd = new SetCameraStateCommand(cameraStateFlag);
            var msg = new CommandMessage(SetCameraStateCommand.CommandId, (ushort)cameraStateFlag);

            var parsedCommand = SetCameraStateCommand.Parse(msg);

            Assert.Equal(cmd, parsedCommand);
            Assert.Equal(msg, (CommandMessage)parsedCommand);
        }
    }

    [Fact]
    public void TryParse_ValidUndefinedCameraStateFlagCombinationTest()
    {
        var undefinedValidCameraStateFlagCombinations = EnumTestUtilities.GetFlagCombinations<CameraState, ushort>(
            excludeZeroValue: true, excludeDefinedFlagValues: true);

        foreach (var cameraStateFlag in undefinedValidCameraStateFlagCombinations)
        {
            var cmd = new SetCameraStateCommand(cameraStateFlag);
            var msg = new CommandMessage(SetCameraStateCommand.CommandId, (ushort)cameraStateFlag);

            Assert.True(SetCameraStateCommand.TryParse(msg, out var result));

            Assert.Equal(cmd, result);
            Assert.Equal(msg, (CommandMessage)result);
        }
    }

    public static IEnumerable<(SetCameraStateCommand Command1, SetCameraStateCommand Command2)> EnumerateEqualityValues()
    {
        yield return (new(CameraState.None), new(CameraState.None));
        yield return (new(CameraState.IsScenicCameraActive), new(CameraState.IsScenicCameraActive));
        yield return (new(AllCameraStateFlags), new(AllCameraStateFlags));
        yield return (default, default);
    }

    public static IEnumerable<(SetCameraStateCommand Command1, SetCameraStateCommand Command2)> EnumerateInequalityValues()
    {
        yield return (new(CameraState.IsScenicCameraActive), new(CameraState.IsCameraToolActive));
        yield return (new(CameraState.IsSessionScreenActive), new(AllCameraStateFlags));
        yield return (new(CameraState.UseAutoShotSelection), default);
    }

    public static IEnumerable<SetCameraStateCommand> EnumerateValidCommands()
    {
        yield return new SetCameraStateCommand(CameraState.None);
        yield return new SetCameraStateCommand(CameraState.UseAutoShotSelection);
        yield return new SetCameraStateCommand(CameraState.IsSessionScreenActive);
        yield return new SetCameraStateCommand(AllCameraStateFlags);
    }
}
