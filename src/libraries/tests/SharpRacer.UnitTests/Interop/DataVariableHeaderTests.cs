using System.Runtime.InteropServices;
using SharpRacer.Telemetry;

namespace SharpRacer.Interop;

public class DataVariableHeaderTests
{
    [Fact]
    public void Ctor_Test()
    {
        var varNameStr = "Foo";
        var varName = IRSDKString.FromString(varNameStr);

        var descriptionStr = "Foo variable";
        var description = IRSDKDescString.FromString(descriptionStr);

        var unitStr = "kg/m";
        var unit = IRSDKString.FromString(unitStr);

        var valueType = (int)DataVariableValueType.Double;
        var valueCount = 1;
        var isTimeSliceArray = false;
        var offset = 1024;


        var header = new DataVariableHeader(
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

        var type = (int)DataVariableValueType.Double;
        var count = 1;
        var countAsTime = false;
        var offset = 1024;

        Span<byte> blob = new byte[DataVariableHeader.Size];

        MemoryMarshal.Write(blob[DataVariableHeader.FieldOffsets.CountAsTimeOffset..], countAsTime);
        MemoryMarshal.Write(blob[DataVariableHeader.FieldOffsets.CountOffset..], count);
        MemoryMarshal.Write(blob[DataVariableHeader.FieldOffsets.DescriptionOffset..], description);
        MemoryMarshal.Write(blob[DataVariableHeader.FieldOffsets.NameOffset..], name);
        MemoryMarshal.Write(blob[DataVariableHeader.FieldOffsets.OffsetOffset..], offset);
        MemoryMarshal.Write(blob[DataVariableHeader.FieldOffsets.TypeOffset..], type);
        MemoryMarshal.Write(blob[DataVariableHeader.FieldOffsets.UnitOffset..], unit);

        var header = MemoryMarshal.Read<DataVariableHeader>(blob);

        Assert.Equal(countAsTime, header.CountAsTime);
        Assert.Equal(count, header.Count);
        Assert.Equal(description, header.Description);
        Assert.Equal(name, header.Name);
        Assert.Equal(offset, header.Offset);
        Assert.Equal(type, header.Type);
        Assert.Equal(unit, header.Unit);
    }
}
