using System.Reflection;
using System.Runtime.Intrinsics;

namespace SharpRacer.Telemetry;

public class TelemetryVariableDescriptorTests
{
    [Fact]
    public void Ctor_Test()
    {
        const string variableName = "Foo";
        const TelemetryVariableValueType valueType = TelemetryVariableValueType.Float;
        const int valueCount = 3;

        var desc = new TelemetryVariableDescriptor(variableName, valueType, valueCount);

        Assert.Equal(variableName, desc.Name);
        Assert.Equal(valueType, desc.ValueType);
        Assert.Equal(valueCount, desc.ValueCount);
    }

    [Fact]
    public void Ctor_ThrowOnNullOrEmptyNameTest()
    {
        Assert.Throws<ArgumentNullException>(() => new TelemetryVariableDescriptor(null!, TelemetryVariableValueType.Int, 1));
        Assert.Throws<ArgumentException>(() => new TelemetryVariableDescriptor(string.Empty, TelemetryVariableValueType.Int, 1));
    }

    [Fact]
    public void Ctor_ThrowOnNegativeOrZeroValueCountTest()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new TelemetryVariableDescriptor("Foo", TelemetryVariableValueType.Int, valueCount: 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new TelemetryVariableDescriptor("Foo", TelemetryVariableValueType.Int, valueCount: -1));
    }

    [Fact]
    public void CreateArray_Test()
    {
        const string variableName = "Foo";
        const int valueCount = 3;
        const TelemetryVariableValueType valueType = TelemetryVariableValueType.Float;

        var descriptor = TelemetryVariableDescriptor.CreateArray(variableName, valueCount, valueType);

        Assert.Equal(variableName, descriptor.Name);
        Assert.Equal(valueCount, descriptor.ValueCount);
        Assert.Equal(valueType, descriptor.ValueType);
    }

    [Theory]
    [MemberData(nameof(CreateArrayGeneric_TestData))]
    public void CreateArrayGeneric_Test(string variableName, int valueCount, TelemetryVariableValueType valueType, Type typeArg)
    {
        var createScalarMethod = typeof(TelemetryVariableDescriptor)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(x => x.IsGenericMethod && x.Name == nameof(TelemetryVariableDescriptor.CreateArray));

        Assert.NotNull(createScalarMethod);

        var genericMethodInfo = createScalarMethod.MakeGenericMethod([typeArg]);

        var resultObj = genericMethodInfo.Invoke(null, [variableName, valueCount]);
        Assert.NotNull(resultObj);

        var result = (TelemetryVariableDescriptor)resultObj;

        Assert.Equal(variableName, result.Name);
        Assert.Equal(valueCount, result.ValueCount);
        Assert.Equal(valueType, result.ValueType);
    }

    [Fact]
    public void CreateScalar_Test()
    {
        const string variableName = "Foo";
        const int valueCount = 1;
        const TelemetryVariableValueType valueType = TelemetryVariableValueType.Float;

        var descriptor = TelemetryVariableDescriptor.CreateScalar(variableName, valueType);

        Assert.Equal(variableName, descriptor.Name);
        Assert.Equal(valueCount, descriptor.ValueCount);
        Assert.Equal(valueType, descriptor.ValueType);
    }

    [Theory]
    [MemberData(nameof(CreateScalarGeneric_TestData))]
    public void CreateScalarGeneric_Test(string variableName, int valueCount, TelemetryVariableValueType valueType, Type typeArg)
    {
        var createScalarMethod = typeof(TelemetryVariableDescriptor)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(x => x.IsGenericMethod && x.Name == nameof(TelemetryVariableDescriptor.CreateScalar));

        Assert.NotNull(createScalarMethod);

        var genericMethodInfo = createScalarMethod.MakeGenericMethod([typeArg]);

        var resultObj = genericMethodInfo.Invoke(null, [variableName]);
        Assert.NotNull(resultObj);

        var result = (TelemetryVariableDescriptor)resultObj;

        Assert.Equal(variableName, result.Name);
        Assert.Equal(valueCount, result.ValueCount);
        Assert.Equal(valueType, result.ValueType);
    }

    [Fact]
    public void CreateScalarGeneric_BitfieldEnumWrongSizeValueTest()
    {
        Assert.Throws<ArgumentException>(() => TelemetryVariableDescriptor.CreateScalar<SimpleByteEnum>("Foo"));
    }

    [Fact]
    public void CreateGenericMethods_InvalidTypeArgTest()
    {
        Assert.Throws<ArgumentException>(() => TelemetryVariableDescriptor.CreateScalar<Vector128<float>>("Foo"));
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

    public static TheoryData<string, int, TelemetryVariableValueType, Type> CreateArrayGeneric_TestData()
    {
        return new TheoryData<string, int, TelemetryVariableValueType, Type>()
        {
            { "Foo", 3, TelemetryVariableValueType.Bitfield, typeof(SimpleInt32Enum) },
            { "Foo", 3, TelemetryVariableValueType.Byte, typeof(byte) },
            { "Foo", 3, TelemetryVariableValueType.Bool, typeof(bool) },
            { "Foo", 3, TelemetryVariableValueType.Int, typeof(int) },
            { "Foo", 3, TelemetryVariableValueType.Float, typeof(float) },
            { "Foo", 3, TelemetryVariableValueType.Double, typeof(double) },
        };
    }

    public static TheoryData<string, int, TelemetryVariableValueType, Type> CreateScalarGeneric_TestData()
    {
        return new TheoryData<string, int, TelemetryVariableValueType, Type>()
        {
            { "Foo", 1, TelemetryVariableValueType.Bitfield, typeof(SimpleInt32Enum) },
            { "Foo", 1, TelemetryVariableValueType.Byte, typeof(byte) },
            { "Foo", 1, TelemetryVariableValueType.Bool, typeof(bool) },
            { "Foo", 1, TelemetryVariableValueType.Int, typeof(int) },
            { "Foo", 1, TelemetryVariableValueType.Float, typeof(float) },
            { "Foo", 1, TelemetryVariableValueType.Double, typeof(double) },
        };
    }
}
