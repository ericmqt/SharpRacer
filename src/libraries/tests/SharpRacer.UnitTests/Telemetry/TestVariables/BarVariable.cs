namespace SharpRacer.Telemetry.TestVariables;

internal class BarVariable : ArrayTelemetryVariable<float>
{
    private static readonly int _ArrayLength = 3;
    private static readonly string _VariableName = "Bar";

    public BarVariable()
        : base(_VariableName, _ArrayLength, variableInfo: null)
    {

    }

    public BarVariable(TelemetryVariableInfo variableInfo)
        : base(_VariableName, _ArrayLength, variableInfo)
    {

    }

    public static BarVariable Create(TelemetryVariableInfo variableInfo)
    {
        return new BarVariable(variableInfo);
    }
}
