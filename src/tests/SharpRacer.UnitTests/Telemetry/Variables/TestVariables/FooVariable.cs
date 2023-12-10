using SharpRacer.Telemetry.Variables;

namespace SharpRacer.Telemetry.Variables.TestVariables;

internal class FooVariable : ScalarDataVariable<int>, ICreateDataVariable<FooVariable>
{
    private static readonly string _VariableName = "Foo";

    public FooVariable()
        : base(_VariableName, null)
    {

    }

    public FooVariable(DataVariableInfo variableInfo)
        : base(_VariableName, variableInfo)
    {

    }

    public static FooVariable Create(DataVariableInfo dataVariableInfo)
    {
        return new FooVariable(dataVariableInfo);
    }
}
