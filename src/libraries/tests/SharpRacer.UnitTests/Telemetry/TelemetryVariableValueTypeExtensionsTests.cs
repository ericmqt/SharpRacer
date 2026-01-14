namespace SharpRacer.Telemetry;

public class TelemetryVariableValueTypeExtensionsTests
{
    [Fact]
    public void GetSize_InvalidValueTypeTest()
    {
        var valueType = (TelemetryVariableValueType)499;

        Assert.Throws<InvalidOperationException>(() => valueType.GetSize());
    }

    [Fact]
    public void IsCompatibleValueTypeArgumentTest()
    {
        Assert.True(TelemetryVariableValueType.Bitfield.IsCompatibleValueTypeArgument<int>());
        Assert.True(TelemetryVariableValueType.Bool.IsCompatibleValueTypeArgument<bool>());
        Assert.True(TelemetryVariableValueType.Byte.IsCompatibleValueTypeArgument<byte>());
        Assert.True(TelemetryVariableValueType.Double.IsCompatibleValueTypeArgument<double>());
        Assert.True(TelemetryVariableValueType.Float.IsCompatibleValueTypeArgument<float>());
        Assert.True(TelemetryVariableValueType.Int.IsCompatibleValueTypeArgument<int>());

        Assert.False(TelemetryVariableValueType.Bitfield.IsCompatibleValueTypeArgument<short>());
        Assert.False(TelemetryVariableValueType.Bitfield.IsCompatibleValueTypeArgument<long>());

        Assert.False(TelemetryVariableValueType.Byte.IsCompatibleValueTypeArgument<short>());
        Assert.False(TelemetryVariableValueType.Byte.IsCompatibleValueTypeArgument<int>());
        Assert.False(TelemetryVariableValueType.Byte.IsCompatibleValueTypeArgument<long>());

        Assert.False(TelemetryVariableValueType.Double.IsCompatibleValueTypeArgument<float>());

        Assert.False(TelemetryVariableValueType.Float.IsCompatibleValueTypeArgument<double>());
    }

    [Fact]
    public void IsCompatibleValueTypeArgumentTest_InvalidVariableValueTypeValueReturnsFalseTest()
    {
        var impossibleValue = (TelemetryVariableValueType)9000;

        Assert.False(impossibleValue.IsCompatibleValueTypeArgument<double>());
    }
}
