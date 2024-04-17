namespace SharpRacer.Telemetry.TestVariables;

internal static class TestDescriptors
{
    public static DataVariableDescriptor Baz { get; } = new DataVariableDescriptor("Baz", DataVariableValueType.Float, 3);
}

internal class BazVariable : ArrayDataVariable<float>, ICreateDataVariable<BazVariable>
{
    private static DataVariableDescriptor _Descriptor = TestDescriptors.Baz;

    public BazVariable()
        : base(_Descriptor, variableInfo: null)
    {

    }

    public BazVariable(DataVariableInfo? variableInfo)
        : base(_Descriptor, variableInfo)
    {
    }

    public static BazVariable Create(DataVariableInfo dataVariableInfo)
    {
        return new BazVariable(dataVariableInfo);
    }
}
