using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace SharpRacer.Tools.TelemetryVariables.Data.Stores;

internal abstract class EntityStore<TEntity> : IEntityStore<TEntity>
    where TEntity : class
{
    protected EntityStore(TelemetryVariablesDbContext dbContext)
    {
        DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public TelemetryVariablesDbContext DbContext { get; }
    protected DbSet<TEntity> EntitySet => DbContext.Set<TEntity>();

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return EntitySet.CountAsync(cancellationToken);
    }

    public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return EntitySet.CountAsync(predicate, cancellationToken);
    }

    public Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        return CreateAsync(entity, false, cancellationToken);
    }

    public async Task<TEntity> CreateAsync(TEntity entity, bool saveChanges, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        DbContext.Set<TEntity>().Add(entity);

        if (saveChanges)
        {
            await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        return entity;
    }

    public Task<List<TEntity>> ListAsync(CancellationToken cancellationToken = default)
    {
        return EntitySet.ToListAsync(cancellationToken);
    }

    public Task<List<TEntity>> ListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return EntitySet.Where(predicate).ToListAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return DbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        return UpdateAsync(entity, false, cancellationToken);
    }

    public async Task<TEntity> UpdateAsync(TEntity entity, bool saveChanges, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        DbContext.Set<TEntity>().Update(entity);

        if (saveChanges)
        {
            await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        return entity;
    }
}
