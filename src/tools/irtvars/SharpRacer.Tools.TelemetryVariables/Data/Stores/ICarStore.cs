using SharpRacer.Tools.TelemetryVariables.Data.Entities;

namespace SharpRacer.Tools.TelemetryVariables.Data.Stores;

public interface ICarStore : IEntityStore<CarEntity>
{
    Task<CarEntity> AddVariableAsync(CarEntity carEntity, VariableEntity variableEntity, CancellationToken cancellationToken = default);
    Task<bool> ExistsByPathAsync(string path, CancellationToken cancellationToken = default);
    Task<CarEntity?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<CarEntity?> FindByPathAsync(string path, CancellationToken cancellationToken = default);
    Task<IEnumerable<VariableEntity>> GetVariablesAsync(CarEntity car, CancellationToken cancellationToken = default);
}
