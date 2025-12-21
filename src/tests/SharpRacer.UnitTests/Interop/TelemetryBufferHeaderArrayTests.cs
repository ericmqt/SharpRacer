using SharpRacer.Extensions.Xunit;

namespace SharpRacer.Interop;

public class TelemetryBufferHeaderArrayTests
{
    public static TheoryData<TelemetryBufferHeaderArray, TelemetryBufferHeaderArray> InequalityData => GetInequalityData();

    [Fact]
    public void FromArray_Test()
    {
        var telemetryBufferHeaders = new TelemetryBufferHeader[] { new(25, 5120), new(52, 5376), new(1, 5632), new(17, 5888) };
        var telemetryBufferHeadersArray = TelemetryBufferHeaderArray.FromArray(telemetryBufferHeaders);

        Assert.Equal(telemetryBufferHeaders[0], telemetryBufferHeadersArray[0]);
        Assert.Equal(telemetryBufferHeaders[1], telemetryBufferHeadersArray[1]);
        Assert.Equal(telemetryBufferHeaders[2], telemetryBufferHeadersArray[2]);
        Assert.Equal(telemetryBufferHeaders[3], telemetryBufferHeadersArray[3]);
    }

    [Fact]
    public void FromArray_LengthMismatchTest()
    {
        var tooLongHeaders = new TelemetryBufferHeader[] { new(25, 5120), new(52, 5376), new(1, 5632), new(17, 5888), new(94, 6247), };
        var tooShortHeaders = new TelemetryBufferHeader[] { new(25, 5120), new(52, 5376) };

        Assert.Throws<ArgumentException>(() => TelemetryBufferHeaderArray.FromArray(tooLongHeaders));
        Assert.Throws<ArgumentException>(() => TelemetryBufferHeaderArray.FromArray(tooShortHeaders));
    }

    [Fact]
    public void Equals_DefaultValueEqualityTest()
    {
        var constructedHeader = new TelemetryBufferHeaderArray();

        EquatableStructAssert.Equal(constructedHeader, default);
    }

    [Fact]
    public void Equals_DefaultValueInequalityTest()
    {
        var constructedHeader = TelemetryBufferHeaderArray.FromArray(
            [
                new TelemetryBufferHeader(1, 8),
                new TelemetryBufferHeader(2, 16),
                new TelemetryBufferHeader(3, 32),
                new TelemetryBufferHeader(4, 64)
            ]);

        EquatableStructAssert.NotEqual(constructedHeader, default);
    }

    [Fact]
    public void Equals_EqualityTest()
    {
        var header1 = TelemetryBufferHeaderArray.FromArray(
            [
                new TelemetryBufferHeader(1, 8),
                new TelemetryBufferHeader(2, 16),
                new TelemetryBufferHeader(3, 32),
                new TelemetryBufferHeader(4, 64)
            ]);

        var header2 = TelemetryBufferHeaderArray.FromArray(
            [
                new TelemetryBufferHeader(1, 8),
                new TelemetryBufferHeader(2, 16),
                new TelemetryBufferHeader(3, 32),
                new TelemetryBufferHeader(4, 64)
            ]);

        EquatableStructAssert.Equal(header1, header2);
    }

    [Theory]
    [MemberData(nameof(InequalityData))]
    public void Equals_InequalityTest(TelemetryBufferHeaderArray array1, TelemetryBufferHeaderArray array2)
    {
        EquatableStructAssert.NotEqual(array1, array2);
    }

    [Fact]
    public void Equals_NullObjectTest()
    {
        var header1 = TelemetryBufferHeaderArray.FromArray(
            [
                new TelemetryBufferHeader(1, 8),
                new TelemetryBufferHeader(2, 16),
                new TelemetryBufferHeader(3, 32),
                new TelemetryBufferHeader(4, 64)
            ]);

        Assert.False(header1.Equals(obj: null));
    }

    private static TheoryData<TelemetryBufferHeaderArray, TelemetryBufferHeaderArray> GetInequalityData()
    {
        return new TheoryData<TelemetryBufferHeaderArray, TelemetryBufferHeaderArray>()
        {
            // Index 0
            {
                TelemetryBufferHeaderArray.FromArray(
                    [
                        new TelemetryBufferHeader(1, 8),
                        new TelemetryBufferHeader(2, 16),
                        new TelemetryBufferHeader(3, 32),
                        new TelemetryBufferHeader(4, 64)
                    ]),

                TelemetryBufferHeaderArray.FromArray(
                    [
                        new TelemetryBufferHeader(999, 234),
                        new TelemetryBufferHeader(2, 16),
                        new TelemetryBufferHeader(3, 32),
                        new TelemetryBufferHeader(4, 64)
                    ])
            },

            // Index 1
            {
                TelemetryBufferHeaderArray.FromArray(
                    [
                        new TelemetryBufferHeader(1, 8),
                        new TelemetryBufferHeader(2, 16),
                        new TelemetryBufferHeader(3, 32),
                        new TelemetryBufferHeader(4, 64)
                    ]),

                TelemetryBufferHeaderArray.FromArray(
                    [
                        new TelemetryBufferHeader(1, 8),
                        new TelemetryBufferHeader(987, 123),
                        new TelemetryBufferHeader(3, 32),
                        new TelemetryBufferHeader(4, 64)
                    ])
            },

            // Index 2
            {
                TelemetryBufferHeaderArray.FromArray(
                    [
                        new TelemetryBufferHeader(1, 8),
                        new TelemetryBufferHeader(2, 16),
                        new TelemetryBufferHeader(3, 32),
                        new TelemetryBufferHeader(4, 64)
                    ]),

                TelemetryBufferHeaderArray.FromArray(
                    [
                        new TelemetryBufferHeader(1, 8),
                        new TelemetryBufferHeader(2, 16),
                        new TelemetryBufferHeader(555, 678),
                        new TelemetryBufferHeader(4, 64)
                    ])
            },

            // Index 3
            {
                TelemetryBufferHeaderArray.FromArray(
                    [
                        new TelemetryBufferHeader(1, 8),
                        new TelemetryBufferHeader(2, 16),
                        new TelemetryBufferHeader(3, 32),
                        new TelemetryBufferHeader(4, 64)
                    ]),

                TelemetryBufferHeaderArray.FromArray(
                    [
                        new TelemetryBufferHeader(1, 8),
                        new TelemetryBufferHeader(2, 16),
                        new TelemetryBufferHeader(3, 32),
                        new TelemetryBufferHeader(456789, 987654)
                    ])
            }
        };
    }
}
