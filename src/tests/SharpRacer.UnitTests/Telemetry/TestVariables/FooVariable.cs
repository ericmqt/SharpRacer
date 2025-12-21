namespace SharpRacer.Telemetry.TestVariables;

internal class FooVariable : ScalarTelemetryVariable<int>
{
    private static readonly string _VariableName = "Foo";

    public FooVariable()
        : base(_VariableName, null)
    {

    }

    public FooVariable(TelemetryVariableInfo variableInfo)
        : base(_VariableName, variableInfo)
    {

    }

    public static FooVariable Create(TelemetryVariableInfo dataVariableInfo)
    {
        return new FooVariable(dataVariableInfo);
    }
}
