namespace SharpRacer.Tools.TelemetryVariables.Data.Entities;
public class CarVariableEntity
{
    public virtual CarEntity Car { get; set; } = default!;
    public int CarKey { get; set; }
    public virtual VariableEntity Variable { get; set; } = default!;
    public int VariableKey { get; set; }
}
