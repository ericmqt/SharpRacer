using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Moq;

namespace SharpRacer.Telemetry;

public class ScalarTelemetryVariableTests
{
    [Fact]
    public void Ctor_VariableInfo_Test()
    {
        var variableInfo = TelemetryVariableInfoFactory.CreateScalar("Foo", TelemetryVariableValueType.Int);
        var variable = new ScalarTelemetryVariable<int>(variableInfo);

        Assert.Equal(variableInfo.Name, variable.Name);
        Assert.True(variable.IsAvailable);
        Assert.Equal(variableInfo, variable.VariableInfo);
        Assert.Equal(variableInfo.Offset, variable.DataOffset);
        Assert.Equal(1, variable.ValueCount);
    }

    [Fact]
    public void Ctor_Descriptor_AvailableVariableTest()
    {
        var variableInfo = TelemetryVariableInfoFactory.CreateScalar("Foo", TelemetryVariableValueType.Int);
        var variableDescriptor = new TelemetryVariableDescriptor(variableInfo.Name, variableInfo.ValueType, variableInfo.ValueCount);

        var variable = new ScalarTelemetryVariable<int>(variableDescriptor, variableInfo);

        Assert.True(variable.IsAvailable);
        Assert.NotNull(variable.VariableInfo);
        Assert.Equal(variableInfo, variable.VariableInfo);

        Assert.Equal(variableInfo.Name, variable.Name);
        Assert.Equal(variableInfo.Offset, variable.DataOffset);
        Assert.Equal(variableInfo.ValueCount, variable.ValueCount);
        Assert.Equal(variableInfo.ValueSize, variable.ValueSize);
        Assert.Equal(variableInfo.ValueSize, variable.DataLength);
    }

    [Fact]
    public void Ctor_Descriptor_UnavailableVariableTest()
    {
        var variableInfo = TelemetryVariableInfoFactory.CreateScalar("Foo", TelemetryVariableValueType.Int);
        var variableDescriptor = new TelemetryVariableDescriptor(variableInfo.Name, variableInfo.ValueType, variableInfo.ValueCount);

        var variable = new ScalarTelemetryVariable<int>(variableDescriptor, variableInfo: null);

        Assert.False(variable.IsAvailable);
        Assert.Null(variable.VariableInfo);

        Assert.Equal(variableDescriptor.Name, variable.Name);
        Assert.Equal(variableDescriptor.ValueCount, variable.ValueCount);
    }

    [Fact]
    public void Ctor_Descriptor_ThrowOnNullDescriptorTest()
    {
        var variableInfo = TelemetryVariableInfoFactory.CreateScalar("Foo", TelemetryVariableValueType.Int);
        TelemetryVariableDescriptor variableDescriptor = null!;

        Assert.Throws<ArgumentNullException>(() => new ScalarTelemetryVariable<int>(variableDescriptor, variableInfo));
    }

    [Fact]
    public void Ctor_Descriptor_WithProviderTest()
    {
        var variableInfo = TelemetryVariableInfoFactory.CreateScalar("Foo", TelemetryVariableValueType.Int);
        var variableDescriptor = new TelemetryVariableDescriptor(variableInfo.Name, variableInfo.ValueType, variableInfo.ValueCount);

        var mocks = new MockRepository(MockBehavior.Strict);

        var variableInfoProviderMock = mocks.Create<ITelemetryVariableInfoProvider>();

        variableInfoProviderMock.Setup(x => x.NotifyTelemetryVariableActivated(It.IsAny<string>(), It.IsAny<Action<TelemetryVariableInfo>>()));

        var variable = new ScalarTelemetryVariable<int>(variableDescriptor, variableInfoProviderMock.Object);

        Assert.False(variable.IsAvailable);
        Assert.Null(variable.VariableInfo);
        Assert.Equal(variableInfo.Name, variable.Name);
        Assert.Equal(-1, variable.DataOffset);
        Assert.Equal(variableInfo.ValueCount, variable.ValueCount);
        Assert.Equal(variableInfo.ValueSize, variable.ValueSize);
        Assert.Equal(variableInfo.ValueSize, variable.DataLength);

        variableInfoProviderMock.Verify(
            x => x.NotifyTelemetryVariableActivated(variableInfo.Name, It.IsAny<Action<TelemetryVariableInfo>>()), Times.Once());
    }

    [Fact]
    public void Ctor_Name_UnavailableTest()
    {
        var variable = new ScalarTelemetryVariable<int>("Foo", variableInfo: null);

        Assert.Equal("Foo", variable.Name);
        Assert.False(variable.IsAvailable);
        Assert.Null(variable.VariableInfo);

        Assert.Equal(Unsafe.SizeOf<int>(), variable.DataLength);
        Assert.Equal(-1, variable.DataOffset);
    }

    [Fact]
    public void Ctor_Name_NullOrEmptyNameTest()
    {
        Assert.Throws<ArgumentNullException>(() => new ScalarTelemetryVariable<float>(name: null!, variableInfo: null));
        Assert.Throws<ArgumentException>(() => new ScalarTelemetryVariable<float>(string.Empty, variableInfo: null));
    }

    [Fact]
    public void Ctor_VariableInfo_ThrowOnArrayVariableInfoTest()
    {
        var variableInfo = TelemetryVariableInfoFactory.CreateArray("Bar", TelemetryVariableValueType.Int, valueCount: 3);

        Assert.Throws<ArgumentException>(() => new ScalarTelemetryVariable<int>(variableInfo));
    }

