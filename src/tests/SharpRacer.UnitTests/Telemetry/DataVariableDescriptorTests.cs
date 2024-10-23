using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpRacer.Telemetry;
public class DataVariableDescriptorTests
{
    [Fact]
    public void Ctor_Test()
    {
        const string variableName = "Foo";
        const DataVariableValueType valueType = DataVariableValueType.Float;
        const int valueCount = 3;

        var desc = new DataVariableDescriptor(variableName, valueType, valueCount);

        Assert.Equal(variableName, desc.Name);
        Assert.Equal(valueType, desc.ValueType);
        Assert.Equal(valueCount, desc.ValueCount);
    }

    [Fact]
    public void Ctor_ThrowOnNullOrEmptyNameTest()
    {
        Assert.Throws<ArgumentNullException>(() => new DataVariableDescriptor(null!, DataVariableValueType.Int, 1));
        Assert.Throws<ArgumentException>(() => new DataVariableDescriptor(string.Empty, DataVariableValueType.Int, 1));
    }

    [Fact]
    public void Ctor_ThrowOnNegativeOrZeroValueCountTest()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new DataVariableDescriptor("Foo", DataVariableValueType.Int, valueCount: 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new DataVariableDescriptor("Foo", DataVariableValueType.Int, valueCount: -1));
    }
}
