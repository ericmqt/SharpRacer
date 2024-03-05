namespace SharpRacer.Tools.TelemetryVariables.Import;
internal abstract class ImportResult
{
    protected ImportResult(ImportState state, IEnumerable<string>? errorMessages)
    {
        State = state;

        ErrorMessages = errorMessages?.ToList() ?? [];
    }

    public IEnumerable<string> ErrorMessages { get; }
    public ImportState State { get; }

    public static ImportResult<TModel, TEntity> Added<TModel, TEntity>(TModel model, TEntity entity)
        where TModel : class
        where TEntity : class
    {
        return new ImportResult<TModel, TEntity>(model, entity, ImportState.Added, []);
    }

    public static ImportResult<TModel, TEntity> Error<TModel, TEntity>(TModel model, TEntity? entity, IEnumerable<string>? errorMessages)
        where TModel : class
        where TEntity : class
    {
        return new ImportResult<TModel, TEntity>(model, entity, ImportState.Error, errorMessages);
    }

    public static ImportResult<TModel, TEntity> Exists<TModel, TEntity>(TModel model, TEntity entity)
        where TModel : class
        where TEntity : class
    {
        return new ImportResult<TModel, TEntity>(model, entity, ImportState.Exists, []);
    }

    public static ImportResult<TModel, TEntity> Updated<TModel, TEntity>(TModel model, TEntity entity)
        where TModel : class
        where TEntity : class
    {
        return new ImportResult<TModel, TEntity>(model, entity, ImportState.Updated, []);
    }
}
