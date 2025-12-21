namespace SharpRacer.Telemetry.TestUtilities;

internal static class TelemetryVariableAssert
{
    public static void IsAvailable(ITelemetryVariable variable)
    {
        Assert.True(variable.IsAvailable);
        Assert.NotNull(variable.VariableInfo);
    }

    public static void IsUnavailable(ITelemetryVariable variable)
    {
        Assert.False(variable.IsAvailable);
        Assert.Null(variable.VariableInfo);
    }

    public static void MatchesVariableDescriptor<T>(IArrayTelemetryVariable<T> variable, TelemetryVariableDescriptor variableDescriptor)
        where T : unmanaged
    {
        Assert.Equal(variableDescriptor.Name, variable.Name);
        Assert.Equal(variableDescriptor.ValueCount, variable.ValueCount);
    }

    public static void MatchesVariableInfo<T>(IArrayTelemetryVariable<T> variable, TelemetryVariableInfo variableInfo)
        where T : unmanaged
    {
        Assert.Equal(variableInfo, variable.VariableInfo);

        Assert.Equal(variableInfo.Name, variable.Name);

        Assert.Equal(variableInfo.ValueCount, variable.ValueCount);
        Assert.Equal(variableInfo.ValueSize, variable.ValueSize);

        Assert.Equal(variableInfo.Offset, variable.DataOffset);
        Assert.Equal(variableInfo.ValueSize * variableInfo.ValueCount, variable.DataLength);
    }

    public static void MatchesVariableInfo<T>(IScalarTelemetryVariable<T> variable, TelemetryVariableInfo variableInfo)
        where T : unmanaged
    {
        Assert.Equal(variableInfo, variable.VariableInfo);

        Assert.Equal(variableInfo.Name, variable.Name);

        Assert.Equal(variableInfo.ValueSize, variable.ValueSize);

        Assert.Equal(variableInfo.Offset, variable.DataOffset);
        Assert.Equal(variableInfo.ValueSize * variableInfo.ValueCount, variable.DataLength);
    }
}
