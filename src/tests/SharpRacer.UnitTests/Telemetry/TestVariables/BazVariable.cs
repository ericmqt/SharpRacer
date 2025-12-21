namespace SharpRacer.Telemetry.TestVariables;

internal static class TestDescriptors
{
    public static TelemetryVariableDescriptor Baz { get; } = new TelemetryVariableDescriptor("Baz", TelemetryVariableValueType.Float, 3);
}

internal class BazVariable : ArrayTelemetryVariable<float>
{
    private static TelemetryVariableDescriptor _Descriptor = TestDescriptors.Baz;

    public BazVariable()
        : base(_Descriptor, variableInfo: null)
    {

    }

    public BazVariable(TelemetryVariableInfo? variableInfo)
        : base(_Descriptor, variableInfo)
    {
    }
}
