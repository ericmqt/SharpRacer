using SharpRacer.Extensions.Xunit;

namespace SharpRacer.Interop;
public class DataBufferHeaderArrayTests
{
    public static TheoryData<DataBufferHeaderArray, DataBufferHeaderArray> InequalityData => GetInequalityData();

    [Fact]
    public void FromArray_Test()
    {
        var dataBufferHeaders = new DataBufferHeader[] { new(25, 5120), new(52, 5376), new(1, 5632), new(17, 5888) };
        var dataBufferHeadersArray = DataBufferHeaderArray.FromArray(dataBufferHeaders);

        Assert.Equal(dataBufferHeaders[0], dataBufferHeadersArray[0]);
        Assert.Equal(dataBufferHeaders[1], dataBufferHeadersArray[1]);
        Assert.Equal(dataBufferHeaders[2], dataBufferHeadersArray[2]);
        Assert.Equal(dataBufferHeaders[3], dataBufferHeadersArray[3]);
    }

    [Fact]
    public void FromArray_LengthMismatchTest()
    {
        var tooLongHeaders = new DataBufferHeader[] { new(25, 5120), new(52, 5376), new(1, 5632), new(17, 5888), new(94, 6247), };
        var tooShortHeaders = new DataBufferHeader[] { new(25, 5120), new(52, 5376) };

        Assert.Throws<ArgumentException>(() => DataBufferHeaderArray.FromArray(tooLongHeaders));
        Assert.Throws<ArgumentException>(() => DataBufferHeaderArray.FromArray(tooShortHeaders));
    }

    [Fact]
    public void Equals_DefaultValueEqualityTest()
    {
        var constructedHeader = new DataBufferHeaderArray();

        EquatableStructAssert.Equal(constructedHeader, default);
    }

    [Fact]
    public void Equals_DefaultValueInequalityTest()
    {
        var constructedHeader = DataBufferHeaderArray.FromArray(
            [
                new DataBufferHeader(1, 8),
                new DataBufferHeader(2, 16),
                new DataBufferHeader(3, 32),
                new DataBufferHeader(4, 64)
            ]);

        EquatableStructAssert.NotEqual(constructedHeader, default);
    }

    [Fact]
    public void Equals_EqualityTest()
    {
        var header1 = DataBufferHeaderArray.FromArray(
            [
                new DataBufferHeader(1, 8),
                new DataBufferHeader(2, 16),
                new DataBufferHeader(3, 32),
                new DataBufferHeader(4, 64)
            ]);

        var header2 = DataBufferHeaderArray.FromArray(
            [
                new DataBufferHeader(1, 8),
                new DataBufferHeader(2, 16),
                new DataBufferHeader(3, 32),
                new DataBufferHeader(4, 64)
            ]);

        EquatableStructAssert.Equal(header1, header2);
    }

    [Theory]
    [MemberData(nameof(InequalityData))]
    public void Equals_InequalityTest(DataBufferHeaderArray array1, DataBufferHeaderArray array2)
    {
        EquatableStructAssert.NotEqual(array1, array2);
    }

    [Fact]
    public void Equals_NullObjectTest()
    {
        var header1 = DataBufferHeaderArray.FromArray(
            [
                new DataBufferHeader(1, 8),
                new DataBufferHeader(2, 16),
                new DataBufferHeader(3, 32),
                new DataBufferHeader(4, 64)
            ]);

        Assert.False(header1.Equals(obj: null));
    }

    private static TheoryData<DataBufferHeaderArray, DataBufferHeaderArray> GetInequalityData()
    {
        return new TheoryData<DataBufferHeaderArray, DataBufferHeaderArray>()
        {
            // Index 0
            {
                DataBufferHeaderArray.FromArray(
                    [
                        new DataBufferHeader(1, 8),
                        new DataBufferHeader(2, 16),
                        new DataBufferHeader(3, 32),
                        new DataBufferHeader(4, 64)
                    ]),

                DataBufferHeaderArray.FromArray(
                    [
                        new DataBufferHeader(999, 234),
                        new DataBufferHeader(2, 16),
                        new DataBufferHeader(3, 32),
                        new DataBufferHeader(4, 64)
                    ])
            },

            // Index 1
            {
                DataBufferHeaderArray.FromArray(
                    [
                        new DataBufferHeader(1, 8),
                        new DataBufferHeader(2, 16),
                        new DataBufferHeader(3, 32),
                        new DataBufferHeader(4, 64)
                    ]),

                DataBufferHeaderArray.FromArray(
                    [
                        new DataBufferHeader(1, 8),
                        new DataBufferHeader(987, 123),
                        new DataBufferHeader(3, 32),
                        new DataBufferHeader(4, 64)
                    ])
            },

            // Index 2
            {
                DataBufferHeaderArray.FromArray(
                    [
                        new DataBufferHeader(1, 8),
                        new DataBufferHeader(2, 16),
                        new DataBufferHeader(3, 32),
                        new DataBufferHeader(4, 64)
                    ]),

                DataBufferHeaderArray.FromArray(
                    [
                        new DataBufferHeader(1, 8),
                        new DataBufferHeader(2, 16),
                        new DataBufferHeader(555, 678),
                        new DataBufferHeader(4, 64)
                    ])
            },

            // Index 3
            {
                DataBufferHeaderArray.FromArray(
                    [
                        new DataBufferHeader(1, 8),
                        new DataBufferHeader(2, 16),
                        new DataBufferHeader(3, 32),
                        new DataBufferHeader(4, 64)
                    ]),

                DataBufferHeaderArray.FromArray(
                    [
                        new DataBufferHeader(1, 8),
                        new DataBufferHeader(2, 16),
                        new DataBufferHeader(3, 32),
                        new DataBufferHeader(456789, 987654)
                    ])
            }
        };
    }
}
