using Microsoft.Extensions.Logging;
using SharpRacer.Tools.TelemetryVariables.Data;
using SharpRacer.Tools.TelemetryVariables.Data.Entities;
using SharpRacer.Tools.TelemetryVariables.Models;

namespace SharpRacer.Tools.TelemetryVariables.Services;
internal class DataVariableImporter
{
    private static readonly IReadOnlyDictionary<string, string?> _deprecatedVariables = new Dictionary<string, string?>()
    {
        { "SessionLapsRemain", "SessionLapsRemainEx" },
        { "TrackTemp", "TrackTempCrew" },
        { "ShiftIndicatorPct", null }
    };

    private readonly ICarManager _carManager;
    private int _carsAdded;
    private readonly ILogger<DataVariableImporter> _logger;
    private int _variablesAdded;
    private readonly IVariableManager _variableManager;

    public DataVariableImporter(
        IVariableManager variableManager,
        ICarManager carManager,
        ILogger<DataVariableImporter> logger)
    {
        _variableManager = variableManager ?? throw new ArgumentNullException(nameof(variableManager));
        _carManager = carManager ?? throw new ArgumentNullException(nameof(carManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public int CarsAddedCount => _carsAdded;
    public int VariablesAddedCount => _variablesAdded;

    public async Task ImportAsync(IEnumerable<DataVariableModel> variables, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(variables);

        var variableEntities = new List<VariableEntity>();

        // Import variables
        foreach (var variableModel in variables)
        {
            var entity = await CreateOrGetVariableEntityAsync(variableModel, cancellationToken).ConfigureAwait(false);

            variableEntities.Add(entity);
        }

        // Update deprecated variables
        foreach (var entity in variableEntities)
        {
            if (!IsVariableDeprecated(entity.Name, out var deprecatingVariableName))
            {
                continue;
            }

            VariableEntity? deprecatingVariable = null;

            if (deprecatingVariableName != null)
            {
                deprecatingVariable = await _variableManager.FindByNameAsync(deprecatingVariableName, cancellationToken).ConfigureAwait(false);

                if (deprecatingVariable is null)
                {
                    _logger.LogWarning(
                        "Variable '{VariableName}' is deprecated by variable '{DeprecatingVariableName}', but the deprecating variable does not exist in the database.",
                        entity.Name,
                        deprecatingVariableName);
                }
            }

            await _variableManager.SetDeprecatedAsync(entity, deprecatingVariable, cancellationToken).ConfigureAwait(false);
        }
    }

    public async Task ImportCarAsync(CarModel carModel, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(carModel);

        var carVariables = new List<VariableEntity>();

        // Get all of the variables ahead of time to make sure they exist before we add anything to the database
        foreach (var carVariableName in carModel.VariableNames)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var variableEntity = await _variableManager.FindByNameAsync(carVariableName, cancellationToken).ConfigureAwait(false);

            if (variableEntity is null)
            {
                throw new ArgumentException(
                    $"'{nameof(carModel)}' requires a variable '{carVariableName}' but no matching variable was found in the database.",
                    nameof(carModel));
            }

            carVariables.Add(variableEntity);
        }

        // Create/get the car entity
        var carEntity = await CreateOrGetCarEntityAsync(carModel, cancellationToken).ConfigureAwait(false);

        // Associate variables with the car
        foreach (var variableEntity in carVariables)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!await _carManager.HasVariableAsync(carEntity, variableEntity, cancellationToken).ConfigureAwait(false))
            {
                await _carManager.AddVariableAsync(carEntity, variableEntity, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    private async Task<CarEntity> CreateOrGetCarEntityAsync(CarModel carModel, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(carModel);

        var entity = await _carManager.FindCarByPathAsync(carModel.Path, cancellationToken).ConfigureAwait(false);

        if (entity is null)
        {
            entity = await _carManager.CreateCarAsync(
                carModel.Path,
                carModel.Name,
                carModel.ShortName,
                cancellationToken)
                .ConfigureAwait(false);

            _logger.LogDebug($"Imported car: {entity.Name} (Path: {entity.Path})");
            _carsAdded++;
        }

        return entity;
    }

    private async Task<VariableEntity> CreateOrGetVariableEntityAsync(DataVariableModel variableModel, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(variableModel);

        var entity = await _variableManager.FindByNameAsync(variableModel.Name, cancellationToken).ConfigureAwait(false);

        if (entity is null)
        {
            entity = await _variableManager.CreateVariableAsync(
                variableModel.Name,
                variableModel.Description,
                variableModel.ValueType,
                variableModel.ValueUnit,
                variableModel.ValueCount,
                variableModel.IsTimeSliceArray,
                cancellationToken)
                .ConfigureAwait(false);

            _logger.LogDebug($"Imported variable: {entity.Name}");
            _variablesAdded++;
        }

        return entity;
    }

    private bool IsVariableDeprecated(string variableName, out string? deprecatingVariableName)
    {
        ArgumentException.ThrowIfNullOrEmpty(variableName);

        return _deprecatedVariables.TryGetValue(variableName, out deprecatingVariableName);
    }
}
