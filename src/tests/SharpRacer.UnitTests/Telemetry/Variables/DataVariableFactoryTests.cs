using SharpRacer.Telemetry.Variables.TestVariables;

namespace SharpRacer.Telemetry.Variables;
public class DataVariableFactoryTests
{
    [Fact]
    public void Ctor_Test()
    {
        var fooVar = DataVariableInfoFactory.CreateScalar("Foo", DataVariableValueType.Int);

        var factory = new DataVariableFactory(new[] { fooVar });
    }

    [Fact]
    public void Ctor_NullDataVariablesTest()
    {
        Assert.Throws<ArgumentNullException>(() => new DataVariableFactory(dataVariables: null!));
    }

    [Fact]
    public void CreateScalar_Test()
    {
        var fooVar = DataVariableInfoFactory.CreateScalar("Foo", DataVariableValueType.Int);

        var factory = new DataVariableFactory(new[] { fooVar });

        var variable = factory.CreateScalar<int>("Foo");

        Assert.True(variable.IsAvailable);
        Assert.Equal(fooVar, variable.VariableInfo);
    }

    [Fact]
    public void CreateScalar_ThrowOnArrayVariableTest()
    {
        var fooVar = DataVariableInfoFactory.CreateArray("Foo", DataVariableValueType.Float, 4);

        var factory = new DataVariableFactory(new[] { fooVar });

        Assert.Throws<ArgumentException>(() => factory.CreateScalar<float>("Foo"));
    }

    [Fact]
    public void CreateScalar_UnavailableTest()
    {
        var fooVar = DataVariableInfoFactory.CreateScalar("Foo", DataVariableValueType.Int);

        var factory = new DataVariableFactory(new[] { fooVar });

        var variable = factory.CreateScalar<int>("Bar");

        Assert.False(variable.IsAvailable);
        Assert.Null(variable.VariableInfo);
    }

    [Fact]
    public void CreateArray_Test()
    {
        var arrayLength = 3;
        var isTimeSliceArray = false;
        var barVar = DataVariableInfoFactory.CreateArray("Bar", DataVariableValueType.Float, arrayLength, isTimeSliceArray);

        var factory = new DataVariableFactory(new[] { barVar });

        var variable = factory.CreateArray<float>("Bar", arrayLength);

        Assert.True(variable.IsAvailable);
        Assert.Equal(barVar, variable.VariableInfo);
        Assert.Equal(arrayLength, variable.ValueCount);
    }

    [Fact]
    public void CreateArray_UnavailableTest()
    {
        var arrayLength = 3;
        var isTimeSliceArray = false;
        var barVar = DataVariableInfoFactory.CreateArray("Bar", DataVariableValueType.Float, arrayLength, isTimeSliceArray);

        var factory = new DataVariableFactory(new[] { barVar });

        var variable = factory.CreateArray<float>("Foo", arrayLength);

        Assert.False(variable.IsAvailable);
        Assert.Null(variable.VariableInfo);
    }

    [Fact]
    public void CreateType_UnavailableTest()
    {
        var fooVar = DataVariableInfoFactory.CreateArray("Foo", DataVariableValueType.Float, 3);

        var factory = new DataVariableFactory(new[] { fooVar });
        var variable = factory.CreateType<BarVariable>("Bar");

        Assert.False(variable.IsAvailable);
        Assert.Null(variable.VariableInfo);
    }

    [Fact]
    public void CreateTypedArray_Test()
    {
        var arrayLength = 3;
        var isTimeSliceArray = false;
        var barVar = DataVariableInfoFactory.CreateArray("Bar", DataVariableValueType.Float, arrayLength, isTimeSliceArray);

        var factory = new DataVariableFactory(new[] { barVar });
        var variable = factory.CreateType<BarVariable>("Bar");

        Assert.NotNull(variable);
        Assert.NotNull(variable.VariableInfo);
        Assert.True(variable.IsAvailable);
        Assert.Equal("Bar", variable.Name);
        Assert.Equal(arrayLength, variable.ValueCount);
        Assert.Equal(barVar, variable.VariableInfo);
    }

    [Fact]
    public void CreateTypedScalar_Test()
    {
        var fooVar = DataVariableInfoFactory.CreateScalar("Foo", DataVariableValueType.Int);

        var factory = new DataVariableFactory(new[] { fooVar });

        var variable = factory.CreateType<FooVariable>("Foo");

        Assert.NotNull(variable);
        Assert.NotNull(variable.VariableInfo);
        Assert.True(variable.IsAvailable);
        Assert.Equal("Foo", variable.Name);
        Assert.Equal(fooVar, variable.VariableInfo);
    }

    [Fact]
    public void CreateTypedScalar_ThrowOnArrayVariableTest()
    {
        var fooVar = DataVariableInfoFactory.CreateArray("Foo", DataVariableValueType.Int, 4);

        var factory = new DataVariableFactory(new[] { fooVar });

        Assert.Throws<DataVariableInitializationException>(() => factory.CreateType<FooVariable>("Foo"));
    }

