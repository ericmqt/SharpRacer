namespace SharpRacer.Telemetry;
public class DataVariableInitializationExceptionTests
{
    [Fact]
    public void Ctor_MessageInnerExceptionTest()
    {
        var exMessage = "Unable to initialize variable.";

        var innerException = new InvalidOperationException("You can't do that!");

        var ex = new DataVariableInitializationException(exMessage, innerException);

        Assert.Equal(exMessage, ex.Message);
        Assert.Equal(innerException, ex.InnerException);
    }

    [Fact]
    public void ThrowIfDataVariableInfoIsNotScalar_ArrayTest()
    {
        var variableInfo = DataVariableInfoFactory.CreateArray("Bar", DataVariableValueType.Int, 4);

        Assert.Throws<DataVariableInitializationException>(() =>
            DataVariableInitializationException.ThrowIfDataVariableInfoIsNotScalar(GetType(), variableInfo));
    }

    [Fact]
    public void ThrowIfDataVariableInfoIsNotScalar_ScalarTest()
    {
        var variableInfo = DataVariableInfoFactory.CreateScalar("Foo", DataVariableValueType.Int);

        DataVariableInitializationException.ThrowIfDataVariableInfoIsNotScalar(GetType(), variableInfo);
    }

    [Fact]
    public void ThrowIfDataVariableInfoNameIsNotEqual_NameEqualsTest()
    {
        var variableInfo = DataVariableInfoFactory.CreateScalar("Foo", DataVariableValueType.Int);

        DataVariableInitializationException.ThrowIfDataVariableInfoNameIsNotEqual(GetType(), variableInfo, "Foo");
    }

    [Fact]
    public void ThrowIfDataVariableInfoNameIsNotEqual_NameNotEqualsTest()
    {
        var variableInfo = DataVariableInfoFactory.CreateScalar("Bar", DataVariableValueType.Int);

        Assert.Throws<DataVariableInitializationException>(() =>
            DataVariableInitializationException.ThrowIfDataVariableInfoNameIsNotEqual(GetType(), variableInfo, "Foo"));
    }

    [Fact]
    public void ThrowIfDataVariableInfoValueCountIsNotEqual_ValueCountEqualTest()
    {
        var variableInfo = DataVariableInfoFactory.CreateArray("Bar", DataVariableValueType.Int, 4);

        DataVariableInitializationException.ThrowIfDataVariableInfoValueCountIsNotEqual(GetType(), variableInfo, 4);
    }

    [Fact]
    public void ThrowIfDataVariableInfoValueCountIsNotEqual_ValueCountNotEqualTest()
    {
        var variableInfo = DataVariableInfoFactory.CreateArray("Bar", DataVariableValueType.Int, 4);

        Assert.Throws<DataVariableInitializationException>(() =>
            DataVariableInitializationException.ThrowIfDataVariableInfoValueCountIsNotEqual(GetType(), variableInfo, 1));
    }

    [Fact]
    public void ThrowIfValueTypeArgumentIsInvalid_ValueTypeIsValidTest()
    {
        DataVariableInitializationException.ThrowIfValueTypeArgumentIsInvalid<int>(GetType(), DataVariableValueType.Bitfield);
        DataVariableInitializationException.ThrowIfValueTypeArgumentIsInvalid<uint>(GetType(), DataVariableValueType.Bitfield);

        DataVariableInitializationException.ThrowIfValueTypeArgumentIsInvalid<bool>(GetType(), DataVariableValueType.Bool);
        DataVariableInitializationException.ThrowIfValueTypeArgumentIsInvalid<byte>(GetType(), DataVariableValueType.Byte);
        DataVariableInitializationException.ThrowIfValueTypeArgumentIsInvalid<double>(GetType(), DataVariableValueType.Double);
        DataVariableInitializationException.ThrowIfValueTypeArgumentIsInvalid<float>(GetType(), DataVariableValueType.Float);
        DataVariableInitializationException.ThrowIfValueTypeArgumentIsInvalid<int>(GetType(), DataVariableValueType.Int);
    }

    [Fact]
    public void ThrowIfValueTypeArgumentIsInvalid_ValueTypeNotValidTest()
    {
        Assert.Throws<DataVariableInitializationException>(() =>
            DataVariableInitializationException.ThrowIfValueTypeArgumentIsInvalid<float>(GetType(), DataVariableValueType.Bool));
    }

    [Fact]
    public void ThrowIfValueTypeArgumentIsInvalid_ValueTypeWrongSizeForBitfieldTest()
    {
        Assert.Throws<DataVariableInitializationException>(() =>
            DataVariableInitializationException.ThrowIfValueTypeArgumentIsInvalid<short>(GetType(), DataVariableValueType.Bitfield));

        Assert.Throws<DataVariableInitializationException>(() =>
            DataVariableInitializationException.ThrowIfValueTypeArgumentIsInvalid<long>(GetType(), DataVariableValueType.Bitfield));
    }
}
