namespace SharpRacer.Telemetry;

public class TelemetryVariableInitializationExceptionTests
{
    [Fact]
    public void Ctor_MessageInnerExceptionTest()
    {
        var exMessage = "Unable to initialize variable.";

        var innerException = new InvalidOperationException("You can't do that!");

        var ex = new TelemetryVariableInitializationException(exMessage, innerException);

        Assert.Equal(exMessage, ex.Message);
        Assert.Equal(innerException, ex.InnerException);
    }

    [Fact]
    public void ThrowIfVariableInfoIsNotScalar_ArrayTest()
    {
        var variableInfo = TelemetryVariableInfoFactory.CreateArray("Bar", TelemetryVariableValueType.Int, 4);

        Assert.Throws<TelemetryVariableInitializationException>(() =>
            TelemetryVariableInitializationException.ThrowIfVariableInfoIsNotScalar(GetType(), variableInfo));
    }

    [Fact]
    public void ThrowIfVariableInfoIsNotScalar_ScalarTest()
    {
        var variableInfo = TelemetryVariableInfoFactory.CreateScalar("Foo", TelemetryVariableValueType.Int);

        TelemetryVariableInitializationException.ThrowIfVariableInfoIsNotScalar(GetType(), variableInfo);
    }

    [Fact]
    public void ThrowIfVariableInfoNameIsNotEqual_NameEqualsTest()
    {
        var variableInfo = TelemetryVariableInfoFactory.CreateScalar("Foo", TelemetryVariableValueType.Int);

        TelemetryVariableInitializationException.ThrowIfVariableInfoNameIsNotEqual(GetType(), variableInfo, "Foo");
    }

    [Fact]
    public void ThrowIfVariableInfoNameIsNotEqual_NameNotEqualsTest()
    {
        var variableInfo = TelemetryVariableInfoFactory.CreateScalar("Bar", TelemetryVariableValueType.Int);

        Assert.Throws<TelemetryVariableInitializationException>(() =>
            TelemetryVariableInitializationException.ThrowIfVariableInfoNameIsNotEqual(GetType(), variableInfo, "Foo"));
    }

    [Fact]
    public void ThrowIfVariableInfoValueCountIsNotEqual_ValueCountEqualTest()
    {
        var variableInfo = TelemetryVariableInfoFactory.CreateArray("Bar", TelemetryVariableValueType.Int, 4);

        TelemetryVariableInitializationException.ThrowIfVariableInfoValueCountIsNotEqual(GetType(), variableInfo, 4);
    }

    [Fact]
    public void ThrowIfVariableInfoValueCountIsNotEqual_ValueCountNotEqualTest()
    {
        var variableInfo = TelemetryVariableInfoFactory.CreateArray("Bar", TelemetryVariableValueType.Int, 4);

        Assert.Throws<TelemetryVariableInitializationException>(() =>
            TelemetryVariableInitializationException.ThrowIfVariableInfoValueCountIsNotEqual(GetType(), variableInfo, 1));
    }

    [Fact]
    public void ThrowIfValueTypeArgumentIsInvalid_ValueTypeIsValidTest()
    {
        TelemetryVariableInitializationException.ThrowIfValueTypeArgumentIsInvalid<int>(GetType(), TelemetryVariableValueType.Bitfield);
        TelemetryVariableInitializationException.ThrowIfValueTypeArgumentIsInvalid<uint>(GetType(), TelemetryVariableValueType.Bitfield);

        TelemetryVariableInitializationException.ThrowIfValueTypeArgumentIsInvalid<bool>(GetType(), TelemetryVariableValueType.Bool);
        TelemetryVariableInitializationException.ThrowIfValueTypeArgumentIsInvalid<byte>(GetType(), TelemetryVariableValueType.Byte);
        TelemetryVariableInitializationException.ThrowIfValueTypeArgumentIsInvalid<double>(GetType(), TelemetryVariableValueType.Double);
        TelemetryVariableInitializationException.ThrowIfValueTypeArgumentIsInvalid<float>(GetType(), TelemetryVariableValueType.Float);
        TelemetryVariableInitializationException.ThrowIfValueTypeArgumentIsInvalid<int>(GetType(), TelemetryVariableValueType.Int);
    }

    [Fact]
    public void ThrowIfValueTypeArgumentIsInvalid_ValueTypeNotValidTest()
    {
        Assert.Throws<TelemetryVariableInitializationException>(() =>
            TelemetryVariableInitializationException.ThrowIfValueTypeArgumentIsInvalid<float>(GetType(), TelemetryVariableValueType.Bool));
    }

    [Fact]
    public void ThrowIfValueTypeArgumentIsInvalid_ValueTypeWrongSizeForBitfieldTest()
    {
        Assert.Throws<TelemetryVariableInitializationException>(() =>
            TelemetryVariableInitializationException.ThrowIfValueTypeArgumentIsInvalid<short>(GetType(), TelemetryVariableValueType.Bitfield));

        Assert.Throws<TelemetryVariableInitializationException>(() =>
            TelemetryVariableInitializationException.ThrowIfValueTypeArgumentIsInvalid<long>(GetType(), TelemetryVariableValueType.Bitfield));
    }
}
