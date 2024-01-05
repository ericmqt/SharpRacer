namespace SharpRacer.SourceGenerators.TelemetryVariables.Syntax;
public class VariableValueTypesTests
{
    [Fact]
    public void Enumeration_ThrowOnNullOrEmptyVariableUnitTest()
    {
        Assert.Throws<ArgumentException>(() => VariableValueTypes.Enumeration(null!));
        Assert.Throws<ArgumentException>(() => VariableValueTypes.Enumeration(string.Empty));
    }

    [Fact]
    public void Enumeration_ReturnsNullOnInvalidVariableUnitTest()
    {
        Assert.Null(VariableValueTypes.Enumeration("hello"));
    }

    [Theory]
    [InlineData("irsdk_CameraState", "SharpRacer.Telemetry.CameraState")]
    [InlineData("irsdk_CarLeftRight", "SharpRacer.Telemetry.CarLeftRight")]
    [InlineData("irsdk_EngineWarnings", "SharpRacer.Telemetry.EngineWarnings")]
    [InlineData("irsdk_Flags", "SharpRacer.Telemetry.RacingFlags")]
    [InlineData("irsdk_PaceFlags", "SharpRacer.Telemetry.PaceRacingFlags")]
    [InlineData("irsdk_PaceMode", "SharpRacer.Telemetry.PaceMode")]
    [InlineData("irsdk_PitSvFlags", "SharpRacer.Telemetry.PitServiceOptions")]
    [InlineData("irsdk_PitSvStatus", "SharpRacer.Telemetry.PitServiceStatus")]
    [InlineData("irsdk_SessionState", "SharpRacer.Telemetry.SessionState")]
    [InlineData("irsdk_TrkLoc", "SharpRacer.Telemetry.TrackLocationType")]
    [InlineData("irsdk_TrkSurf", "SharpRacer.Telemetry.TrackSurfaceType")]
    public void Enumeration_Test(string variableUnit, string expected)
    {
        var enumType = VariableValueTypes.Enumeration(variableUnit, TypeNameFormat.Qualified);

        Assert.NotNull(enumType);
        Assert.Equal(expected, enumType.ToFullString());
    }

    [Theory]
    [InlineData("irsdk_CameraState", "SharpRacer.Telemetry.CameraState")]
    [InlineData("irsdk_CarLeftRight", "SharpRacer.Telemetry.CarLeftRight")]
    [InlineData("irsdk_EngineWarnings", "SharpRacer.Telemetry.EngineWarnings")]
    [InlineData("irsdk_Flags", "SharpRacer.Telemetry.RacingFlags")]
    [InlineData("irsdk_PaceFlags", "SharpRacer.Telemetry.PaceRacingFlags")]
    [InlineData("irsdk_PaceMode", "SharpRacer.Telemetry.PaceMode")]
    [InlineData("irsdk_PitSvFlags", "SharpRacer.Telemetry.PitServiceOptions")]
    [InlineData("irsdk_PitSvStatus", "SharpRacer.Telemetry.PitServiceStatus")]
    [InlineData("irsdk_SessionState", "SharpRacer.Telemetry.SessionState")]
    [InlineData("irsdk_TrkLoc", "SharpRacer.Telemetry.TrackLocationType")]
    [InlineData("irsdk_TrkSurf", "SharpRacer.Telemetry.TrackSurfaceType")]
    [InlineData("FooBarNotARealEnumName", "int")]
    public void EnumerationOrInt_Test(string variableUnit, string expected)
    {
        var enumType = VariableValueTypes.EnumerationOrInt(variableUnit, TypeNameFormat.Qualified);

        Assert.NotNull(enumType);
        Assert.Equal(expected, enumType.ToFullString());
    }

    [Fact]
    public void Bool_Test()
    {
        var type = VariableValueTypes.Bool();

        Assert.Equal("bool", type.ToFullString());
    }

    [Fact]
    public void Byte_Test()
    {
        var type = VariableValueTypes.Byte();

        Assert.Equal("byte", type.ToFullString());
    }

    [Fact]
    public void Double_Test()
    {
        var type = VariableValueTypes.Double();

        Assert.Equal("double", type.ToFullString());
    }

    [Fact]
    public void Float_Test()
    {
        var type = VariableValueTypes.Float();

        Assert.Equal("float", type.ToFullString());
    }

    [Fact]
    public void Int_Test()
    {
        var type = VariableValueTypes.Int();

        Assert.Equal("int", type.ToFullString());
    }
}
