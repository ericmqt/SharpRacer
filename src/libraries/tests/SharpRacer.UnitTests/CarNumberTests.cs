namespace SharpRacer;

using SharpRacer.Extensions.Xunit;
using ExpectedCarNumberValues = (int ExpectedValue, int ExpectedBase, int ExpectedLength, string ExpectedString);

public class CarNumberTests
{
    public static TheoryData<int, ExpectedCarNumberValues> ConstructorTestData { get; } = GetCtorTestData();
    public static TheoryData<CarNumber, CarNumber> EqualityTestData { get; } = GetEqualityTestData();
    public static TheoryData<CarNumber, CarNumber> InequalityTestData { get; } = GetInequalityTestData();

    [Theory]
    [MemberData(nameof(ConstructorTestData))]
    public void Ctor_Test(int rawValue, ExpectedCarNumberValues expected)
    {
        var carNumber = new CarNumber(rawValue);

        Assert.True(carNumber.HasValue);
        Assert.Equal(expected.ExpectedValue, carNumber.Value);
        Assert.Equal(expected.ExpectedBase, carNumber.Base);
        Assert.Equal(expected.ExpectedLength, carNumber.Length);
        Assert.Equal(expected.ExpectedString, carNumber.ToString());
    }

    [Fact]
    public void Ctor_DefaultTest()
    {
        var carNumber = default(CarNumber);

        Assert.False(carNumber.HasValue);
        Assert.Equal(0, carNumber.Value);
        Assert.Equal(0, carNumber.Length);
        Assert.Equal(0, carNumber.Base);
        Assert.Equal(string.Empty, carNumber.ToString());
    }

