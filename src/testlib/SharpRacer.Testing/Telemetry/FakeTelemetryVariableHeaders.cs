using SharpRacer.Interop;

namespace SharpRacer.Testing.Telemetry;

public class FakeTelemetryVariableHeaders
{
    public FakeTelemetryVariableHeaders()
    {
        var builder = new TelemetryVariableHeaderCollectionBuilder();

        SessionTime = builder.AddScalar<double>("SessionTime", "Seconds since session start", "s");
        SessionTick = builder.AddScalar<int>("SessionTick", "Current update number");
        SessionState = builder.AddScalar<SessionState>("SessionState", "Session state", "irsdk_SessionState");
        SessionFlags = builder.AddScalar<RacingFlags>("SessionFlags", "Session flags", "irsdk_Flags");
        IsOnTrack = builder.AddScalar<bool>("IsOnTrack", "1=Car on track physics running with player in car");
        PlayerCarPowerAdjust = builder.AddScalar<float>("PlayerCarPowerAdjust", "Players power adjust", "%");

        ConfigureAdditionalVariables(builder);

        Headers = builder.Build();
        BufferSize = builder.BufferSize;
    }

    public int BufferSize { get; }
    public TelemetryVariableHeader[] Headers { get; }

    public TelemetryVariableHeader IsOnTrack { get; }
    public TelemetryVariableHeader PlayerCarPowerAdjust { get; }
    public TelemetryVariableHeader SessionFlags { get; }
    public TelemetryVariableHeader SessionState { get; }
    public TelemetryVariableHeader SessionTick { get; }
    public TelemetryVariableHeader SessionTime { get; }

    protected virtual void ConfigureAdditionalVariables(TelemetryVariableHeaderCollectionBuilder builder)
    {

    }
}
