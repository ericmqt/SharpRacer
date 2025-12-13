using System.Reflection;
using System.Runtime.Intrinsics;

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

    [Fact]
    public void CreateArray_Test()
    {
        const string variableName = "Foo";
        const int valueCount = 3;
        const DataVariableValueType valueType = DataVariableValueType.Float;

        var descriptor = DataVariableDescriptor.CreateArray(variableName, valueCount, valueType);

        Assert.Equal(variableName, descriptor.Name);
        Assert.Equal(valueCount, descriptor.ValueCount);
        Assert.Equal(valueType, descriptor.ValueType);
    }

    [Theory]
    [MemberData(nameof(CreateArrayGeneric_TestData))]
    public void CreateArrayGeneric_Test(string variableName, int valueCount, DataVariableValueType valueType, Type typeArg)
    {
        var createScalarMethod = typeof(DataVariableDescriptor)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(x => x.IsGenericMethod && x.Name == nameof(DataVariableDescriptor.CreateArray));

        Assert.NotNull(createScalarMethod);

        var genericMethodInfo = createScalarMethod.MakeGenericMethod([typeArg]);

        var resultObj = genericMethodInfo.Invoke(null, [variableName, valueCount]);
        Assert.NotNull(resultObj);

        var result = (DataVariableDescriptor)resultObj;

        Assert.Equal(variableName, result.Name);
        Assert.Equal(valueCount, result.ValueCount);
        Assert.Equal(valueType, result.ValueType);
    }

    [Fact]
    public void CreateScalar_Test()
    {
        const string variableName = "Foo";
        const int valueCount = 1;
        const DataVariableValueType valueType = DataVariableValueType.Float;

        var descriptor = DataVariableDescriptor.CreateScalar(variableName, valueType);

        Assert.Equal(variableName, descriptor.Name);
        Assert.Equal(valueCount, descriptor.ValueCount);
        Assert.Equal(valueType, descriptor.ValueType);
    }

    [Theory]
    [MemberData(nameof(CreateScalarGeneric_TestData))]
    public void CreateScalarGeneric_Test(string variableName, int valueCount, DataVariableValueType valueType, Type typeArg)
    {
        var createScalarMethod = typeof(DataVariableDescriptor)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(x => x.IsGenericMethod && x.Name == nameof(DataVariableDescriptor.CreateScalar));

        Assert.NotNull(createScalarMethod);

        var genericMethodInfo = createScalarMethod.MakeGenericMethod([typeArg]);

        var resultObj = genericMethodInfo.Invoke(null, [variableName]);
        Assert.NotNull(resultObj);

        var result = (DataVariableDescriptor)resultObj;

        Assert.Equal(variableName, result.Name);
        Assert.Equal(valueCount, result.ValueCount);
        Assert.Equal(valueType, result.ValueType);
    }

    [Fact]
    public void CreateScalarGeneric_BitfieldEnumWrongSizeValueTest()
    {
        Assert.Throws<ArgumentException>(() => DataVariableDescriptor.CreateScalar<SimpleByteEnum>("Foo"));
    }

    [Fact]
    public void CreateGenericMethods_InvalidTypeArgTest()
    {
        Assert.Throws<ArgumentException>(() => DataVariableDescriptor.CreateScalar<Vector128<float>>("Foo"));
    }

    private enum SimpleByteEnum : byte
    {
        None = 0,
        One = 1
    }

    private enum SimpleInt32Enum : int
    {
        None = 0,
        One = 1
    }

    public static TheoryData<string, int, DataVariableValueType, Type> CreateArrayGeneric_TestData()
    {
        return new TheoryData<string, int, DataVariableValueType, Type>()
        {
            { "Foo", 3, DataVariableValueType.Bitfield, typeof(SimpleInt32Enum) },
            { "Foo", 3, DataVariableValueType.Byte, typeof(byte) },
            { "Foo", 3, DataVariableValueType.Bool, typeof(bool) },
            { "Foo", 3, DataVariableValueType.Int, typeof(int) },
            { "Foo", 3, DataVariableValueType.Float, typeof(float) },
            { "Foo", 3, DataVariableValueType.Double, typeof(double) },
        };
    }

    public static TheoryData<string, int, DataVariableValueType, Type> CreateScalarGeneric_TestData()
    {
        return new TheoryData<string, int, DataVariableValueType, Type>()
        {
            { "Foo", 1, DataVariableValueType.Bitfield, typeof(SimpleInt32Enum) },
            { "Foo", 1, DataVariableValueType.Byte, typeof(byte) },
            { "Foo", 1, DataVariableValueType.Bool, typeof(bool) },
            { "Foo", 1, DataVariableValueType.Int, typeof(int) },
            { "Foo", 1, DataVariableValueType.Float, typeof(float) },
            { "Foo", 1, DataVariableValueType.Double, typeof(double) },
        };
    }
}
