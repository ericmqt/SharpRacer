using SharpRacer.Telemetry;

namespace SharpRacer.Testing.Telemetry;

public class FakeTelemetryVariables<TFakeHeaders>
    where TFakeHeaders : FakeTelemetryVariableHeaders
{
    private readonly List<TelemetryVariableInfo> _variables;

    public FakeTelemetryVariables(TFakeHeaders headers)
    {
        var builder = new TelemetryVariableInfoCollectionBuilder();

        IsOnTrack = builder.Add(headers.IsOnTrack);
        PlayerCarPowerAdjust = builder.Add(headers.PlayerCarPowerAdjust);
        SessionFlags = builder.Add(headers.SessionFlags);
        SessionState = builder.Add(headers.SessionState);
        SessionTick = builder.Add(headers.SessionTick);
        SessionTime = builder.Add(headers.SessionTime);

        ConfigureAdditionalVariables(headers, builder);

        _variables = builder.Build();
        BufferSize = _variables.Sum(x => x.ValueSize * x.ValueCount);
    }

    public int BufferSize { get; protected set; }
    public IEnumerable<TelemetryVariableInfo> Variables => _variables;

    public TelemetryVariableInfo IsOnTrack { get; }
    public TelemetryVariableInfo PlayerCarPowerAdjust { get; }
    public TelemetryVariableInfo SessionFlags { get; }
    public TelemetryVariableInfo SessionState { get; }
    public TelemetryVariableInfo SessionTick { get; }
    public TelemetryVariableInfo SessionTime { get; }

    protected virtual void ConfigureAdditionalVariables(TFakeHeaders headers, TelemetryVariableInfoCollectionBuilder builder)
    {

    }
}
