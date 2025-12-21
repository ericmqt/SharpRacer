using System.Runtime.InteropServices;
using SharpRacer.Extensions.Xunit;
using SharpRacer.Telemetry;

namespace SharpRacer.Interop;

public class TelemetryVariableHeaderTests
{
    public static TheoryData<TelemetryVariableHeader, TelemetryVariableHeader> InequalityData => GetInequalityData();

    [Fact]
    public void Ctor_DefaultTest()
    {
        var header = new TelemetryVariableHeader();

        Assert.Equal(default, header.Count);
        Assert.Equal(default, header.CountAsTime);
        Assert.Equal(default, header.Description);
        Assert.Equal(default, header.Name);
        Assert.Equal(default, header.Offset);
        Assert.Equal(default, header.Type);
        Assert.Equal(default, header.Unit);
    }

    [Fact]
    public void Ctor_ParameterizedTest()
    {
        var varNameStr = "Foo";
        var varName = IRSDKString.FromString(varNameStr);

        var descriptionStr = "Foo variable";
        var description = IRSDKDescString.FromString(descriptionStr);

        var unitStr = "kg/m";
        var unit = IRSDKString.FromString(unitStr);

        var valueType = (int)TelemetryVariableValueType.Double;
        var valueCount = 1;
        var isTimeSliceArray = false;
        var offset = 1024;

        var header = new TelemetryVariableHeader(
            varName,
            valueType,
            valueCount,
            isTimeSliceArray,
            offset,
            description,
            unit);

        Assert.Equal(varName, header.Name);
        Assert.Equal(valueType, header.Type);
        Assert.Equal(valueCount, header.Count);
        Assert.Equal(isTimeSliceArray, header.CountAsTime);
        Assert.Equal(offset, header.Offset);
        Assert.Equal(description, header.Description);
        Assert.Equal(unit, header.Unit);
    }

    [Fact]
    public void StructLayout_Test()
    {
        var name = IRSDKString.FromString("Foo");
        var description = IRSDKDescString.FromString("Foo variable");
        var unit = IRSDKString.FromString("kg/m");

        var type = (int)TelemetryVariableValueType.Double;
        var count = 1;
        var countAsTime = false;
        var offset = 1024;

        Span<byte> blob = new byte[TelemetryVariableHeader.Size];

        MemoryMarshal.Write(blob[TelemetryVariableHeader.FieldOffsets.CountAsTimeOffset..], countAsTime);
        MemoryMarshal.Write(blob[TelemetryVariableHeader.FieldOffsets.CountOffset..], count);
        MemoryMarshal.Write(blob[TelemetryVariableHeader.FieldOffsets.DescriptionOffset..], description);
        MemoryMarshal.Write(blob[TelemetryVariableHeader.FieldOffsets.NameOffset..], name);
        MemoryMarshal.Write(blob[TelemetryVariableHeader.FieldOffsets.OffsetOffset..], offset);
        MemoryMarshal.Write(blob[TelemetryVariableHeader.FieldOffsets.TypeOffset..], type);
        MemoryMarshal.Write(blob[TelemetryVariableHeader.FieldOffsets.UnitOffset..], unit);

        var header = MemoryMarshal.Read<TelemetryVariableHeader>(blob);

        Assert.Equal(countAsTime, header.CountAsTime);
        Assert.Equal(count, header.Count);
        Assert.Equal(description, header.Description);
        Assert.Equal(name, header.Name);
        Assert.Equal(offset, header.Offset);
        Assert.Equal(type, header.Type);
        Assert.Equal(unit, header.Unit);
    }

    [Fact]
    public void Equals_DefaultValueEqualityTest()
    {
        var constructedHeader = new TelemetryVariableHeader();

        EquatableStructAssert.Equal(constructedHeader, default);
    }

