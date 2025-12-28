namespace SharpRacer.Testing.Telemetry;

public sealed class FakeTelemetryVariables : FakeTelemetryVariables<FakeTelemetryVariableHeaders>
{
    public FakeTelemetryVariables()
        : this(new FakeTelemetryVariableHeaders())
    {

    }

    public FakeTelemetryVariables(FakeTelemetryVariableHeaders headers)
        : base(headers)
    {

    }
}
