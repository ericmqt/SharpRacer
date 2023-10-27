namespace SharpRacer.Telemetry;
public class DataVariableValueTypeExtensionsTests
{
    [Fact]
    public void GetSize_InvalidValueTypeTest()
    {
        var valueType = (DataVariableValueType)499;

        Assert.Throws<InvalidOperationException>(() => valueType.GetSize());
    }
}