    [Fact]
    public void Ctor_VariableInfo_ThrowOnNullVariableInfoTest()
    {
        Assert.Throws<ArgumentNullException>(() => new ScalarTelemetryVariable<float>(variableInfo: null!));
    }

    [Fact]
    public void GetDataSpan_Test()
    {
        var variableInfo = TelemetryVariableInfoFactory.CreateScalar("Foo", TelemetryVariableValueType.Int, offset: 8);
        var variable = new ScalarTelemetryVariable<int>(variableInfo);

        Span<byte> dataFrame = new byte[256];
        int variableValue = 37;
        var variableValueSlice = dataFrame.Slice(variableInfo.Offset, Unsafe.SizeOf<int>());

        MemoryMarshal.Write(variableValueSlice, variableValue);

        var dataSpan = variable.GetDataSpan(dataFrame);

        Assert.Equal(variableValueSlice.Length, dataSpan.Length);
        Assert.True(dataSpan.SequenceEqual(variableValueSlice));
    }

    [Fact]
    public void GetDataSpan_ThrowsOnUnavailableTest()
    {
        var variable = new ScalarTelemetryVariable<float>("Bar", variableInfo: null);

        var data = new byte[512];

        Assert.Throws<TelemetryVariableUnavailableException>(() => variable.GetDataSpan(data));
    }

    [Fact]
    public void Read_Test()
    {
        var variableInfo = TelemetryVariableInfoFactory.CreateScalar("Foo", TelemetryVariableValueType.Int, offset: 1020);
        var variable = new ScalarTelemetryVariable<int>(variableInfo);

        Span<byte> dataFrame = new byte[2048];
        int variableValue = 37;
        MemoryMarshal.Write(dataFrame.Slice(variableInfo.Offset, Unsafe.SizeOf<int>()), variableValue);

        Assert.Equal(variableValue, variable.Read(dataFrame));
    }

    [Fact]
    public void Read_ThrowsOnUnavailableTest()
    {
        var variable = new ScalarTelemetryVariable<float>("Bar", variableInfo: null);

        var data = new byte[1024];
        Assert.Throws<TelemetryVariableUnavailableException>(() => variable.Read(data));
    }

    [Fact]
    public void SetVariableInfo_Test()
    {
        var variableInfo = TelemetryVariableInfoFactory.CreateScalar("Foo", TelemetryVariableValueType.Int);
        var variableDescriptor = new TelemetryVariableDescriptor(variableInfo.Name, variableInfo.ValueType, variableInfo.ValueCount);

        var fakeProvider = new FakeDataVariableInfoProvider();

        var variable = new ScalarTelemetryVariable<int>(variableDescriptor, fakeProvider);

        Assert.False(variable.IsAvailable);
        Assert.Null(variable.VariableInfo);

        // Initialize variables to invoke the callback
        fakeProvider.InitializeVariables([variableInfo]);

        Assert.True(variable.IsAvailable);
        Assert.NotNull(variable.VariableInfo);
        Assert.Equal(variableInfo, variable.VariableInfo);

        Assert.Equal(variableInfo.Name, variable.Name);
        Assert.Equal(variableInfo.Offset, variable.DataOffset);
        Assert.Equal(variableInfo.ValueCount, variable.ValueCount);
        Assert.Equal(variableInfo.ValueSize, variable.ValueSize);
        Assert.Equal(variableInfo.ValueSize, variable.DataLength);
    }

    private class FakeDataVariableInfoProvider : ITelemetryVariableInfoProvider
    {
        private readonly Dictionary<string, ConcurrentQueue<Action<TelemetryVariableInfo>>> _callbacks = [];
        private bool _isInitialized;
        private readonly List<TelemetryVariableInfo> _variables = [];

        public FakeDataVariableInfoProvider()
        {

        }

        public IEnumerable<TelemetryVariableInfo> Variables => _variables;

        public void InitializeVariables(IEnumerable<TelemetryVariableInfo> variables)
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException("Telemetry variables have already been initialized.");
            }

            foreach (var variable in variables)
            {
                _variables.Add(variable);
            }

            // Invoke registered callbacks for active variables
            foreach (var variableName in _callbacks.Keys)
            {
                if (!_variables.TryFindByName(variableName, out var variableInfo))
                {
                    continue;
                }

                var callbackQueue = _callbacks[variableName];

                while (callbackQueue.TryDequeue(out var callback))
                {
                    try { callback(variableInfo); }
                    catch
                    {
                        // Swallow the exception. Do not allow a callback that throws an exception prevent executing remaining callbacks.
                    }
                }
            }

            _isInitialized = true;
        }

        public void NotifyTelemetryVariableActivated(string variableName, Action<TelemetryVariableInfo> callback)
        {
            ArgumentException.ThrowIfNullOrEmpty(variableName);
            ArgumentNullException.ThrowIfNull(callback);

            if (!_isInitialized)
            {
                if (!_callbacks.TryGetValue(variableName, out var callbackQueue))
                {
                    callbackQueue = new ConcurrentQueue<Action<TelemetryVariableInfo>>();

                    _callbacks.Add(variableName, callbackQueue);
                }

                callbackQueue.Enqueue(callback);
            }
            else
            {
                var variableInfo = _variables.FindByName(variableName);

                if (variableInfo != null)
                {
                    callback(variableInfo);
                }
            }
        }
    }
}
