using SharpRacer.Tools.TelemetryVariables.Data.Entities;

namespace SharpRacer.Tools.TelemetryVariables.Data;
public interface ICarManager
{
    Task<CarEntity> AddVariableAsync(CarEntity carEntity, VariableEntity variableEntity, CancellationToken cancellationToken = default);
    Task<CarEntity> CreateCarAsync(string path, string name, string shortName, CancellationToken cancellationToken = default);
    Task<CarEntity?> FindCarByPathAsync(string path, CancellationToken cancellationToken = default);
    Task<bool> HasVariableAsync(CarEntity carEntity, VariableEntity variableEntity, CancellationToken cancellationToken = default);
    Task<IEnumerable<VariableEntity>> GetVariablesAsync(CarEntity entity, CancellationToken cancellationToken = default);
    Task<IEnumerable<VariableEntity>> GetVariablesByCarNameAsync(string carName, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetVariableNamesAsync(CarEntity entity, CancellationToken cancellationToken = default);
    Task<List<CarEntity>> ListAsync(CancellationToken cancellationToken = default);
}