    [Fact]
    public void Equals_DefaultValueInequalityTest()
    {
        var constructedHeader = new TelemetryVariableHeader(
            IRSDKString.FromString("Foo"),
            (int)TelemetryVariableValueType.Double,
            1,
            false,
            1024,
            IRSDKDescString.FromString("Foo variable"),
            IRSDKString.FromString("km/h"));

        EquatableStructAssert.NotEqual(constructedHeader, default);
    }

    [Fact]
    public void Equals_EqualityTest()
    {
        var header1 = new TelemetryVariableHeader(
            IRSDKString.FromString("Foo"),
            (int)TelemetryVariableValueType.Double,
            1,
            false,
            1024,
            IRSDKDescString.FromString("Foo variable"),
            IRSDKString.FromString("km/h"));

        var header2 = new TelemetryVariableHeader(
            IRSDKString.FromString("Foo"),
            (int)TelemetryVariableValueType.Double,
            1,
            false,
            1024,
            IRSDKDescString.FromString("Foo variable"),
            IRSDKString.FromString("km/h"));

        EquatableStructAssert.Equal(header1, header2);
    }

    [Theory]
    [MemberData(nameof(InequalityData))]
    public void Equals_InequalityTest(TelemetryVariableHeader header1, TelemetryVariableHeader header2)
    {
        EquatableStructAssert.NotEqual(header1, header2);
    }

    [Fact]
    public void Equals_NullObjectTest()
    {
        var constructedHeader = new TelemetryVariableHeader(
            IRSDKString.FromString("Foo"),
            (int)TelemetryVariableValueType.Double,
            1,
            false,
            1024,
            IRSDKDescString.FromString("Foo variable"),
            IRSDKString.FromString("km/h"));

        Assert.False(constructedHeader.Equals(obj: null));
    }

    private static TheoryData<TelemetryVariableHeader, TelemetryVariableHeader> GetInequalityData()
    {
        var name = IRSDKString.FromString("Foo");
        var description = IRSDKDescString.FromString("Foo variable");
        var unit = IRSDKString.FromString("kg/m");

        var type = (int)TelemetryVariableValueType.Double;
        var count = 1;
        var countAsTime = false;
        var offset = 1024;

        return new TheoryData<TelemetryVariableHeader, TelemetryVariableHeader>()
        {
            // Type
            {
                new TelemetryVariableHeader(name, (int)TelemetryVariableValueType.Double, count, countAsTime, offset, description, unit),
                new TelemetryVariableHeader(name, (int)TelemetryVariableValueType.Float, count, countAsTime, offset, description, unit)
            },

            // Offset
            {
                new TelemetryVariableHeader(name, type, count, countAsTime, 1024, description, unit),
                new TelemetryVariableHeader(name, type, count, countAsTime, 2048, description, unit)
            },

            // Count
            {
                new TelemetryVariableHeader(name, type, 1, countAsTime, offset, description, unit),
                new TelemetryVariableHeader(name, type, 4, countAsTime, offset, description, unit)
            },

            // CountAsTime
            {
                new TelemetryVariableHeader(name, type, count, true, offset, description, unit),
                new TelemetryVariableHeader(name, type, count, false, offset, description, unit)
            },

            // Name
            {
                new TelemetryVariableHeader(IRSDKString.FromString("Foo"), type, count, countAsTime, offset, description, unit),
                new TelemetryVariableHeader(IRSDKString.FromString("Bar"), type, count, countAsTime, offset, description, unit)
            },

            // Description
            {
                new TelemetryVariableHeader(name, type, count, countAsTime, offset, IRSDKDescString.FromString("Foo variable"), unit),
                new TelemetryVariableHeader(name, type, count, countAsTime, offset, IRSDKDescString.FromString("Bar variable"), unit)
            },

            // Unit
            {
                new TelemetryVariableHeader(name, type, count, countAsTime, offset, description, IRSDKString.FromString("kg/m")),
                new TelemetryVariableHeader(name, type, count, countAsTime, offset, description, IRSDKString.FromString("Gm/h"))
            }
        };
    }
}
