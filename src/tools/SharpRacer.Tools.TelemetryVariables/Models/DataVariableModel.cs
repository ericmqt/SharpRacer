using SharpRacer.Telemetry;
using SharpRacer.Tools.TelemetryVariables.Data.Entities;

namespace SharpRacer.Tools.TelemetryVariables.Models;
public class DataVariableModel
{
    public DataVariableModel()
    {

    }

    public DataVariableModel(VariableEntity variableEntity)
    {
        ArgumentNullException.ThrowIfNull(variableEntity);

        Description = variableEntity.Description;
        IsDeprecated = variableEntity.IsDeprecated;
        IsTimeSliceArray = variableEntity.IsTimeSliceArray;
        Name = variableEntity.Name;
        SimulatorVersion = variableEntity.SimulatorVersion;
        ValueCount = variableEntity.ValueCount;
        ValueType = variableEntity.ValueType;
        ValueUnit = variableEntity.ValueUnit;

        if (variableEntity.DeprecatingVariable != null)
        {
            DeprecatedBy = variableEntity.DeprecatingVariable.Name;
        }
    }

    public DataVariableModel(TelemetryVariableInfo variableInfo, ContentVersion simulatorVersion)
    {
        ArgumentNullException.ThrowIfNull(variableInfo);

        Description = variableInfo.Description;
        IsTimeSliceArray = variableInfo.IsTimeSliceArray;
        Name = variableInfo.Name;
        SimulatorVersion = simulatorVersion;
        ValueCount = variableInfo.ValueCount;
        ValueType = variableInfo.ValueType;
        ValueUnit = variableInfo.ValueUnit;
    }

    public string? DeprecatedBy { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsDeprecated { get; set; }
    public bool IsTimeSliceArray { get; set; }
    public string Name { get; set; } = string.Empty;
    public ContentVersion SimulatorVersion { get; set; }
    public int ValueCount { get; set; }
    public TelemetryVariableValueType ValueType { get; set; }
    public string? ValueUnit { get; set; }
}
