using SharpRacer.Tools.TelemetryVariables.Data.Entities;

namespace SharpRacer.Tools.TelemetryVariables.Data.Stores;
public interface ICarVariableStore : IEntityStore<CarVariableEntity>
{
    Task<bool> ExistsAsync(CarEntity car, VariableEntity variable, CancellationToken cancellationToken = default);
    Task<IEnumerable<VariableEntity>> GetVariablesAsync(CarEntity car, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetVariableNamesAsync(CarEntity car, CancellationToken cancellationToken = default);
}