    [Fact]
    public void Ctor_ThrowIfRawValueIsNegativeTest()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new CarNumber(-1));
    }

    [Fact]
    public void Ctor_ThrowIfRawValueIsGreaterThanMaxTest()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new CarNumber(4000));
    }

    [Theory]
    [MemberData(nameof(EqualityTestData))]
    public void Equals_Test(CarNumber left, CarNumber right)
    {
        EquatableStructAssert.Equal(left, right);
    }

    [Fact]
    public void Equals_DefaultValueTest()
    {
        var carNumber1 = default(CarNumber);
        var carNumber2 = default(CarNumber);

        EquatableStructAssert.Equal(carNumber1, carNumber2);

        carNumber1 = new CarNumber(0);
        EquatableStructAssert.NotEqual(carNumber1, carNumber2);
    }

    [Theory]
    [MemberData(nameof(InequalityTestData))]
    public void Equals_InequalityTest(CarNumber left, CarNumber right)
    {
        EquatableStructAssert.NotEqual(left, right);
    }

    [Fact]
    public void Equals_ObjectInequalityTest()
    {
        var carNumber = new CarNumber(12);
        var other = new object();

        EquatableStructAssert.ObjectEqualsMethod(false, carNumber, other);
    }

    [Fact]
    public void None_Test()
    {
        var carNumber = CarNumber.None;

        Assert.False(carNumber.HasValue);
        Assert.Equal(0, carNumber.Value);
        Assert.Equal(0, carNumber.Length);
        Assert.Equal(0, carNumber.Base);
        Assert.Equal(string.Empty, carNumber.ToString());
    }

    [Fact]
    public void GetLeadingZeroCount_SpanTest()
    {
        const string s0 = "10023456";
        const string s1 = "01020304";
        const string s2 = "001230456";
        const string s3 = "00010002";

        Assert.Equal(0, CarNumber.GetLeadingZeroCount(s0));
        Assert.Equal(1, CarNumber.GetLeadingZeroCount(s1));
        Assert.Equal(2, CarNumber.GetLeadingZeroCount(s2));
        Assert.Equal(3, CarNumber.GetLeadingZeroCount(s3));
    }

    [Fact]
    public void GetNonZeroDigitCount_Test()
    {
        Assert.Equal(1, CarNumber.GetNonZeroDigitCount(1));
        Assert.Equal(2, CarNumber.GetNonZeroDigitCount(10));
        Assert.Equal(3, CarNumber.GetNonZeroDigitCount(111));
    }

    [Fact]
    public void Parse_AllZeroesTest()
    {
        const string carNumberString = "000";
        var carNumber = CarNumber.Parse(carNumberString);

        Assert.Equal(0, carNumber.Base);
        Assert.Equal(3, carNumber.Length);
        Assert.Equal(3000, carNumber.Value);
        Assert.Equal(carNumberString, carNumber.ToString());
    }

    [Fact]
    public void Parse_NoLeadingZeroesTest()
    {
        const string carNumberString = "22";
        var carNumber = CarNumber.Parse(carNumberString);

        Assert.Equal(22, carNumber.Base);
        Assert.Equal(2, carNumber.Length);
        Assert.Equal(22, carNumber.Value);
        Assert.Equal(carNumberString, carNumber.ToString());
    }

    [Fact]
    public void Parse_WithLeadingZeroesTest()
    {
        const string carNumberString = "007";
        var carNumber = CarNumber.Parse(carNumberString);

        Assert.Equal(7, carNumber.Base);
        Assert.Equal(3, carNumber.Length);
        Assert.Equal(3007, carNumber.Value);
        Assert.Equal(carNumberString, carNumber.ToString());
    }

    [Fact]
    public void Parse_ThrowOnNonDigitCharacterTest()
    {
        Assert.Throws<FormatException>(() => CarNumber.Parse("00X"));
    }

    [Fact]
    public void ParseSpan_ThrowIfEmptyTest()
    {
        Assert.Throws<ArgumentException>(() => CarNumber.Parse(Span<char>.Empty));
    }

    [Fact]
    public void ParseSpan_ThrowIfLengthExceedsMaxTest()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            ReadOnlySpan<char> inputSpan = "1234";

            CarNumber.Parse(inputSpan);
        });
    }

    [Fact]
    public void ParseString_ThrowIfNullOrEmptyTest()
    {
        Assert.Throws<ArgumentException>(() => CarNumber.Parse(string.Empty));
        Assert.Throws<ArgumentNullException>(() => CarNumber.Parse(null!));
    }

    [Fact]
    public void ParseString_ThrowIfLengthExceedsMaxTest()
    {
        Assert.Throws<ArgumentException>(() => CarNumber.Parse("1234"));
    }

    [Fact]
    public void TryParse_AllZeroesTest()
    {
        const string carNumberString = "000";
        Assert.True(CarNumber.TryParse(carNumberString, null, out var carNumber));

        Assert.Equal(0, carNumber.Base);
        Assert.Equal(3, carNumber.Length);
        Assert.Equal(3000, carNumber.Value);
        Assert.Equal(carNumberString, carNumber.ToString());
    }

    [Fact]
    public void TryParse_NoLeadingZeroesTest()
    {
        const string carNumberString = "22";
        Assert.True(CarNumber.TryParse(carNumberString, null, out var carNumber));

        Assert.Equal(22, carNumber.Base);
        Assert.Equal(2, carNumber.Length);
        Assert.Equal(22, carNumber.Value);
        Assert.Equal(carNumberString, carNumber.ToString());
    }

    [Fact]
    public void TryParse_WithLeadingZeroesTest()
    {
        const string carNumberString = "007";
        Assert.True(CarNumber.TryParse(carNumberString, null, out var carNumber));

        Assert.Equal(7, carNumber.Base);
        Assert.Equal(3, carNumber.Length);
        Assert.Equal(3007, carNumber.Value);
        Assert.Equal(carNumberString, carNumber.ToString());
    }

    [Fact]
    public void TryParse_SpanTest()
    {
        ReadOnlySpan<char> carNumberSpan = "007";
        Assert.True(CarNumber.TryParse(carNumberSpan, out var carNumber));

        Assert.Equal(7, carNumber.Base);
        Assert.Equal(3, carNumber.Length);
        Assert.Equal(3007, carNumber.Value);
        Assert.Equal(carNumberSpan, carNumber.ToString());
    }

    [Fact]
    public void TryParseSpan_ReturnFalseIfStringExceedsMaxLengthTest()
    {
        ReadOnlySpan<char> inputSpan = "1234";

        Assert.False(CarNumber.TryParse(inputSpan, null, out var result));
        Assert.Equal(default, result);
    }

    [Fact]
    public void TryParseString_ReturnFalseIfNullOrEmptyStringTest()
    {
        string? value = null;
        Assert.False(CarNumber.TryParse(value, out var result));
        Assert.Equal(default, result);

        Assert.False(CarNumber.TryParse(string.Empty, out result));
        Assert.Equal(default, result);
    }

    [Fact]
    public void TryParseString_ReturnFalseIfStringExceedsMaxLengthTest()
    {
        Assert.False(CarNumber.TryParse("1234", null, out var result));
        Assert.Equal(default, result);
    }

    [Fact]
    public void TryParseString_ReturnFalseOnNonDigitCharacterTest()
    {
        Assert.False(CarNumber.TryParse("00X", null, out _));
    }

    private static TheoryData<int, ExpectedCarNumberValues> GetCtorTestData()
    {
        return new TheoryData<int, ExpectedCarNumberValues>()
        {
            { 7, (ExpectedValue: 7, ExpectedBase: 7, ExpectedLength: 1, ExpectedString: "7") },
            { 1007, (ExpectedValue: 7, ExpectedBase: 7, ExpectedLength: 1, ExpectedString: "7") },
            { 2007, (ExpectedValue: 2007, ExpectedBase: 7, ExpectedLength: 2, ExpectedString: "07") },
            { 3007, (ExpectedValue: 3007, ExpectedBase: 7, ExpectedLength: 3, ExpectedString: "007") },

            { 0, (ExpectedValue: 0, ExpectedBase: 0, ExpectedLength: 1, ExpectedString: "0") },
            { 1000, (ExpectedValue: 0, ExpectedBase: 0, ExpectedLength: 1, ExpectedString: "0") },
            { 2000, (ExpectedValue: 2000, ExpectedBase: 0, ExpectedLength: 2, ExpectedString: "00") },
            { 3000, (ExpectedValue: 3000, ExpectedBase: 0, ExpectedLength: 3, ExpectedString: "000") },

            { 22, (ExpectedValue: 22, ExpectedBase: 22, ExpectedLength: 2, ExpectedString: "22") },
            { 1022, (ExpectedValue: 22, ExpectedBase: 22, ExpectedLength: 2, ExpectedString: "22") },
            { 2022, (ExpectedValue: 22, ExpectedBase: 22, ExpectedLength: 2, ExpectedString: "22") },
            { 3022, (ExpectedValue: 3022, ExpectedBase: 22, ExpectedLength: 3, ExpectedString: "022") },

            { 333, (ExpectedValue: 333, ExpectedBase: 333, ExpectedLength: 3, ExpectedString: "333") },
            { 1333, (ExpectedValue: 333, ExpectedBase: 333, ExpectedLength: 3, ExpectedString: "333") },
            { 2333, (ExpectedValue: 333, ExpectedBase: 333, ExpectedLength: 3, ExpectedString: "333") },
            { 3333, (ExpectedValue: 333, ExpectedBase: 333, ExpectedLength: 3, ExpectedString: "333") },
        };
    }

    private static TheoryData<CarNumber, CarNumber> GetEqualityTestData()
    {
        return new TheoryData<CarNumber, CarNumber>()
        {
            { new(0), new(0) },
            { new(0), new(1000) },
            { new(2000), new(2000) },
            { new(3000), new(3000) },

            { new(7), new(7) },
            { new(7), new(1007) },
            { new(2007), new(2007) },
            { new(3007), new(3007) },

            { new(22), new(22) },
            { new(22), new(1022) },
            { new(22), new(2022) },

            { new(333), new(333) },
            { new(333), new(1333) },
            { new(333), new(2333) },
            { new(333), new(3333) },
        };
    }

    private static TheoryData<CarNumber, CarNumber> GetInequalityTestData()
    {
        return new TheoryData<CarNumber, CarNumber>()
        {
            { new(0), default },
            { new(0), new(1) },
            { new(1), new(2) },

            { new(0), new(2000) },
            { new(0), new(3000) },
            { new(2000), new(3000) },

            { new(7), new(2007) },
            { new(7), new(3007) },

            { new(22), new(3022) }
        };
    }
}
