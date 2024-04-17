using System.ComponentModel.DataAnnotations;
using SharpRacer.Interop;
using SharpRacer.Telemetry;

namespace SharpRacer.Tools.TelemetryVariables.Data.Entities;
public class VariableEntity
{
    private string _name = string.Empty;

    public VariableEntity()
    {

    }

    public virtual VariableEntity? DeprecatingVariable { get; set; }

    [MaxLength(DataFileConstants.MaxDescriptionLength)]
    public string Description { get; set; } = string.Empty;

    public int Id { get; set; }

    public bool IsDeprecated { get; set; }

    public bool IsTimeSliceArray { get; set; }

    [MaxLength(DataFileConstants.MaxStringLength)]
    public string Name
    {
        get => _name;
        set
        {
            ArgumentNullException.ThrowIfNull(value);

            _name = value;
            NormalizedName = value.ToUpperInvariant();
        }
    }

    [MaxLength(DataFileConstants.MaxStringLength)]
    public string NormalizedName { get; private set; } = string.Empty;

    public ContentVersion SimulatorVersion { get; set; }

    public int ValueCount { get; set; }
    public DataVariableValueType ValueType { get; set; }
    public string? ValueUnit { get; set; }
}
