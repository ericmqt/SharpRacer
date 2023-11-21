namespace SharpRacer.Tools.TelemetryVariables.Data.Entities;

/// <summary>
/// Represents a <see cref="VariableEntity"/> that is available in a session regardless of car.
/// </summary>
public class SessionVariableEntity
{
    public virtual VariableEntity Variable { get; set; } = default!;
    public int VariableKey { get; set; }
}
