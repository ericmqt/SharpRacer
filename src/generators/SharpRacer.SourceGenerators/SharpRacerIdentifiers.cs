namespace SharpRacer.SourceGenerators;

public static class SharpRacerIdentifiers
{
    static SharpRacerIdentifiers()
    {
        SharpRacerNamespace = new NamespaceIdentifier("SharpRacer");
        TelemetryNamespace = new NamespaceIdentifier("SharpRacer.Telemetry");

        ArrayTelemetryVariable = TelemetryNamespace.CreateType("ArrayTelemetryVariable");
        TelemetryVariableDescriptor = TelemetryNamespace.CreateType("TelemetryVariableDescriptor");
        TelemetryVariableInfo = TelemetryNamespace.CreateType("TelemetryVariableInfo");
        TelemetryVariableInitializationException = TelemetryNamespace.CreateType("TelemetryVariableInitializationException");
        TelemetryVariableValueType = TelemetryNamespace.CreateType("TelemetryVariableValueType");
        GenerateTelemetryVariableDescriptorsAttribute = TelemetryNamespace.CreateType("GenerateTelemetryVariableDescriptorsAttribute");
        GenerateTelemetryVariablesContextAttribute = TelemetryNamespace.CreateType("GenerateTelemetryVariablesContextAttribute");
        IArrayTelemetryVariable = TelemetryNamespace.CreateType("IArrayTelemetryVariable");
        ITelemetryVariable = TelemetryNamespace.CreateType("ITelemetryVariable");
        ITelemetryVariablesContext = TelemetryNamespace.CreateType("ITelemetryVariablesContext");
        ITelemetryVariableInfoProvider = TelemetryNamespace.CreateType("ITelemetryVariableInfoProvider");
        IScalarTelemetryVariable = TelemetryNamespace.CreateType("IScalarTelemetryVariable");
        ScalarTelemetryVariable = TelemetryNamespace.CreateType("ScalarTelemetryVariable");

        CameraState = SharpRacerNamespace.CreateType("CameraState");
        CarLeftRight = SharpRacerNamespace.CreateType("CarLeftRight");
        EngineWarnings = SharpRacerNamespace.CreateType("EngineWarnings");
        PaceMode = SharpRacerNamespace.CreateType("PaceMode");
        PaceRacingFlags = SharpRacerNamespace.CreateType("PaceRacingFlags");
        PitServiceOptions = SharpRacerNamespace.CreateType("PitServiceOptions");
        PitServiceStatus = SharpRacerNamespace.CreateType("PitServiceStatus");
        RacingFlags = SharpRacerNamespace.CreateType("RacingFlags");
        SessionState = SharpRacerNamespace.CreateType("SessionState");
        TrackLocationType = SharpRacerNamespace.CreateType("TrackLocationType");
        TrackSurfaceType = SharpRacerNamespace.CreateType("TrackSurfaceType");
    }

    public static NamespaceIdentifier SharpRacerNamespace { get; }
    public static NamespaceIdentifier TelemetryNamespace { get; }

    public static TypeIdentifier ArrayTelemetryVariable { get; }
    public static TypeIdentifier CameraState { get; }
    public static TypeIdentifier CarLeftRight { get; }
    public static TypeIdentifier EngineWarnings { get; }
    public static TypeIdentifier GenerateTelemetryVariableDescriptorsAttribute { get; }
    public static TypeIdentifier GenerateTelemetryVariablesContextAttribute { get; }
    public static TypeIdentifier IArrayTelemetryVariable { get; }
    public static TypeIdentifier ITelemetryVariable { get; }
    public static TypeIdentifier ITelemetryVariablesContext { get; }
    public static TypeIdentifier ITelemetryVariableInfoProvider { get; }
    public static TypeIdentifier IScalarTelemetryVariable { get; }
    public static TypeIdentifier PaceMode { get; }
    public static TypeIdentifier PaceRacingFlags { get; }
    public static TypeIdentifier PitServiceOptions { get; }
    public static TypeIdentifier PitServiceStatus { get; }
    public static TypeIdentifier RacingFlags { get; }
    public static TypeIdentifier ScalarTelemetryVariable { get; }
    public static TypeIdentifier SessionState { get; }
    public static TypeIdentifier TrackLocationType { get; }
    public static TypeIdentifier TrackSurfaceType { get; }
    public static TypeIdentifier TelemetryVariableDescriptor { get; }
    public static TypeIdentifier TelemetryVariableInfo { get; }
    public static TypeIdentifier TelemetryVariableInitializationException { get; }
    public static TypeIdentifier TelemetryVariableValueType { get; }
}
