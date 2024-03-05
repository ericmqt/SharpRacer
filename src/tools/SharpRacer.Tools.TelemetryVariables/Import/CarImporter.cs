using Microsoft.Extensions.Logging;
using SharpRacer.Tools.TelemetryVariables.Data.Entities;
using SharpRacer.Tools.TelemetryVariables.Data.Stores;
using SharpRacer.Tools.TelemetryVariables.Models;

namespace SharpRacer.Tools.TelemetryVariables.Import;
internal class CarImporter
{
    private readonly ICarStore _carStore;
    private readonly ICarVariableStore _carVariableStore;
    private readonly ILogger<CarImporter> _logger;
    private readonly IVariableStore _variableStore;

    public CarImporter(
        ICarStore carStore,
        IVariableStore variableStore,
        ICarVariableStore carVariableStore,
        ILogger<CarImporter> logger)
    {
        _carStore = carStore ?? throw new ArgumentNullException(nameof(carStore));
        _variableStore = variableStore ?? throw new ArgumentNullException(nameof(variableStore));
        _carVariableStore = carVariableStore ?? throw new ArgumentNullException(nameof(carVariableStore));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ImportResult<CarModel, CarEntity>> ImportAsync(CarModel model, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);

        var carVariables = new List<VariableEntity>();
        var missingVariableErrors = new List<string>();

        // Get all of the variables ahead of time to make sure they exist before we add anything to the database
        foreach (var carVariableName in model.VariableNames)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var variableEntity = await _variableStore.FindByNameAsync(carVariableName, cancellationToken).ConfigureAwait(false);

            if (variableEntity != null)
            {
                carVariables.Add(variableEntity);
            }
            else
            {
                _logger.LogWarning("Car '{CarName}' requires a variable '{CarVariableName}' which does not exist in the database.",
                    model.Name,
                    carVariableName);

                missingVariableErrors.Add($"Variable '{carVariableName}' does not exist in the database.");
            }
        }

        if (missingVariableErrors.Count != 0)
        {
            return ImportResult.Error<CarModel, CarEntity>(model, null, missingVariableErrors);
        }

        var entity = await _carStore.FindByPathAsync(model.Path, cancellationToken).ConfigureAwait(false);

        if (entity != null)
        {
            return await UpdateExistingAsync(entity, model, carVariables, cancellationToken).ConfigureAwait(false);
        }

        return await CreateAsync(model, carVariables, cancellationToken).ConfigureAwait(false);
    }

    private async Task<ImportResult<CarModel, CarEntity>> CreateAsync(
        CarModel model,
        IEnumerable<VariableEntity> variables,
        CancellationToken cancellationToken = default)
    {
        var entity = new CarEntity()
        {
            ContentVersion = model.ContentVersion,
            Name = model.Name,
            Path = model.Path,
            ShortName = model.ShortName
        };

        foreach (var variable in variables)
        {
            entity.Variables.Add(variable);
        }

        try
        {
            entity = await _carStore.CreateAsync(entity, true, cancellationToken).ConfigureAwait(false);

            _logger.LogDebug("Added car '{CarName}' (Path: {CarPath})", model.Name, model.Path);

            return ImportResult.Added(model, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding car {CarName}' (Path: {CarPath})", model.Name, model.Path);

            return ImportResult.Error(model, entity, [ex.Message]);
        }
    }

    private async Task<ImportResult<CarModel, CarEntity>> UpdateExistingAsync(
        CarEntity entity,
        CarModel model,
        IEnumerable<VariableEntity> variables,
        CancellationToken cancellationToken = default)
    {
        if (entity.ContentVersion >= model.ContentVersion)
        {
            return ImportResult.Exists(model, entity);
        }

        entity.ContentVersion = model.ContentVersion;
        entity.Name = model.Name;
        entity.ShortName = model.ShortName;

        // Ensure car has its variables
        foreach (var variable in variables)
        {
            if (!await _carVariableStore.ExistsAsync(entity, variable, cancellationToken).ConfigureAwait(false))
            {
                entity.Variables.Add(variable);

                _logger.LogInformation("{CarName}: Added new variable '{VariableName}'", model.Name, variable.Name);
            }
        }

        entity = await _carStore.UpdateAsync(entity, true, cancellationToken).ConfigureAwait(false);

        _logger.LogDebug("Updated car '{CarName}' (Path: {CarPath})", model.Name, model.Path);

        return ImportResult.Updated(model, entity);
    }
}
