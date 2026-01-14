using SharpRacer.Extensions.Xunit;

namespace SharpRacer;
public class ContentVersionTests
{
    [Fact]
    public void Comparable_EqualityTest()
    {
        var left = new ContentVersion(2025, 1, 2, 3);
        var right = new ContentVersion(2025, 1, 2, 3);

        ComparableStructAssert.Equal(left, right);
    }

    [Theory]
    [MemberData(nameof(ComparisonGreaterThan_TestData))]
    public void Comparable_GreaterThanTest(ContentVersion left, ContentVersion right)
    {
        ComparableStructAssert.GreaterThan(left, right);
    }

    [Theory]
    [MemberData(nameof(ComparisonGreaterThanOrEqual_TestData))]
    public void Comparable_GreaterThanOrEqualTest(ContentVersion left, ContentVersion right)
    {
        ComparableStructAssert.CompareToMethod_GreaterThanOrEqual(left, right);
    }

    [Theory]
    [MemberData(nameof(ComparisonGreaterThan_TestData))]
    public void Comparable_InequalityTest(ContentVersion left, ContentVersion right)
    {
        ComparableStructAssert.NotEqual(left, right);
    }

    [Theory]
    [MemberData(nameof(ComparisonLessThan_TestData))]
    public void Comparable_LessThanTest(ContentVersion left, ContentVersion right)
    {
        ComparableStructAssert.LessThan(left, right);
    }

    [Theory]
    [MemberData(nameof(ComparisonLessThanOrEqual_TestData))]
    public void Comparable_LessThanOrEqualTest(ContentVersion left, ContentVersion right)
    {
        ComparableStructAssert.CompareToMethod_LessThanOrEqual(left, right);
    }

    [Fact]
    public void Equals_Test()
    {
        var left = new ContentVersion(2025, 1, 2, 3);
        var right = new ContentVersion(2025, 1, 2, 3);

        EquatableStructAssert.Equal(left, right);
        EquatableStructAssert.Equal<ContentVersion>(default, default);
    }

    [Fact]
    public void Equals_InequalityTest()
    {
        var left = new ContentVersion(2025, 1, 2, 3);
        var right = new ContentVersion(2026, 1, 2, 3);

        EquatableStructAssert.NotEqual(left, right);
        EquatableStructAssert.NotEqual(left, default);
    }

    [Fact]
    public void Equals_NullObjectTest()
    {
        var left = new ContentVersion(2025, 1, 2, 3);

        Assert.False(left.Equals(obj: null));
    }

    [Fact]
    public void Parse_Test()
    {
        var version = ContentVersion.Parse("2023.04.18.02", null);

        Assert.Equal(2023, version.Major);
        Assert.Equal(4, version.Minor);
        Assert.Equal(18, version.Build);
        Assert.Equal(2, version.Patch);
    }

    [Fact]
    public void Parse_ThrowsIfMoreThanFourPartsTest()
    {
        Assert.Throws<FormatException>(() => ContentVersion.Parse("1.2.3.4.5", null));
    }

    [Fact]
    public void Parse_ThrowsOnNonIntegerPartTest()
    {
        Assert.Throws<FormatException>(() => ContentVersion.Parse("x.2.3.4", null));
        Assert.Throws<FormatException>(() => ContentVersion.Parse("1.y.3.4", null));
        Assert.Throws<FormatException>(() => ContentVersion.Parse("1.2.z.4", null));
        Assert.Throws<FormatException>(() => ContentVersion.Parse("1.2.3.w", null));
    }

    [Fact]
    public void ToString_Test()
    {
        var version = new ContentVersion(2023, 4, 18, 2);

        Assert.Equal("2023.04.18.02", version.ToString());
    }

    [Fact]
    public void TryParse_Test()
    {
        int major = 1;
        int minor = 2;
        int build = 3;
        int patch = 4;
        var parseString = $"{major}.{minor}.{build}.{patch}";

        Assert.True(ContentVersion.TryParse(parseString, null, out var version));

        Assert.Equal(major, version.Major);
        Assert.Equal(minor, version.Minor);
        Assert.Equal(build, version.Build);
        Assert.Equal(patch, version.Patch);
    }

    [Fact]
    public void TryParse_ReturnFalseIfMoreThanFourPartsTest()
    {
        Assert.False(ContentVersion.TryParse("1.2.3.4.5", null, out _));
    }

    [Fact]
    public void TryParse_ReturnFalseIfNullOrEmptyStringTest()
    {
        Assert.False(ContentVersion.TryParse("", null, out _));
        Assert.False(ContentVersion.TryParse(string.Empty, null, out _));
        Assert.False(ContentVersion.TryParse(null, null, out _));
    }

    [Fact]
    public void TryParse_ReturnFalseOnNonIntegerPartTest()
    {
        Assert.False(ContentVersion.TryParse("X.2.3.4", null, out _));
        Assert.False(ContentVersion.TryParse("1.Y.3.4", null, out _));
        Assert.False(ContentVersion.TryParse("1.2.Z.4", null, out _));
        Assert.False(ContentVersion.TryParse("1.2.3.W", null, out _));
    }

    public static TheoryData<ContentVersion, ContentVersion> ComparisonGreaterThan_TestData()
    {
        return new TheoryData<ContentVersion, ContentVersion>()
        {
            // Major
            { new ContentVersion(2026, 1, 1, 1), new ContentVersion(2025, 1, 1, 1) },

            // Minor
            { new ContentVersion(2025, 2, 1, 1), new ContentVersion(2025, 1, 1, 1) },

            // Build
            { new ContentVersion(2025, 1, 2, 1), new ContentVersion(2025, 1, 1, 1) },

            // Patch
            { new ContentVersion(2025, 1, 1, 2), new ContentVersion(2025, 1, 1, 1) },
        };
    }

    public static TheoryData<ContentVersion, ContentVersion> ComparisonGreaterThanOrEqual_TestData()
    {
        var testData = ComparisonGreaterThan_TestData();

        testData.Add(new ContentVersion(2025, 1, 2, 3), new ContentVersion(2025, 1, 2, 3));

        return testData;
    }

    public static TheoryData<ContentVersion, ContentVersion> ComparisonLessThan_TestData()
    {
        return new TheoryData<ContentVersion, ContentVersion>()
        {
            // Major
            { new ContentVersion(2025, 1, 1, 1), new ContentVersion(2026, 1, 1, 1) },

            // Minor
            { new ContentVersion(2025, 1, 1, 1), new ContentVersion(2025, 2, 1, 1) },

            // Build
            { new ContentVersion(2025, 1, 1, 1), new ContentVersion(2025, 1, 2, 1) },

            // Patch
            { new ContentVersion(2025, 1, 1, 1), new ContentVersion(2025, 1, 1, 2) },
        };
    }

    public static TheoryData<ContentVersion, ContentVersion> ComparisonLessThanOrEqual_TestData()
    {
        var testData = ComparisonLessThan_TestData();

        testData.Add(new ContentVersion(2025, 1, 2, 3), new ContentVersion(2025, 1, 2, 3));

        return testData;
    }
}

