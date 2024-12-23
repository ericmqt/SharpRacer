﻿namespace SharpRacer.SourceGenerators.TelemetryVariables.Syntax;
public class SharpRacerTypesTests
{
    [Theory]
    [InlineData(TypeNameFormat.Default, "CarLeftRight")]
    [InlineData(TypeNameFormat.Qualified, "SharpRacer.CarLeftRight")]
    [InlineData(TypeNameFormat.GlobalQualified, "global::SharpRacer.CarLeftRight")]
    public void DataVariableTypeArgument_BitfieldTest(TypeNameFormat typeNameFormat, string expected)
    {
        var type = SharpRacerTypes.DataVariableTypeArgument(VariableValueType.Bitfield, "irsdk_CarLeftRight", typeNameFormat);

        Assert.Equal(expected, type.ToFullString());
    }

    [Theory]
    [InlineData(VariableValueType.Bool, "bool")]
    [InlineData(VariableValueType.Byte, "byte")]
    [InlineData(VariableValueType.Double, "double")]
    [InlineData(VariableValueType.Float, "float")]
    [InlineData(VariableValueType.Int, "int")]
    public void DataVariableTypeArgument_ScalarTest(VariableValueType valueType, string expected)
    {
        var type = SharpRacerTypes.DataVariableTypeArgument(valueType, null);

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
        Assert.Equal("CameraState", SharpRacerTypes.CameraState(TypeNameFormat.Default).ToFullString());
        Assert.Equal("SharpRacer.CameraState", SharpRacerTypes.CameraState(TypeNameFormat.Qualified).ToFullString());
        Assert.Equal("global::SharpRacer.CameraState", SharpRacerTypes.CameraState(TypeNameFormat.GlobalQualified).ToFullString());
    }

    [Fact]
    public void Enumerations_CarLeftRight_Test()
    {
        Assert.Equal("CarLeftRight", SharpRacerTypes.CarLeftRight(TypeNameFormat.Default).ToFullString());
        Assert.Equal("SharpRacer.CarLeftRight", SharpRacerTypes.CarLeftRight(TypeNameFormat.Qualified).ToFullString());
        Assert.Equal("global::SharpRacer.CarLeftRight", SharpRacerTypes.CarLeftRight(TypeNameFormat.GlobalQualified).ToFullString());
    }

    [Fact]
    public void Enumerations_EngineWarnings_Test()
    {
        Assert.Equal("EngineWarnings", SharpRacerTypes.EngineWarnings(TypeNameFormat.Default).ToFullString());
        Assert.Equal("SharpRacer.EngineWarnings", SharpRacerTypes.EngineWarnings(TypeNameFormat.Qualified).ToFullString());
        Assert.Equal("global::SharpRacer.EngineWarnings", SharpRacerTypes.EngineWarnings(TypeNameFormat.GlobalQualified).ToFullString());
    }

    [Fact]
    public void Enumerations_PaceMode_Test()
    {
        Assert.Equal("PaceMode", SharpRacerTypes.PaceMode(TypeNameFormat.Default).ToFullString());
        Assert.Equal("SharpRacer.PaceMode", SharpRacerTypes.PaceMode(TypeNameFormat.Qualified).ToFullString());
        Assert.Equal("global::SharpRacer.PaceMode", SharpRacerTypes.PaceMode(TypeNameFormat.GlobalQualified).ToFullString());
    }

    [Fact]
    public void Enumerations_PaceRacingFlags_Test()
    {
        Assert.Equal("PaceRacingFlags", SharpRacerTypes.PaceRacingFlags(TypeNameFormat.Default).ToFullString());
        Assert.Equal("SharpRacer.PaceRacingFlags", SharpRacerTypes.PaceRacingFlags(TypeNameFormat.Qualified).ToFullString());
        Assert.Equal("global::SharpRacer.PaceRacingFlags", SharpRacerTypes.PaceRacingFlags(TypeNameFormat.GlobalQualified).ToFullString());
    }

    [Fact]
    public void Enumerations_PitServiceOptions_Test()
    {
        Assert.Equal("PitServiceOptions", SharpRacerTypes.PitServiceOptions(TypeNameFormat.Default).ToFullString());
        Assert.Equal("SharpRacer.PitServiceOptions", SharpRacerTypes.PitServiceOptions(TypeNameFormat.Qualified).ToFullString());
        Assert.Equal("global::SharpRacer.PitServiceOptions", SharpRacerTypes.PitServiceOptions(TypeNameFormat.GlobalQualified).ToFullString());
    }

    [Fact]
    public void Enumerations_PitServiceStatus_Test()
    {
        Assert.Equal("PitServiceStatus", SharpRacerTypes.PitServiceStatus(TypeNameFormat.Default).ToFullString());
        Assert.Equal("SharpRacer.PitServiceStatus", SharpRacerTypes.PitServiceStatus(TypeNameFormat.Qualified).ToFullString());
        Assert.Equal("global::SharpRacer.PitServiceStatus", SharpRacerTypes.PitServiceStatus(TypeNameFormat.GlobalQualified).ToFullString());
    }

    [Fact]
    public void Enumerations_RacingFlags_Test()
    {
        Assert.Equal("RacingFlags", SharpRacerTypes.RacingFlags(TypeNameFormat.Default).ToFullString());
        Assert.Equal("SharpRacer.RacingFlags", SharpRacerTypes.RacingFlags(TypeNameFormat.Qualified).ToFullString());
        Assert.Equal("global::SharpRacer.RacingFlags", SharpRacerTypes.RacingFlags(TypeNameFormat.GlobalQualified).ToFullString());
    }

    [Fact]
    public void Enumerations_SessionState_Test()
    {
        Assert.Equal("SessionState", SharpRacerTypes.SessionState(TypeNameFormat.Default).ToFullString());
        Assert.Equal("SharpRacer.SessionState", SharpRacerTypes.SessionState(TypeNameFormat.Qualified).ToFullString());
        Assert.Equal("global::SharpRacer.SessionState", SharpRacerTypes.SessionState(TypeNameFormat.GlobalQualified).ToFullString());
    }

    [Fact]
    public void Enumerations_TrackLocationType_Test()
    {
        Assert.Equal("TrackLocationType", SharpRacerTypes.TrackLocationType(TypeNameFormat.Default).ToFullString());
        Assert.Equal("SharpRacer.TrackLocationType", SharpRacerTypes.TrackLocationType(TypeNameFormat.Qualified).ToFullString());
        Assert.Equal("global::SharpRacer.TrackLocationType", SharpRacerTypes.TrackLocationType(TypeNameFormat.GlobalQualified).ToFullString());
    }

    [Fact]
    public void Enumerations_TrackSurfaceType_Test()
    {
        Assert.Equal("TrackSurfaceType", SharpRacerTypes.TrackSurfaceType(TypeNameFormat.Default).ToFullString());
        Assert.Equal("SharpRacer.TrackSurfaceType", SharpRacerTypes.TrackSurfaceType(TypeNameFormat.Qualified).ToFullString());
        Assert.Equal("global::SharpRacer.TrackSurfaceType", SharpRacerTypes.TrackSurfaceType(TypeNameFormat.GlobalQualified).ToFullString());
    }

    [Fact]
    public void IDataVariablesContextInterfaceType_Test()
    {
        Assert.Equal(
            "IDataVariablesContext",
            SharpRacerTypes.IDataVariablesContextInterfaceType(TypeNameFormat.Default).ToFullString());
    }
}
