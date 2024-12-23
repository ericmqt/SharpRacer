﻿namespace SharpRacer.Telemetry.TestVariables;

internal class FooVariable : ScalarDataVariable<int>
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