    [Fact]
    public void CreateTypedScalar_ThrowOnNullOrEmptyNameTest()
    {
        var fooVar = DataVariableInfoFactory.CreateArray("Foo", DataVariableValueType.Int, 4);

        var factory = new DataVariableFactory(new[] { fooVar });

        Assert.Throws<ArgumentNullException>(() => factory.CreateType<FooVariable>(null!));
        Assert.Throws<ArgumentException>(() => factory.CreateType<FooVariable>(string.Empty));
    }

    [Fact]
    public void CreateTypedScalar_ThrowOnValueTypeMismatchTest()
    {
        var fooVar = DataVariableInfoFactory.CreateScalar("Foo", DataVariableValueType.Float);

        var factory = new DataVariableFactory(new[] { fooVar });

        Assert.Throws<DataVariableInitializationException>(() => factory.CreateType<FooVariable>("Foo"));
    }

    [Fact]
    public void IsValueTypeMatch_Test()
    {
        // Bool
        Assert.True(DataVariableFactory.IsValueTypeMatch<bool>(DataVariableValueType.Bool));

        Assert.False(DataVariableFactory.IsValueTypeMatch<bool>(DataVariableValueType.Byte));
        Assert.False(DataVariableFactory.IsValueTypeMatch<bool>(DataVariableValueType.Bitfield));
        Assert.False(DataVariableFactory.IsValueTypeMatch<bool>(DataVariableValueType.Int));
        Assert.False(DataVariableFactory.IsValueTypeMatch<bool>(DataVariableValueType.Float));
        Assert.False(DataVariableFactory.IsValueTypeMatch<bool>(DataVariableValueType.Double));

        // Byte
        Assert.True(DataVariableFactory.IsValueTypeMatch<byte>(DataVariableValueType.Byte));

        Assert.False(DataVariableFactory.IsValueTypeMatch<byte>(DataVariableValueType.Bool));
        Assert.False(DataVariableFactory.IsValueTypeMatch<byte>(DataVariableValueType.Bitfield));
        Assert.False(DataVariableFactory.IsValueTypeMatch<byte>(DataVariableValueType.Int));
        Assert.False(DataVariableFactory.IsValueTypeMatch<byte>(DataVariableValueType.Float));
        Assert.False(DataVariableFactory.IsValueTypeMatch<byte>(DataVariableValueType.Double));

        // Bitfield
        Assert.True(DataVariableFactory.IsValueTypeMatch<int>(DataVariableValueType.Bitfield));
        Assert.True(DataVariableFactory.IsValueTypeMatch<int>(DataVariableValueType.Int));

        Assert.False(DataVariableFactory.IsValueTypeMatch<int>(DataVariableValueType.Bool));
        Assert.False(DataVariableFactory.IsValueTypeMatch<int>(DataVariableValueType.Byte));
        Assert.False(DataVariableFactory.IsValueTypeMatch<int>(DataVariableValueType.Float));
        Assert.False(DataVariableFactory.IsValueTypeMatch<int>(DataVariableValueType.Double));

        // Int
        Assert.True(DataVariableFactory.IsValueTypeMatch<int>(DataVariableValueType.Int));
        Assert.True(DataVariableFactory.IsValueTypeMatch<int>(DataVariableValueType.Bitfield));

        Assert.False(DataVariableFactory.IsValueTypeMatch<int>(DataVariableValueType.Bool));
        Assert.False(DataVariableFactory.IsValueTypeMatch<int>(DataVariableValueType.Byte));
        Assert.False(DataVariableFactory.IsValueTypeMatch<int>(DataVariableValueType.Float));
        Assert.False(DataVariableFactory.IsValueTypeMatch<int>(DataVariableValueType.Double));

        // Float
        Assert.True(DataVariableFactory.IsValueTypeMatch<float>(DataVariableValueType.Float));

        Assert.False(DataVariableFactory.IsValueTypeMatch<float>(DataVariableValueType.Bool));
        Assert.False(DataVariableFactory.IsValueTypeMatch<float>(DataVariableValueType.Byte));
        Assert.False(DataVariableFactory.IsValueTypeMatch<float>(DataVariableValueType.Bitfield));
        Assert.False(DataVariableFactory.IsValueTypeMatch<float>(DataVariableValueType.Int));
        Assert.False(DataVariableFactory.IsValueTypeMatch<float>(DataVariableValueType.Double));

        // Double
        Assert.True(DataVariableFactory.IsValueTypeMatch<double>(DataVariableValueType.Double));

        Assert.False(DataVariableFactory.IsValueTypeMatch<double>(DataVariableValueType.Bool));
        Assert.False(DataVariableFactory.IsValueTypeMatch<double>(DataVariableValueType.Byte));
        Assert.False(DataVariableFactory.IsValueTypeMatch<double>(DataVariableValueType.Bitfield));
        Assert.False(DataVariableFactory.IsValueTypeMatch<double>(DataVariableValueType.Int));
        Assert.False(DataVariableFactory.IsValueTypeMatch<double>(DataVariableValueType.Float));

        Assert.False(DataVariableFactory.IsValueTypeMatch<double>((DataVariableValueType)9999));
    }
}
