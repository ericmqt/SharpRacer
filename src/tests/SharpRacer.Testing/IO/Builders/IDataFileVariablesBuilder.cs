using SharpRacer.Telemetry;

namespace SharpRacer.Testing.IO.Builders;

public interface IDataFileVariablesBuilder
{
    IDataFileVariablesBuilder Add(TelemetryVariableDescriptor descriptor);

    IDataFileVariablesBuilder AddArray<T>(
        string name,
        int valueCount,
        bool isTimeSliceArray = false,
        string? description = "An array variable",
        string? unit = null)
        where T : unmanaged;

    IDataFileVariablesBuilder AddScalar<T>(string name, string? description = null, string? unit = null)
        where T : unmanaged;
}
