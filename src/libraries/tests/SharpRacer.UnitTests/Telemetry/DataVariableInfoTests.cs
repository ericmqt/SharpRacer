using System.Runtime.CompilerServices;
using SharpRacer.Interop;

namespace SharpRacer.Telemetry;
public class DataVariableInfoTests
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

        var valueType = DataVariableValueType.Double;
        var valueCount = 1;
        var isTimeSliceArray = false;
        var offset = 1024;

        var header = new DataVariableHeader(
            varName,
            (int)valueType,
            valueCount,
            isTimeSliceArray,
            offset,
            description,
            unit);

        var variableInfo = new DataVariableInfo(header);

        Assert.Equal(varNameStr, variableInfo.Name);
        Assert.Equal(descriptionStr, variableInfo.Description);
        Assert.Equal(unitStr, variableInfo.ValueUnit);
        Assert.Equal(valueType, variableInfo.ValueType);
        Assert.Equal(isTimeSliceArray, variableInfo.IsTimeSliceArray);
        Assert.Equal(valueCount, variableInfo.ValueCount);
        Assert.Equal(offset, variableInfo.Offset);
        Assert.Equal(Unsafe.SizeOf<double>(), variableInfo.ValueSize);
    }

    [Fact]
    public void Ctor_NullOrEmptyUnitTest()
    {
        var emptyUnitHeader = CreateScalarHeader("Test", DataVariableValueType.Float, 12123, "test", string.Empty);
        var emptyUnitVariableInfo = new DataVariableInfo(emptyUnitHeader);

        Assert.Null(emptyUnitVariableInfo.ValueUnit);

        var nullUnitHeader = CreateScalarHeader("Test", DataVariableValueType.Float, 12123, "test", null);
        var nullUnitVariableInfo = new DataVariableInfo(nullUnitHeader);

        Assert.Null(nullUnitVariableInfo.ValueUnit);
    }

    [Fact]
    public void Ctor_ValueTypeTest()
    {
        var byteVariableInfo = createVariableInfo(DataVariableValueType.Byte);
        Assert.Equal(DataVariableValueType.Byte, byteVariableInfo.ValueType);
        Assert.Equal(1, byteVariableInfo.ValueSize);

        var boolVariableInfo = createVariableInfo(DataVariableValueType.Bool);
        Assert.Equal(DataVariableValueType.Bool, boolVariableInfo.ValueType);
        Assert.Equal(1, boolVariableInfo.ValueSize);

        var intVariableInfo = createVariableInfo(DataVariableValueType.Int);
        Assert.Equal(DataVariableValueType.Int, intVariableInfo.ValueType);
        Assert.Equal(4, intVariableInfo.ValueSize);

        var bitfieldVariableInfo = createVariableInfo(DataVariableValueType.Bitfield);
        Assert.Equal(DataVariableValueType.Bitfield, bitfieldVariableInfo.ValueType);
        Assert.Equal(4, bitfieldVariableInfo.ValueSize);

        var floatVariableInfo = createVariableInfo(DataVariableValueType.Float);
        Assert.Equal(DataVariableValueType.Float, floatVariableInfo.ValueType);
        Assert.Equal(4, floatVariableInfo.ValueSize);

        var doubleVariableInfo = createVariableInfo(DataVariableValueType.Double);
        Assert.Equal(DataVariableValueType.Double, doubleVariableInfo.ValueType);
        Assert.Equal(8, doubleVariableInfo.ValueSize);

        static DataVariableInfo createVariableInfo(DataVariableValueType valueType)
        {
            var header = CreateScalarHeader("Foo", valueType, 2048, "Description", "unit");

            return new DataVariableInfo(header);
        }
    }

    private static DataVariableHeader CreateArrayHeader(
        string variableName,
        DataVariableValueType valueType,
        int valueCount,
        bool isTimeSliceArray,
        int offset,
        string description,
        string? unit)
    {
        return new DataVariableHeader(
            IRSDKString.FromString(variableName),
            (int)valueType,
            valueCount,
            isTimeSliceArray,
            offset,
            IRSDKDescString.FromString(description),
            IRSDKString.FromString(unit));
    }

    private static DataVariableHeader CreateScalarHeader(
        string variableName,
        DataVariableValueType valueType,
        int offset,
        string description,
        string? unit)
    {
        return new DataVariableHeader(
            IRSDKString.FromString(variableName),
            (int)valueType,
            1,
            false,
            offset,
            IRSDKDescString.FromString(description),
            IRSDKString.FromString(unit));
    }
}
