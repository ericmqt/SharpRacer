using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpRacer.Telemetry;
public class DataVariableTests
{
    [Fact]
    public void Ctor_Test()
    {
        var variableInfo = DataVariableInfoFactory.CreateScalar("Foo", DataVariableValueType.Int);
        var variable = new DataVariable<int>(variableInfo);

        Assert.Equal(variableInfo.Name, variable.Name);
        Assert.True(variable.IsAvailable);
        Assert.Equal(variableInfo, variable.VariableInfo);
        Assert.Equal(variableInfo.Offset, variable.DataOffset);
    }

    [Fact]
    public void Ctor_ThrowsOnNullVariableInfoTest()
    {
        Assert.Throws<ArgumentNullException>(() => new DataVariable<float>(dataVariableInfo: null!));
    }

    [Fact]
    public void Ctor_PlaceholderInstanceTest()
    {
        var variable = new DataVariable<int>("Foo");

        Assert.Equal("Foo", variable.Name);
        Assert.False(variable.IsAvailable);
        Assert.Null(variable.VariableInfo);

        Assert.Equal(Unsafe.SizeOf<int>(), variable.DataLength);
        Assert.Equal(-1, variable.DataOffset);
    }

    [Fact]
    public void Ctor_PlaceholderInstance_NullOrEmptyNameTest()
    {
        Assert.Throws<ArgumentException>(() => new DataVariable<float>(name: null!));
        Assert.Throws<ArgumentException>(() => new DataVariable<float>(string.Empty));
    }

    [Fact]
    public void GetDataSpan_Test()
    {
        var variableInfo = DataVariableInfoFactory.CreateScalar("Foo", DataVariableValueType.Int, offset: 8);
        var variable = new DataVariable<int>(variableInfo);

        Span<byte> dataFrame = new byte[256];
        int variableValue = 37;
        var variableValueSlice = dataFrame.Slice(variableInfo.Offset, Unsafe.SizeOf<int>());

        MemoryMarshal.Write(variableValueSlice, variableValue);

        var dataSpan = variable.GetDataSpan(dataFrame);

        Assert.Equal(variableValueSlice.Length, dataSpan.Length);
        Assert.True(dataSpan.SequenceEqual(variableValueSlice));
    }

    [Fact]
    public void GetDataSpan_ThrowsOnUnavailableTest()
    {
        var variable = new DataVariable<float>("Bar");

        var data = new byte[512];

        Assert.Throws<DataVariableUnavailableException>(() => variable.GetDataSpan(data));
    }

    [Fact]
    public void Read_Test()
    {
        var variableInfo = DataVariableInfoFactory.CreateScalar("Foo", DataVariableValueType.Int, offset: 1020);
        var variable = new DataVariable<int>(variableInfo);

        Span<byte> dataFrame = new byte[2048];
        int variableValue = 37;
        MemoryMarshal.Write(dataFrame.Slice(variableInfo.Offset, Unsafe.SizeOf<int>()), variableValue);

        Assert.Equal(variableValue, variable.Read(dataFrame));
    }

    [Fact]
    public void Read_ThrowsOnUnavailableTest()
    {
        var variable = new DataVariable<float>("Bar");

        var data = new byte[1024];
        Assert.Throws<DataVariableUnavailableException>(() => variable.Read(data));
    }
}
