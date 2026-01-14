using System.Runtime.InteropServices;
using Moq;
using SharpRacer.Telemetry.TestUtilities;

namespace SharpRacer.Telemetry;

public class ArrayTelemetryVariableTests
{
    [Fact]
    public void Ctor_VariableInfo_Test()
    {
        var variableInfo = TelemetryVariableInfoFactory.CreateArray("Foo", TelemetryVariableValueType.Float, 8);

        var variable = new ArrayTelemetryVariable<float>(variableInfo);

        TelemetryVariableAssert.IsAvailable(variable);
        TelemetryVariableAssert.MatchesVariableInfo(variable, variableInfo);
    }

    [Fact]
    public void Ctor_Descriptor_AvailableVariableTest()
    {
        var variableInfo = TelemetryVariableInfoFactory.CreateArray("Foo", TelemetryVariableValueType.Float, 8);
        var variableDescriptor = new TelemetryVariableDescriptor(variableInfo.Name, variableInfo.ValueType, variableInfo.ValueCount);

        var variable = new ArrayTelemetryVariable<float>(variableDescriptor, variableInfo);

        TelemetryVariableAssert.IsAvailable(variable);
        TelemetryVariableAssert.MatchesVariableInfo(variable, variableInfo);
    }

    [Fact]
    public void Ctor_Descriptor_UnavailableVariableTest()
    {
        var variableInfo = TelemetryVariableInfoFactory.CreateArray("Foo", TelemetryVariableValueType.Float, 8);
        var variableDescriptor = new TelemetryVariableDescriptor(variableInfo.Name, variableInfo.ValueType, variableInfo.ValueCount);

        var variable = new ArrayTelemetryVariable<float>(variableDescriptor, variableInfo: null);

        TelemetryVariableAssert.IsUnavailable(variable);
        TelemetryVariableAssert.MatchesVariableDescriptor(variable, variableDescriptor);
    }

    [Fact]
    public void Ctor_Descriptor_ThrowOnNullDescriptorTest()
    {
        var variableInfo = TelemetryVariableInfoFactory.CreateArray("Foo", TelemetryVariableValueType.Float, 8);
        TelemetryVariableDescriptor variableDescriptor = null!;

        Assert.Throws<ArgumentNullException>(() => new ArrayTelemetryVariable<float>(variableDescriptor, variableInfo));
    }

    [Fact]
    public void Ctor_Descriptor_WithProviderTest()
    {
        int valueCount = 3;
        var variableInfo = TelemetryVariableInfoFactory.CreateArray("Foo", TelemetryVariableValueType.Int, valueCount);
        var variableDescriptor = new TelemetryVariableDescriptor(variableInfo.Name, variableInfo.ValueType, variableInfo.ValueCount);

        var mocks = new MockRepository(MockBehavior.Strict);

        var variableInfoProviderMock = mocks.Create<ITelemetryVariableInfoProvider>();

        variableInfoProviderMock.Setup(x => x.NotifyTelemetryVariableActivated(It.IsAny<string>(), It.IsAny<Action<TelemetryVariableInfo>>()));

        var variable = new ArrayTelemetryVariable<int>(variableDescriptor, variableInfoProviderMock.Object);

        Assert.False(variable.IsAvailable);
        Assert.Null(variable.VariableInfo);
        Assert.Equal(variableInfo.Name, variable.Name);
        Assert.Equal(-1, variable.DataOffset);
        Assert.Equal(variableInfo.ValueCount, variable.ValueCount);
        Assert.Equal(variableInfo.ValueSize, variable.ValueSize);
        Assert.Equal(variableInfo.ValueSize * variableInfo.ValueCount, variable.DataLength);

        variableInfoProviderMock.Verify(
            x => x.NotifyTelemetryVariableActivated(variableInfo.Name, It.IsAny<Action<TelemetryVariableInfo>>()), Times.Once());
    }

    [Fact]
    public void Ctor_NameLengthVariableInfo_Test()
    {
        const string variableName = "Foo";
        const TelemetryVariableValueType valueType = TelemetryVariableValueType.Float;
        const int valueCount = 3;

        var variableInfo = TelemetryVariableInfoFactory.CreateArray(variableName, valueType, valueCount, false, 1024);

        var result = new ArrayTelemetryVariable<float>(variableName, valueCount, variableInfo);

        Assert.Equal(variableName, result.Name);
        Assert.Equal(valueCount, result.ValueCount);
        Assert.Equal(variableInfo, result.VariableInfo);
        Assert.True(result.IsAvailable);
    }

    [Fact]
    public void Ctor_NameLengthVariableInfo_NullVariableInfoTest()
    {
        const string variableName = "Foo";
        const int valueCount = 3;

        var result = new ArrayTelemetryVariable<float>(variableName, valueCount, variableInfo: null);

        Assert.Equal(variableName, result.Name);
        Assert.Equal(valueCount, result.ValueCount);
        Assert.Null(result.VariableInfo);
        Assert.False(result.IsAvailable);
    }

    [Fact]
    public void Ctor_NameLengthVariableInfo_ThrowsOnInvalidArrayLengthTest()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ArrayTelemetryVariable<int>("Foo", arrayLength: 0, variableInfo: null));
        Assert.Throws<ArgumentOutOfRangeException>(() => new ArrayTelemetryVariable<int>("Foo", arrayLength: -1, variableInfo: null));
    }

    [Fact]
    public void GetDataSpan_Test()
    {
        var variableInfo = TelemetryVariableInfoFactory.CreateArray("Foo", TelemetryVariableValueType.Int, 3, false, 12);
        var variable = new ArrayTelemetryVariable<int>(variableInfo);

        // Build a data frame
        Span<byte> dataFrame = new byte[128];

        var variableValues = new int[] { 2, 3, 4 };
        Span<byte> variableValueBytes = MemoryMarshal.AsBytes((Span<int>)variableValues);

        variableValueBytes.CopyTo(dataFrame.Slice(variableInfo.Offset));

        var dataSpan = variable.GetDataSpan(dataFrame);

        Assert.Equal(variableValueBytes.Length, dataSpan.Length);
        Assert.True(dataSpan.SequenceEqual(variableValueBytes));
    }

    [Fact]
    public void GetDataSpan_ThrowsOnUnavailableTest()
    {
        var variable = new ArrayTelemetryVariable<float>("Bar", 4, variableInfo: null);

        var data = new byte[512];

        Assert.Throws<TelemetryVariableUnavailableException>(() => variable.GetDataSpan(data));
    }

    [Fact]
    public void Read_Test()
    {
        var variableInfo = TelemetryVariableInfoFactory.CreateArray("Foo", TelemetryVariableValueType.Int, 3, false, 12);
        var variable = new ArrayTelemetryVariable<int>(variableInfo);

        var variableValues = new int[] { 2, 3, 4 };

        Span<byte> dataFrame = new byte[128];

        MemoryMarshal.AsBytes<int>(variableValues).CopyTo(dataFrame.Slice(variableInfo.Offset));

        var readValues = variable.Read(dataFrame);

        Assert.True(readValues.SequenceEqual(variableValues));
    }

    [Fact]
    public void Read_ThrowsOnUnavailableTest()
    {
        var variable = new ArrayTelemetryVariable<float>("Bar", 4, variableInfo: null);

        var data = new byte[1024];
        Assert.Throws<TelemetryVariableUnavailableException>(() => variable.Read(data));
    }
}
