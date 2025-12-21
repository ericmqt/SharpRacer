using System.Runtime.CompilerServices;
using SharpRacer.Interop;
using SharpRacer.Telemetry.TestUtilities;

namespace SharpRacer.Telemetry;

public class TelemetryVariableInfoTests
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

        var valueType = TelemetryVariableValueType.Double;
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

        var variableInfo = new TelemetryVariableInfo(header);

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
        var emptyUnitHeader = CreateScalarHeader("Test", TelemetryVariableValueType.Float, 12123, "test", string.Empty);
        var emptyUnitVariableInfo = new TelemetryVariableInfo(emptyUnitHeader);

        Assert.Null(emptyUnitVariableInfo.ValueUnit);

        var nullUnitHeader = CreateScalarHeader("Test", TelemetryVariableValueType.Float, 12123, "test", null);
        var nullUnitVariableInfo = new TelemetryVariableInfo(nullUnitHeader);

        Assert.Null(nullUnitVariableInfo.ValueUnit);
    }

    [Fact]
    public void Ctor_ValueTypeTest()
    {
        var byteVariableInfo = createVariableInfo(TelemetryVariableValueType.Byte);
        Assert.Equal(TelemetryVariableValueType.Byte, byteVariableInfo.ValueType);
        Assert.Equal(1, byteVariableInfo.ValueSize);

        var boolVariableInfo = createVariableInfo(TelemetryVariableValueType.Bool);
        Assert.Equal(TelemetryVariableValueType.Bool, boolVariableInfo.ValueType);
        Assert.Equal(1, boolVariableInfo.ValueSize);

        var intVariableInfo = createVariableInfo(TelemetryVariableValueType.Int);
        Assert.Equal(TelemetryVariableValueType.Int, intVariableInfo.ValueType);
        Assert.Equal(4, intVariableInfo.ValueSize);

        var bitfieldVariableInfo = createVariableInfo(TelemetryVariableValueType.Bitfield);
        Assert.Equal(TelemetryVariableValueType.Bitfield, bitfieldVariableInfo.ValueType);
        Assert.Equal(4, bitfieldVariableInfo.ValueSize);

        var floatVariableInfo = createVariableInfo(TelemetryVariableValueType.Float);
        Assert.Equal(TelemetryVariableValueType.Float, floatVariableInfo.ValueType);
        Assert.Equal(4, floatVariableInfo.ValueSize);

        var doubleVariableInfo = createVariableInfo(TelemetryVariableValueType.Double);
        Assert.Equal(TelemetryVariableValueType.Double, doubleVariableInfo.ValueType);
        Assert.Equal(8, doubleVariableInfo.ValueSize);

        static TelemetryVariableInfo createVariableInfo(TelemetryVariableValueType valueType)
        {
            var header = CreateScalarHeader("Foo", valueType, 2048, "Description", "unit");

            return new TelemetryVariableInfo(header);
        }
    }

    [Fact]
    public void GetValueSpan_Test()
    {
        var varHeader = CreateScalarHeader("Foo", TelemetryVariableValueType.Int, offset: 8, description: "Test", unit: "test/s");
        int varHeaderValue = 123;

        var variableInfo = new TelemetryVariableInfo(varHeader);

        var frameBlob = new byte[256];
        var frameMemory = new Memory<byte>(frameBlob);
        var frameWriter = new DataFrameWriter(frameMemory);
        frameWriter.Write(varHeader, varHeaderValue);

        var varHeaderValueSpan = frameMemory.Span.Slice(varHeader.Offset, varHeader.GetDataLength());

        var valueSpan = variableInfo.GetValueSpan(frameBlob);

        Assert.True(varHeaderValueSpan.SequenceEqual(valueSpan));
    }

    private static DataVariableHeader CreateArrayHeader(
        string variableName,
        TelemetryVariableValueType valueType,
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
        TelemetryVariableValueType valueType,
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
