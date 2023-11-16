namespace SharpRacer.Telemetry.Variables;
internal static class DataVariableAssert
{
    public static void IsAvailable(IDataVariable dataVariable)
    {
        Assert.True(dataVariable.IsAvailable);
        Assert.NotNull(dataVariable.VariableInfo);
    }

    public static void IsUnavailable(IDataVariable dataVariable)
    {
        Assert.False(dataVariable.IsAvailable);
        Assert.Null(dataVariable.VariableInfo);
    }

    public static void MatchesVariableDescriptor<T>(IArrayDataVariable<T> dataVariable, DataVariableDescriptor variableDescriptor)
        where T : unmanaged
    {
        Assert.Equal(variableDescriptor.Name, dataVariable.Name);
        Assert.Equal(variableDescriptor.ValueCount, dataVariable.ValueCount);
    }

    public static void MatchesVariableInfo<T>(IArrayDataVariable<T> dataVariable, DataVariableInfo variableInfo)
        where T : unmanaged
    {
        Assert.Equal(variableInfo, dataVariable.VariableInfo);

        Assert.Equal(variableInfo.Name, dataVariable.Name);

        Assert.Equal(variableInfo.ValueCount, dataVariable.ValueCount);
        Assert.Equal(variableInfo.ValueSize, dataVariable.ValueSize);

        Assert.Equal(variableInfo.Offset, dataVariable.DataOffset);
        Assert.Equal(variableInfo.ValueSize * variableInfo.ValueCount, dataVariable.DataLength);
    }

    public static void MatchesVariableInfo<T>(IScalarDataVariable<T> dataVariable, DataVariableInfo variableInfo)
        where T : unmanaged
    {
        Assert.Equal(variableInfo, dataVariable.VariableInfo);

        Assert.Equal(variableInfo.Name, dataVariable.Name);

        Assert.Equal(variableInfo.ValueSize, dataVariable.ValueSize);

        Assert.Equal(variableInfo.Offset, dataVariable.DataOffset);
        Assert.Equal(variableInfo.ValueSize * variableInfo.ValueCount, dataVariable.DataLength);
    }
}
