using Microsoft.EntityFrameworkCore;
using SharpRacer.Tools.TelemetryVariables.Data.Entities;

namespace SharpRacer.Tools.TelemetryVariables.Data.Stores;
internal sealed class VariableStore : EntityStore<VariableEntity>, IVariableStore
{
    public VariableStore(TelemetryVariablesDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<VariableEntity?> FindByNameAsync(string variableName, CancellationToken cancellationToken = default)
    {
        return EntitySet.SingleOrDefaultAsync(x => x.NormalizedName == variableName.ToUpperInvariant(), cancellationToken);
    }

    public async Task<List<VariableEntity>> ListCarDependentVariablesAsync(CancellationToken cancellationToken = default)
    {
        var allVariables = await ListAsync(cancellationToken).ConfigureAwait(false);
        var sessionVariables = await ListSessionVariablesAsync(cancellationToken).ConfigureAwait(false);

        return allVariables.ExceptBy(sessionVariables.Select(x => x.Id), x => x.Id).ToList();
    }

    public Task<List<VariableEntity>> ListSessionVariablesAsync(CancellationToken cancellationToken = default)
    {
        return DbContext.SessionVariables.Select(x => x.Variable).ToListAsync(cancellationToken);
    }

    public Task<VariableEntity> SetDeprecatedAsync(VariableEntity entity, bool isDeprecated, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        entity.IsDeprecated = isDeprecated;

        return UpdateAsync(entity, cancellationToken);
    }

    public Task<VariableEntity> SetDeprecatingVariableAsync(VariableEntity entity, VariableEntity? deprecatingVariable, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        entity.DeprecatingVariable = deprecatingVariable;

        return UpdateAsync(entity, cancellationToken);
    }
}
