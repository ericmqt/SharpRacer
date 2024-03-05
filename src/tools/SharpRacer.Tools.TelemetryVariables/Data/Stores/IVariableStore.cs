using SharpRacer.Tools.TelemetryVariables.Data.Entities;

namespace SharpRacer.Tools.TelemetryVariables.Data.Stores;
public interface IVariableStore : IEntityStore<VariableEntity>
{
    Task<VariableEntity?> FindByNameAsync(string variableName, CancellationToken cancellationToken = default);
    Task<List<VariableEntity>> ListCarDependentVariablesAsync(CancellationToken cancellationToken = default);
    Task<List<VariableEntity>> ListSessionVariablesAsync(CancellationToken cancellationToken = default);
    Task<VariableEntity> SetDeprecatedAsync(VariableEntity entity, bool isDeprecated, CancellationToken cancellationToken = default);
    Task<VariableEntity> SetDeprecatingVariableAsync(VariableEntity entity, VariableEntity? deprecatingVariable, CancellationToken cancellationToken = default);
}
