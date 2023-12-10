using System.Runtime.InteropServices;

namespace SharpRacer.Telemetry.Variables;
public class ArrayDataVariableTests
{
    [Fact]
    public void Ctor_VariableInfo_Test()
    {
        var variableInfo = DataVariableInfoFactory.CreateArray("Foo", DataVariableValueType.Float, 8);

        var variable = new ArrayDataVariable<float>(variableInfo);

        DataVariableAssert.IsAvailable(variable);
        DataVariableAssert.MatchesVariableInfo(variable, variableInfo);
    }

    [Fact]
    public void Ctor_Descriptor_AvailableVariableTest()
    {
        var variableInfo = DataVariableInfoFactory.CreateArray("Foo", DataVariableValueType.Float, 8);
        var variableDescriptor = new DataVariableDescriptor(variableInfo.Name, variableInfo.ValueType, variableInfo.ValueCount);

        var variable = new ArrayDataVariable<float>(variableDescriptor, variableInfo);

        DataVariableAssert.IsAvailable(variable);
        DataVariableAssert.MatchesVariableInfo(variable, variableInfo);
    }

    [Fact]
    public void Ctor_Descriptor_UnavailableVariableTest()
    {
        var variableInfo = DataVariableInfoFactory.CreateArray("Foo", DataVariableValueType.Float, 8);
        var variableDescriptor = new DataVariableDescriptor(variableInfo.Name, variableInfo.ValueType, variableInfo.ValueCount);

        var variable = new ArrayDataVariable<float>(variableDescriptor, null);

        DataVariableAssert.IsUnavailable(variable);
        DataVariableAssert.MatchesVariableDescriptor(variable, variableDescriptor);
    }

    [Fact]
    public void Ctor_Descriptor_ThrowOnNullDescriptorTest()
    {
        var variableInfo = DataVariableInfoFactory.CreateArray("Foo", DataVariableValueType.Float, 8);
        DataVariableDescriptor variableDescriptor = null!;

        Assert.Throws<ArgumentNullException>(() => new ArrayDataVariable<float>(variableDescriptor, variableInfo));
    }

    [Fact]
    public void Ctor_PlaceholderThrowsOnInvalidArrayLengthTest()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ArrayDataVariable<int>("Foo", arrayLength: 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new ArrayDataVariable<int>("Foo", arrayLength: -1));
    }

    [Fact]
    public void GetDataSpan_Test()
    {
        var variableInfo = DataVariableInfoFactory.CreateArray("Foo", DataVariableValueType.Int, 3, false, 12);
        var variable = new ArrayDataVariable<int>(variableInfo);

        // Build a data frame
        Span<byte> dataFrame = new byte[128];

        var variableValues = new int[] { 2, 3, 4 };
        Span<byte> variableValueBytes = MemoryMarshal.AsBytes<int>(variableValues);

        variableValueBytes.CopyTo(dataFrame.Slice(variableInfo.Offset));

        var dataSpan = variable.GetDataSpan(dataFrame);

        Assert.Equal(variableValueBytes.Length, dataSpan.Length);
        Assert.True(dataSpan.SequenceEqual(variableValueBytes));
    }

    [Fact]
    public void GetDataSpan_ThrowsOnUnavailableTest()
    {
        var variable = new ArrayDataVariable<float>("Bar", 4);

        var data = new byte[512];

        Assert.Throws<DataVariableUnavailableException>(() => variable.GetDataSpan(data));
    }

    [Fact]
    public void Read_Test()
    {
        var variableInfo = DataVariableInfoFactory.CreateArray("Foo", DataVariableValueType.Int, 3, false, 12);
        var variable = new ArrayDataVariable<int>(variableInfo);

        var variableValues = new int[] { 2, 3, 4 };

        Span<byte> dataFrame = new byte[128];

        MemoryMarshal.AsBytes<int>(variableValues).CopyTo(dataFrame.Slice(variableInfo.Offset));

        var readValues = variable.Read(dataFrame);

        Assert.True(readValues.SequenceEqual(variableValues));
    }

    [Fact]
    public void Read_ThrowsOnUnavailableTest()
    {
        var variable = new ArrayDataVariable<float>("Bar", 4);

        var data = new byte[1024];
        Assert.Throws<DataVariableUnavailableException>(() => variable.Read(data));
    }
}
