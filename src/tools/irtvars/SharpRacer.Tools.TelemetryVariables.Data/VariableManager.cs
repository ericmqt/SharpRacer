using Microsoft.EntityFrameworkCore;
using SharpRacer.Telemetry.Variables;
using SharpRacer.Tools.TelemetryVariables.Data.Entities;
using SharpRacer.Tools.TelemetryVariables.Data.Stores;

namespace SharpRacer.Tools.TelemetryVariables.Data;
internal class VariableManager : IVariableManager
{
    private readonly TelemetryVariablesDbContext _dbContext;
    private readonly IVariableStore _variableStore;

    public VariableManager(
        IVariableStore variableStore,
        TelemetryVariablesDbContext dbContext)
    {
        _variableStore = variableStore ?? throw new ArgumentNullException(nameof(variableStore));
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public Task<VariableEntity> CreateVariableAsync(
        string name,
        string description,
        DataVariableValueType valueType,
        string? valueUnit,
        int valueCount,
        bool isTimeSliceArray,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentException.ThrowIfNullOrEmpty(description);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(valueCount);

        var entity = new VariableEntity
        {
            Description = description,
            IsTimeSliceArray = isTimeSliceArray,
            Name = name,
            ValueCount = valueCount,
            ValueType = valueType,
            ValueUnit = valueUnit,
        };

        return CreateAsync(entity, cancellationToken);
    }

    public Task<VariableEntity?> FindByNameAsync(string variableName, CancellationToken cancellationToken = default)
    {
        return _variableStore.FindByNameAsync(variableName, cancellationToken);
    }

    public async Task<IEnumerable<VariableEntity>> GetCarDependentVariablesAsync(CancellationToken cancellationToken = default)
    {
        var allVariables = await _variableStore.ListAsync(cancellationToken).ConfigureAwait(false);
        var sessionVariables = await GetSessionVariablesAsync(cancellationToken).ConfigureAwait(false);

        return allVariables.ExceptBy(sessionVariables.Select(x => x.Id), x => x.Id);
    }

    public async Task<IEnumerable<VariableEntity>> GetSessionVariablesAsync(CancellationToken cancellationToken = default)
    {
        var results = await _dbContext.SessionVariables
            .Select(x => x.Variable)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return results;
    }

    public async Task<VariableEntity> SetDeprecatedAsync(
        VariableEntity variableEntity,
        VariableEntity? deprecatingVariable = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(variableEntity);

        var entity = await _variableStore.SetDeprecatedAsync(variableEntity, true, cancellationToken).ConfigureAwait(false);

        if (deprecatingVariable != null)
        {
            entity = await _variableStore.SetDeprecatingVariableAsync(entity, deprecatingVariable, cancellationToken).ConfigureAwait(false);
        }

        return await UpdateAsync(entity, cancellationToken).ConfigureAwait(false);
    }

    private Task<VariableEntity> CreateAsync(VariableEntity entity, CancellationToken cancellationToken = default)
    {
        return _variableStore.CreateAsync(entity, saveChanges: true, cancellationToken: cancellationToken);
    }

    private Task<VariableEntity> UpdateAsync(VariableEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return _variableStore.UpdateAsync(entity, saveChanges: true, cancellationToken: cancellationToken);
    }
}
