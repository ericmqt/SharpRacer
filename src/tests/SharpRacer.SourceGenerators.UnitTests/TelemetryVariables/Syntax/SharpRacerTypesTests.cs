namespace SharpRacer.SourceGenerators.TelemetryVariables.Syntax;
public class SharpRacerTypesTests
{
    [Theory]
    [InlineData(VariableValueType.Bitfield, "irsdk_CarLeftRight", "SharpRacer.Telemetry.CarLeftRight")]
    [InlineData(VariableValueType.Bool, null, "bool")]
    [InlineData(VariableValueType.Byte, null, "byte")]
    [InlineData(VariableValueType.Double, null, "double")]
    [InlineData(VariableValueType.Float, null, "float")]
    [InlineData(VariableValueType.Int, null, "int")]
    public void DataVariableTypeArgument_Test(VariableValueType valueType, string? unit, string expected)
    {
        var type = SharpRacerTypes.DataVariableTypeArgument(valueType, unit);

        Assert.Equal(expected, type.ToFullString());
    }

    [Fact]
    public void DataVariableTypeArgument_ThrowOnInvalidVariableValueTypeTest()
    {
        Assert.Throws<ArgumentException>(() => SharpRacerTypes.DataVariableTypeArgument((VariableValueType)999, null));
    }

    [Fact]
    public void Enumerations_CameraState_Test()
    {
        var type = SharpRacerTypes.Enumerations.CameraState();

        Assert.Equal("SharpRacer.Telemetry.CameraState", type.ToFullString());
    }

    [Fact]
    public void Enumerations_CarLeftRight_Test()
    {
        var type = SharpRacerTypes.Enumerations.CarLeftRight();

        Assert.Equal("SharpRacer.Telemetry.CarLeftRight", type.ToFullString());
    }

    [Fact]
    public void Enumerations_EngineWarnings_Test()
    {
        var type = SharpRacerTypes.Enumerations.EngineWarnings();

        Assert.Equal("SharpRacer.Telemetry.EngineWarnings", type.ToFullString());
    }

    [Fact]
    public void Enumerations_PaceMode_Test()
    {
        var type = SharpRacerTypes.Enumerations.PaceMode();

        Assert.Equal("SharpRacer.Telemetry.PaceMode", type.ToFullString());
    }

    [Fact]
    public void Enumerations_PaceRacingFlags_Test()
    {
        var type = SharpRacerTypes.Enumerations.PaceRacingFlags();

        Assert.Equal("SharpRacer.Telemetry.PaceRacingFlags", type.ToFullString());
    }

    [Fact]
    public void Enumerations_PitServiceOptions_Test()
    {
        var type = SharpRacerTypes.Enumerations.PitServiceOptions();

        Assert.Equal("SharpRacer.Telemetry.PitServiceOptions", type.ToFullString());
    }

    [Fact]
    public void Enumerations_PitServiceStatus_Test()
    {
        var type = SharpRacerTypes.Enumerations.PitServiceStatus();

        Assert.Equal("SharpRacer.Telemetry.PitServiceStatus", type.ToFullString());
    }

    [Fact]
    public void Enumerations_RacingFlags_Test()
    {
        var type = SharpRacerTypes.Enumerations.RacingFlags();

        Assert.Equal("SharpRacer.Telemetry.RacingFlags", type.ToFullString());
    }

    [Fact]
    public void Enumerations_SessionState_Test()
    {
        var type = SharpRacerTypes.Enumerations.SessionState();

        Assert.Equal("SharpRacer.Telemetry.SessionState", type.ToFullString());
    }

    [Fact]
    public void Enumerations_TrackLocationType_Test()
    {
        var type = SharpRacerTypes.Enumerations.TrackLocationType();

        Assert.Equal("SharpRacer.Telemetry.TrackLocationType", type.ToFullString());
    }

    [Fact]
    public void Enumerations_TrackSurfaceType_Test()
    {
        var type = SharpRacerTypes.Enumerations.TrackSurfaceType();

        Assert.Equal("SharpRacer.Telemetry.TrackSurfaceType", type.ToFullString());
    }
}
