using System.Runtime.InteropServices;
using SharpRacer.Telemetry.TestUtilities;

namespace SharpRacer.Telemetry;
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

        var variable = new ArrayDataVariable<float>(variableDescriptor, variableInfo: null);

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
    public void Ctor_NameLengthVariableInfo_Test()
    {
        const string variableName = "Foo";
        const DataVariableValueType valueType = DataVariableValueType.Float;
        const int valueCount = 3;

        var variableInfo = DataVariableInfoFactory.CreateArray(variableName, valueType, valueCount, false, 1024);

        var result = new ArrayDataVariable<float>(variableName, valueCount, variableInfo);

        Assert.Equal(variableName, result.Name);
        Assert.Equal(valueCount, result.ValueCount);
        Assert.Equal(variableInfo, result.VariableInfo);
        Assert.True(result.IsAvailable);
    }

    [Fact]
    public void Ctor_NameLengthVariableInfo_NullVariableInfoTest()
    {
        const string variableName = "Foo";
        const int valueCount = 3;

        var result = new ArrayDataVariable<float>(variableName, valueCount, variableInfo: null);

        Assert.Equal(variableName, result.Name);
        Assert.Equal(valueCount, result.ValueCount);
        Assert.Null(result.VariableInfo);
        Assert.False(result.IsAvailable);
    }

    [Fact]
    public void Ctor_NameLengthVariableInfo_ThrowsOnInvalidArrayLengthTest()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ArrayDataVariable<int>("Foo", arrayLength: 0, variableInfo: null));
        Assert.Throws<ArgumentOutOfRangeException>(() => new ArrayDataVariable<int>("Foo", arrayLength: -1, variableInfo: null));
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
        var variable = new ArrayDataVariable<float>("Bar", 4, variableInfo: null);

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
        var variable = new ArrayDataVariable<float>("Bar", 4, variableInfo: null);

        var data = new byte[1024];
        Assert.Throws<DataVariableUnavailableException>(() => variable.Read(data));
    }
}
