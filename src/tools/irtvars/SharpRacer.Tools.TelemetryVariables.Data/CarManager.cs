using SharpRacer.Tools.TelemetryVariables.Data.Entities;
using SharpRacer.Tools.TelemetryVariables.Data.Stores;

namespace SharpRacer.Tools.TelemetryVariables.Data;
internal class CarManager : ICarManager
{
    private readonly ICarStore _carStore;
    private readonly ICarVariableStore _carVariableStore;

    public CarManager(
        ICarStore carStore,
        ICarVariableStore carVariableStore)
    {
        _carStore = carStore ?? throw new ArgumentNullException(nameof(carStore));
        _carVariableStore = carVariableStore ?? throw new ArgumentNullException(nameof(carVariableStore));
    }

    public Task<CarEntity> AddVariableAsync(CarEntity carEntity, VariableEntity variableEntity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(carEntity);
        ArgumentNullException.ThrowIfNull(variableEntity);

        if (carEntity.Variables.Any(x => x.NormalizedName == variableEntity.NormalizedName))
        {
            throw new ArgumentException($"Car '{carEntity.Name}' already has a variable '{variableEntity.Name}'.", nameof(carEntity));
        }

        carEntity.Variables.Add(variableEntity);

        return UpdateAsync(carEntity, cancellationToken);
    }

    public Task<CarEntity> CreateCarAsync(string path, string name, string shortName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentException.ThrowIfNullOrEmpty(shortName);

        var entity = new CarEntity()
        {
            Name = name,
            Path = path,
            ShortName = shortName
        };

        return CreateAsync(entity, cancellationToken);
    }

    public Task<CarEntity?> FindCarByPathAsync(string path, CancellationToken cancellationToken = default)
    {
        return _carStore.FindByPathAsync(path, cancellationToken);
    }

    public Task<bool> HasVariableAsync(CarEntity carEntity, VariableEntity variableEntity, CancellationToken cancellationToken = default)
    {
        return _carVariableStore.ExistsAsync(carEntity, variableEntity, cancellationToken);
    }

    public Task<IEnumerable<VariableEntity>> GetVariablesAsync(CarEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return _carVariableStore.GetVariablesAsync(entity, cancellationToken);
    }

    public async Task<IEnumerable<VariableEntity>> GetVariablesByCarNameAsync(string carName, CancellationToken cancellationToken = default)
    {
        var carEntity = await _carStore.FindByNameAsync(carName, cancellationToken).ConfigureAwait(false);

        if (carEntity is null)
        {
            throw new ArgumentException($"{nameof(CarEntity)} with name '{carName}' does not exist.", nameof(carName));
        }

        return await _carStore.GetVariablesAsync(carEntity, cancellationToken).ConfigureAwait(false);
    }

    public Task<IEnumerable<string>> GetVariableNamesAsync(CarEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return _carVariableStore.GetVariableNamesAsync(entity, cancellationToken);
    }

    public Task<List<CarEntity>> ListAsync(CancellationToken cancellationToken = default)
    {
        return _carStore.ListAsync(cancellationToken);
    }

    private Task<CarEntity> CreateAsync(CarEntity entity, CancellationToken cancellationToken = default)
    {
        return _carStore.CreateAsync(entity, saveChanges: true, cancellationToken: cancellationToken);
    }

    private Task<CarEntity> UpdateAsync(CarEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return _carStore.UpdateAsync(entity, saveChanges: true, cancellationToken: cancellationToken);
    }
}
