namespace SharpRacer.Telemetry.TestVariables;

internal class BarVariable : ArrayDataVariable<float>
{
    private static readonly int _ArrayLength = 3;
    private static readonly string _VariableName = "Bar";

    public BarVariable()
        : base(_VariableName, _ArrayLength, variableInfo: null)
    {

    }

    public BarVariable(DataVariableInfo dataVariableInfo)
        : base(_VariableName, _ArrayLength, dataVariableInfo)
    {

    }

    public static BarVariable Create(DataVariableInfo dataVariableInfo)
    {
        return new BarVariable(dataVariableInfo);
    }
}
