namespace SharpRacer.Telemetry;

/// <summary>
/// Indicates that the decorated class is a source generator target for generating a listing of <see cref="TelemetryVariableDescriptor"/> objects.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class GenerateTelemetryVariableDescriptorsAttribute : Attribute
{
}
