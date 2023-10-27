using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpRacer.Telemetry;
public class ArrayDataVariableTests
{
    [Fact]
    public void Ctor_Test()
    {
        var variableInfo = DataVariableInfoFactory.CreateArray("Foo", DataVariableValueType.Float, 8);

        var variable = new ArrayDataVariable<float>(variableInfo);

        Assert.Equal(variableInfo, variable.VariableInfo);
        Assert.True(variable.IsAvailable);
        Assert.False(variable.IsTimeSliceArray);
        Assert.Equal(8, variable.ArrayLength);
        Assert.Equal(Unsafe.SizeOf<float>() * 8, variable.DataLength);
    }

    [Fact]
    public void Ctor_ThrowsOnScalarVariableInfoTest()
    {
        var variableInfo = DataVariableInfoFactory.CreateScalar("Foo", DataVariableValueType.Int);

        Assert.Throws<ArgumentException>(() => new ArrayDataVariable<int>(variableInfo));
    }

    [Fact]
    public void Ctor_PlaceholderThrowsOnInvalidArrayLengthTest()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ArrayDataVariable<int>("Foo", arrayLength: 1, false));
        Assert.Throws<ArgumentOutOfRangeException>(() => new ArrayDataVariable<int>("Foo", arrayLength: -1, false));
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
        var variable = new ArrayDataVariable<float>("Bar", 4, false);

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
        var variable = new ArrayDataVariable<float>("Bar", 4, false);

        var data = new byte[1024];
        Assert.Throws<DataVariableUnavailableException>(() => variable.Read(data));
    }
}
