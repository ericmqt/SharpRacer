using Microsoft.Extensions.Logging;
using SharpRacer.Tools.TelemetryVariables.Data.Entities;
using SharpRacer.Tools.TelemetryVariables.Data.Stores;
using SharpRacer.Tools.TelemetryVariables.Models;

namespace SharpRacer.Tools.TelemetryVariables.Import;
internal class VariableImporter
{
    private readonly ILogger<VariableImporter> _logger;
    private readonly IVariableStore _variableStore;

    public VariableImporter(
        IVariableStore variableStore,
        ILogger<VariableImporter> logger)
    {
        _variableStore = variableStore ?? throw new ArgumentNullException(nameof(variableStore));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<ImportResult<DataVariableModel, VariableEntity>>> ImportVariablesAsync(
        IEnumerable<DataVariableModel> variables,
        CancellationToken cancellationToken = default)
    {
        var results = new List<ImportResult<DataVariableModel, VariableEntity>>();

        foreach (var model in variables)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var importResult = await ImportVariableAsync(model, cancellationToken).ConfigureAwait(false);

            results.Add(importResult);
        }

        foreach (var result in results.Where(x => x.Entity != null && IsDeprecated(x.Model) && x.Entity.DeprecatingVariable == null))
        {
            if (!DeprecatedVariables.TryGetDeprecatingVariableName(result.Model, out var deprecatingVariableName))
            {
                continue;
            }

            var deprecatingVariable = await _variableStore.FindByNameAsync(deprecatingVariableName, cancellationToken).ConfigureAwait(false);

            deprecatingVariable ??= results.FirstOrDefault(x => x.Model.Name.Equals(deprecatingVariableName, StringComparison.Ordinal))?.Entity;

            if (deprecatingVariable is null)
            {
                _logger.LogWarning(
                    "Variable '{VariableName}' is deprecated by variable '{DeprecatingVariableName}', but the deprecating variable does not exist in the database or import source.",
                    result.Model.Name,
                    deprecatingVariableName);

                continue;
            }

            result.Entity!.IsDeprecated = true;
            result.Entity!.DeprecatingVariable = deprecatingVariable;

            await _variableStore.UpdateAsync(result.Entity!, true, cancellationToken).ConfigureAwait(false);
        }

        return results;
    }

    private async Task<ImportResult<DataVariableModel, VariableEntity>> ImportVariableAsync(DataVariableModel variableModel, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(variableModel);

        var entity = await _variableStore.FindByNameAsync(variableModel.Name, cancellationToken).ConfigureAwait(false);

        if (entity != null)
        {
            // Check if update needed
            if (variableModel.SimulatorVersion > entity.SimulatorVersion)
            {
                entity = await UpdateAsync(entity, variableModel, cancellationToken).ConfigureAwait(false);

                return ImportResult.Updated(variableModel, entity);
            }

            return ImportResult.Exists(variableModel, entity);
        }

        entity = await CreateAsync(variableModel, cancellationToken).ConfigureAwait(false);

        return ImportResult.Added(variableModel, entity);
    }

    private Task<VariableEntity> CreateAsync(DataVariableModel variableModel, CancellationToken cancellationToken = default)
    {
        var entity = new VariableEntity
        {
            Description = variableModel.Description,
            IsTimeSliceArray = variableModel.IsTimeSliceArray,
            Name = variableModel.Name,
            ValueCount = variableModel.ValueCount,
            ValueType = variableModel.ValueType,
            ValueUnit = variableModel.ValueUnit,
            IsDeprecated = IsDeprecated(variableModel),
            SimulatorVersion = variableModel.SimulatorVersion
        };

        return _variableStore.CreateAsync(entity, true, cancellationToken);
    }

    private Task<VariableEntity> UpdateAsync(VariableEntity entity, DataVariableModel variableModel, CancellationToken cancellationToken = default)
    {
        entity.SimulatorVersion = variableModel.SimulatorVersion;

        entity.Description = variableModel.Description;
        entity.IsDeprecated = IsDeprecated(variableModel);
        entity.IsTimeSliceArray = variableModel.IsTimeSliceArray;
        entity.ValueCount = variableModel.ValueCount;
        entity.ValueUnit = variableModel.ValueUnit;
        entity.ValueType = variableModel.ValueType;

        return _variableStore.UpdateAsync(entity, true, cancellationToken);
    }

    private static bool IsDeprecated(DataVariableModel variableModel)
    {
        return variableModel.IsDeprecated || DeprecatedVariables.IsDeprecated(variableModel.Name);
    }

    private static bool IsDeprecated(DataVariableModel variableModel, VariableEntity entity)
    {
        return IsDeprecated(variableModel) || entity.IsDeprecated;
    }
}
