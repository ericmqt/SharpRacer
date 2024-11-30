namespace SharpRacer.SourceGenerators;
public static class SharpRacerIdentifiers
{
    static SharpRacerIdentifiers()
    {
        SharpRacerNamespace = new NamespaceIdentifier("SharpRacer");
        TelemetryNamespace = new NamespaceIdentifier("SharpRacer.Telemetry");

        ArrayDataVariable = TelemetryNamespace.CreateType("ArrayDataVariable");
        DataVariableDescriptor = TelemetryNamespace.CreateType("DataVariableDescriptor");
        DataVariableInfo = TelemetryNamespace.CreateType("DataVariableInfo");
        DataVariableInitializationException = TelemetryNamespace.CreateType("DataVariableInitializationException");
        DataVariableValueType = TelemetryNamespace.CreateType("DataVariableValueType");
        GenerateDataVariableDescriptorsAttribute = TelemetryNamespace.CreateType("GenerateDataVariableDescriptorsAttribute");
        GenerateDataVariablesContextAttribute = TelemetryNamespace.CreateType("GenerateDataVariablesContextAttribute");
        IArrayDataVariable = TelemetryNamespace.CreateType("IArrayDataVariable");
        IDataVariable = TelemetryNamespace.CreateType("IDataVariable");
        IDataVariablesContext = TelemetryNamespace.CreateType("IDataVariablesContext");
        IDataVariableInfoProvider = TelemetryNamespace.CreateType("IDataVariableInfoProvider");
        IScalarDataVariable = TelemetryNamespace.CreateType("IScalarDataVariable");
        ScalarDataVariable = TelemetryNamespace.CreateType("ScalarDataVariable");

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

    public static TypeIdentifier ArrayDataVariable { get; }
    public static TypeIdentifier CameraState { get; }
    public static TypeIdentifier CarLeftRight { get; }
    public static TypeIdentifier DataVariableDescriptor { get; }
    public static TypeIdentifier DataVariableInfo { get; }
    public static TypeIdentifier DataVariableInitializationException { get; }
    public static TypeIdentifier DataVariableValueType { get; }
    public static TypeIdentifier EngineWarnings { get; }
    public static TypeIdentifier GenerateDataVariableDescriptorsAttribute { get; }
    public static TypeIdentifier GenerateDataVariablesContextAttribute { get; }
    public static TypeIdentifier IArrayDataVariable { get; }
    public static TypeIdentifier IDataVariable { get; }
    public static TypeIdentifier IDataVariablesContext { get; }
    public static TypeIdentifier IDataVariableInfoProvider { get; }
    public static TypeIdentifier IScalarDataVariable { get; }
    public static TypeIdentifier PaceMode { get; }
    public static TypeIdentifier PaceRacingFlags { get; }
    public static TypeIdentifier PitServiceOptions { get; }
    public static TypeIdentifier PitServiceStatus { get; }
    public static TypeIdentifier RacingFlags { get; }
    public static TypeIdentifier ScalarDataVariable { get; }
    public static TypeIdentifier SessionState { get; }
    public static TypeIdentifier TrackLocationType { get; }
    public static TypeIdentifier TrackSurfaceType { get; }
}
