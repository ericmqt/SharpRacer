namespace SharpRacer.Telemetry.TestUtilities;

internal static class TelemetryVariableAssert
{
    public static void IsAvailable(ITelemetryVariable dataVariable)
    {
        Assert.True(dataVariable.IsAvailable);
        Assert.NotNull(dataVariable.VariableInfo);
    }

    public static void IsUnavailable(ITelemetryVariable dataVariable)
    {
        Assert.False(dataVariable.IsAvailable);
        Assert.Null(dataVariable.VariableInfo);
    }

    public static void MatchesVariableDescriptor<T>(IArrayTelemetryVariable<T> dataVariable, TelemetryVariableDescriptor variableDescriptor)
        where T : unmanaged
    {
        Assert.Equal(variableDescriptor.Name, dataVariable.Name);
        Assert.Equal(variableDescriptor.ValueCount, dataVariable.ValueCount);
    }

    public static void MatchesVariableInfo<T>(IArrayTelemetryVariable<T> dataVariable, TelemetryVariableInfo variableInfo)
        where T : unmanaged
    {
        Assert.Equal(variableInfo, dataVariable.VariableInfo);

        Assert.Equal(variableInfo.Name, dataVariable.Name);

        Assert.Equal(variableInfo.ValueCount, dataVariable.ValueCount);
        Assert.Equal(variableInfo.ValueSize, dataVariable.ValueSize);

        Assert.Equal(variableInfo.Offset, dataVariable.DataOffset);
        Assert.Equal(variableInfo.ValueSize * variableInfo.ValueCount, dataVariable.DataLength);
    }

    public static void MatchesVariableInfo<T>(IScalarTelemetryVariable<T> dataVariable, TelemetryVariableInfo variableInfo)
        where T : unmanaged
    {
        Assert.Equal(variableInfo, dataVariable.VariableInfo);

        Assert.Equal(variableInfo.Name, dataVariable.Name);

        Assert.Equal(variableInfo.ValueSize, dataVariable.ValueSize);

        Assert.Equal(variableInfo.Offset, dataVariable.DataOffset);
        Assert.Equal(variableInfo.ValueSize * variableInfo.ValueCount, dataVariable.DataLength);
    }
}
