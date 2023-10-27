namespace SharpRacer.Telemetry;
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
        Assert.Throws<ArgumentNullException>(() => new DataVariableFactory(null!));
    }

    [Fact]
    public void Create_Test()
    {
        var fooVar = DataVariableInfoFactory.CreateScalar("Foo", DataVariableValueType.Int);

        var factory = new DataVariableFactory(new[] { fooVar });

        var variable = factory.Create<int>("Foo");

        Assert.True(variable.IsAvailable);
        Assert.Equal(fooVar, variable.VariableInfo);
    }

    [Fact]
    public void Create_ThrowOnArrayVariableTest()
    {
        var fooVar = DataVariableInfoFactory.CreateArray("Foo", DataVariableValueType.Int, 4);

        var factory = new DataVariableFactory(new[] { fooVar });

        Assert.Throws<ArgumentException>(() => factory.Create<int>("Foo"));
    }

    [Fact]
    public void Create_ThrowOnValueTypeMismatchTest()
    {
        var fooVar = DataVariableInfoFactory.CreateScalar("Foo", DataVariableValueType.Int);

        var factory = new DataVariableFactory(new[] { fooVar });

        Assert.Throws<ArgumentException>(() => factory.Create<bool>("Foo"));
    }

    [Fact]
    public void CreatePlaceholder_Test()
    {
        var factory = new DataVariableFactory(Enumerable.Empty<DataVariableInfo>());

        var boolPlaceholder = factory.Create<bool>("DoesntExist");
        var floatArrayPlaceholder = factory.CreateArray<float>("Nope", 23, false);

        Assert.NotNull(boolPlaceholder);
        Assert.False(boolPlaceholder.IsAvailable);

        Assert.NotNull(floatArrayPlaceholder);
        Assert.False(floatArrayPlaceholder.IsAvailable);
    }

    [Fact]
    public void CreateT_ThrowsOnNullOrEmptyNameTest()
    {
        var factory = new DataVariableFactory(Enumerable.Empty<DataVariableInfo>());

        Assert.Throws<ArgumentNullException>(() => factory.Create<bool>(null!));
        Assert.Throws<ArgumentException>(() => factory.Create<bool>(string.Empty));
    }

    [Fact]
    public void CreateArray_Test()
    {
        var arrayLength = 4;
        var isTimeSliceArray = false;
        var fooVar = DataVariableInfoFactory.CreateArray("Foo", DataVariableValueType.Int, arrayLength, isTimeSliceArray);

        var factory = new DataVariableFactory(new[] { fooVar });
        var variable = factory.CreateArray<int>("Foo", arrayLength, isTimeSliceArray);

        Assert.True(variable.IsAvailable);
        Assert.Equal(arrayLength, variable.ArrayLength);
        Assert.Equal(isTimeSliceArray, variable.IsTimeSliceArray);
        Assert.Equal(fooVar, variable.VariableInfo);
    }

    [Fact]
    public void CreateArray_ThrowsOnArrayLengthNotGreaterThan1Test()
    {
        var factory = new DataVariableFactory(Enumerable.Empty<DataVariableInfo>());

        Assert.Throws<ArgumentOutOfRangeException>(() => factory.CreateArray<bool>("Foo", 1, false));
        Assert.Throws<ArgumentOutOfRangeException>(() => factory.CreateArray<bool>("Bar", 0, true));
        Assert.Throws<ArgumentOutOfRangeException>(() => factory.CreateArray<bool>("Moo", -1, true));
    }

    [Fact]
    public void CreateArray_ThrowsOnArrayLengthMismatchTest()
    {
        var arrayLength = 4;
        var isTimeSliceArray = false;
        var fooVar = DataVariableInfoFactory.CreateArray("Foo", DataVariableValueType.Int, arrayLength, isTimeSliceArray);

        var factory = new DataVariableFactory(new[] { fooVar });

        Assert.Throws<ArgumentException>(() => factory.CreateArray<int>("Foo", arrayLength + 9, isTimeSliceArray));
    }

    [Fact]
    public void CreateArray_ThrowsOnNullOrEmptyNameTest()
    {
        var factory = new DataVariableFactory(Enumerable.Empty<DataVariableInfo>());

        Assert.Throws<ArgumentNullException>(() => factory.CreateArray<bool>(null!, 9, false));
        Assert.Throws<ArgumentException>(() => factory.CreateArray<bool>(string.Empty, 32, true));
    }

    [Fact]
    public void CreateArray_ThrowOnScalarVariableTest()
    {
        var fooVar = DataVariableInfoFactory.CreateScalar("Foo", DataVariableValueType.Int);

        var factory = new DataVariableFactory(new[] { fooVar });

        Assert.Throws<ArgumentException>(() => factory.CreateArray<int>("Foo", 4, false));
    }

    [Fact]
    public void CreateArray_ThrowOnTimeSliceMismatchTest()
    {
        var arrayLength = 4;

        var fooVar = DataVariableInfoFactory.CreateArray("Foo", DataVariableValueType.Int, arrayLength, false);
        var barVar = DataVariableInfoFactory.CreateArray("Bar", DataVariableValueType.Int, arrayLength, true);

        var factory = new DataVariableFactory(new[] { fooVar, barVar });

        Assert.Throws<ArgumentException>(() => factory.CreateArray<int>("Foo", arrayLength, true));
        Assert.Throws<ArgumentException>(() => factory.CreateArray<int>("Bar", arrayLength, false));
    }

    [Fact]
    public void CreateArray_ThrowOnValueTypeMismatchTest()
    {
        var arrayLength = 4;
        var isTimeSliceArray = false;
        var fooVar = DataVariableInfoFactory.CreateArray("Foo", DataVariableValueType.Int, arrayLength, isTimeSliceArray);

        var factory = new DataVariableFactory(new[] { fooVar });

        Assert.Throws<ArgumentException>(() => factory.CreateArray<bool>("Foo", arrayLength, isTimeSliceArray));
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
