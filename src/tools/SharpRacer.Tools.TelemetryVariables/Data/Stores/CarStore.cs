using Microsoft.EntityFrameworkCore;
using SharpRacer.Tools.TelemetryVariables.Data.Entities;

namespace SharpRacer.Tools.TelemetryVariables.Data.Stores;

internal sealed class CarStore : EntityStore<CarEntity>, ICarStore
{
    public CarStore(TelemetryVariablesDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<CarEntity> AddVariableAsync(CarEntity carEntity, VariableEntity variableEntity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(carEntity);
        ArgumentNullException.ThrowIfNull(variableEntity);

        if (carEntity.Variables.Any(x => x.Name == variableEntity.Name))
        {
            throw new ArgumentException($"Car '{carEntity.Name}' already has a variable '{variableEntity.Name}'.", nameof(carEntity));
        }

        carEntity.Variables.Add(variableEntity);

        return UpdateAsync(carEntity, false, cancellationToken);
    }

    public Task<bool> ExistsByPathAsync(string path, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        return EntitySet.AnyAsync(x => x.Path == path, cancellationToken);
    }

    public Task<CarEntity?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        return EntitySet
            .Include(x => x.Variables)
            .SingleOrDefaultAsync(x => x.Name == name, cancellationToken);
    }

    public Task<CarEntity?> FindByPathAsync(string path, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        return EntitySet
            .Include(x => x.Variables)
            .SingleOrDefaultAsync(x => x.NormalizedPath == path.ToUpperInvariant(), cancellationToken);
    }

    public async Task<IEnumerable<VariableEntity>> GetVariablesAsync(CarEntity car, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(car);

        var results = await DbContext.Set<CarVariableEntity>()
            .Where(x => x.Car == car)
            .Select(x => x.Variable)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return results;
    }
}
