using SharpRacer.Telemetry.Variables;
using SharpRacer.Tools.TelemetryVariables.Data.Entities;

namespace SharpRacer.Tools.TelemetryVariables.Data;
public interface IVariableManager
{
    Task<VariableEntity> CreateVariableAsync(
        string name,
        string description,
        DataVariableValueType valueType,
        string? valueUnit,
        int valueCount,
        bool isTimeSliceArray,
        bool isDeprecated,
        CancellationToken cancellationToken = default);

    Task<VariableEntity?> FindByNameAsync(string variableName, CancellationToken cancellationToken = default);
    Task<IEnumerable<VariableEntity>> GetCarDependentVariablesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<VariableEntity>> GetSessionVariablesAsync(CancellationToken cancellationToken = default);

    Task<VariableEntity> SetDeprecatedAsync(
        VariableEntity variableEntity,
        VariableEntity? deprecatingVariable = null,
        CancellationToken cancellationToken = default);
}
