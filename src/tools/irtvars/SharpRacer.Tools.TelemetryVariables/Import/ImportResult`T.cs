using System.Diagnostics.CodeAnalysis;

namespace SharpRacer.Tools.TelemetryVariables.Import;
internal class ImportResult<TModel, TEntity> : ImportResult
    where TModel : class
    where TEntity : class
{
    public ImportResult(TModel model, TEntity? entity, ImportState importState, IEnumerable<string>? errorMessages)
        : base(importState, errorMessages)
    {
        Model = model ?? throw new ArgumentNullException(nameof(model));
        Entity = entity;
    }

    [MemberNotNullWhen(true, nameof(Succeeded))]
    public TEntity? Entity { get; }

    public TModel Model { get; }

    public bool Succeeded => State != ImportState.Error;
}
