namespace SharpRacer.Tools.TelemetryVariables.Data.Entities;
public class CarEntity
{
    private string _path = string.Empty;

    public CarEntity()
    {

    }

    public ContentVersion ContentVersion { get; set; }

    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string NormalizedPath { get; private set; } = default!;

    public string Path
    {
        get => _path;
        set
        {
            ArgumentNullException.ThrowIfNull(value);

            _path = value;
            NormalizedPath = value.ToUpperInvariant();
        }
    }

    public string ShortName { get; set; } = string.Empty;

    public List<VariableEntity> Variables { get; set; } = [];
}
