using Microsoft.EntityFrameworkCore;
using SharpRacer.Tools.TelemetryVariables.Data.Entities;

namespace SharpRacer.Tools.TelemetryVariables.Data.Stores;
internal sealed class CarVariableStore : EntityStore<CarVariableEntity>, ICarVariableStore
{
    public CarVariableStore(TelemetryVariablesDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<CarVariableEntity> CreateAsync(CarEntity carEntity, VariableEntity variableEntity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(carEntity);
        ArgumentNullException.ThrowIfNull(variableEntity);

        var entity = new CarVariableEntity()
        {
            Car = carEntity,
            Variable = variableEntity,
        };

        return CreateAsync(entity, cancellationToken);
    }

    public Task<bool> ExistsAsync(CarEntity car, VariableEntity variable, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(car);
        ArgumentNullException.ThrowIfNull(variable);

        return EntitySet.AnyAsync(x => x.Car == car && x.Variable == variable, cancellationToken);
    }

    public async Task<IEnumerable<VariableEntity>> GetVariablesAsync(CarEntity car, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(car);

        var results = await EntitySet.Where(x => x.Car == car)
            .Select(x => x.Variable)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return results;
    }

    public async Task<IEnumerable<string>> GetVariableNamesAsync(CarEntity car, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(car);

        var results = await EntitySet.Where(x => x.Car == car)
            .Select(x => x.Variable.Name)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return results;
    }
}
