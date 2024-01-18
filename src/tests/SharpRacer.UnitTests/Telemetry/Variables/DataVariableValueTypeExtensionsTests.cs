namespace SharpRacer.Telemetry.Variables;

public class DataVariableValueTypeExtensionsTests
{
    [Fact]
    public void GetSize_InvalidValueTypeTest()
    {
        var valueType = (DataVariableValueType)499;

        Assert.Throws<InvalidOperationException>(() => valueType.GetSize());
    }

    [Fact]
    public void IsCompatibleValueTypeArgumentTest()
    {
        Assert.True(DataVariableValueType.Bitfield.IsCompatibleValueTypeArgument<int>());
        Assert.True(DataVariableValueType.Bool.IsCompatibleValueTypeArgument<bool>());
        Assert.True(DataVariableValueType.Byte.IsCompatibleValueTypeArgument<byte>());
        Assert.True(DataVariableValueType.Double.IsCompatibleValueTypeArgument<double>());
        Assert.True(DataVariableValueType.Float.IsCompatibleValueTypeArgument<float>());
        Assert.True(DataVariableValueType.Int.IsCompatibleValueTypeArgument<int>());

        Assert.False(DataVariableValueType.Bitfield.IsCompatibleValueTypeArgument<short>());
        Assert.False(DataVariableValueType.Bitfield.IsCompatibleValueTypeArgument<long>());

        Assert.False(DataVariableValueType.Byte.IsCompatibleValueTypeArgument<short>());
        Assert.False(DataVariableValueType.Byte.IsCompatibleValueTypeArgument<int>());
        Assert.False(DataVariableValueType.Byte.IsCompatibleValueTypeArgument<long>());

        Assert.False(DataVariableValueType.Double.IsCompatibleValueTypeArgument<float>());

        Assert.False(DataVariableValueType.Float.IsCompatibleValueTypeArgument<double>());
    }

    [Fact]
    public void IsCompatibleValueTypeArgumentTest_InvalidDataVariableValueTypeValueReturnsFalseTest()
    {
        var impossibleValue = (DataVariableValueType)9000;

        Assert.False(impossibleValue.IsCompatibleValueTypeArgument<double>());
    }
}
