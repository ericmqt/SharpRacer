namespace SharpRacer.SourceGenerators;
public static class SharpRacerIdentifiers
{
    static SharpRacerIdentifiers()
    {
        TelemetryNamespace = new NamespaceIdentifier("SharpRacer.Telemetry");
        TelemetryVariablesNamespace = new NamespaceIdentifier("SharpRacer.Telemetry.Variables");

        ArrayDataVariable = TelemetryVariablesNamespace.CreateType("ArrayDataVariable");
        DataVariableDescriptor = TelemetryVariablesNamespace.CreateType("DataVariableDescriptor");
        DataVariableFactory = TelemetryVariablesNamespace.CreateType("DataVariableFactory");
        DataVariableInfo = TelemetryVariablesNamespace.CreateType("DataVariableInfo");
        DataVariableInitializationException = TelemetryVariablesNamespace.CreateType("DataVariableInitializationException");
        DataVariableValueType = TelemetryVariablesNamespace.CreateType("DataVariableValueType");
        GenerateDataVariableDescriptorsAttribute = TelemetryVariablesNamespace.CreateType("GenerateDataVariableDescriptorsAttribute");
        GenerateDataVariablesContextAttribute = TelemetryVariablesNamespace.CreateType("GenerateDataVariablesContextAttribute");
        IArrayDataVariable = TelemetryVariablesNamespace.CreateType("IArrayDataVariable");
        ICreateDataVariable = TelemetryVariablesNamespace.CreateType("ICreateDataVariable");
        IDataVariable = TelemetryVariablesNamespace.CreateType("IDataVariable");
        IDataVariablesContext = TelemetryVariablesNamespace.CreateType("IDataVariablesContext");
        IDataVariableInfoProvider = TelemetryVariablesNamespace.CreateType("IDataVariableInfoProvider");
        IScalarDataVariable = TelemetryVariablesNamespace.CreateType("IScalarDataVariable");
        ScalarDataVariable = TelemetryVariablesNamespace.CreateType("ScalarDataVariable");

        CameraState = TelemetryNamespace.CreateType("CameraState");
        CarLeftRight = TelemetryNamespace.CreateType("CarLeftRight");
        EngineWarnings = TelemetryNamespace.CreateType("EngineWarnings");
        PaceMode = TelemetryNamespace.CreateType("PaceMode");
        PaceRacingFlags = TelemetryNamespace.CreateType("PaceRacingFlags");
        PitServiceOptions = TelemetryNamespace.CreateType("PitServiceOptions");
        PitServiceStatus = TelemetryNamespace.CreateType("PitServiceStatus");
        RacingFlags = TelemetryNamespace.CreateType("RacingFlags");
        SessionState = TelemetryNamespace.CreateType("SessionState");
        TrackLocationType = TelemetryNamespace.CreateType("TrackLocationType");
        TrackSurfaceType = TelemetryNamespace.CreateType("TrackSurfaceType");
    }

    public static NamespaceIdentifier TelemetryNamespace { get; }
    public static NamespaceIdentifier TelemetryVariablesNamespace { get; }

    public static TypeIdentifier ArrayDataVariable { get; }
    public static TypeIdentifier CameraState { get; }
    public static TypeIdentifier CarLeftRight { get; }
    public static TypeIdentifier DataVariableDescriptor { get; }
    public static TypeIdentifier DataVariableFactory { get; }
    public static TypeIdentifier DataVariableInfo { get; }
    public static TypeIdentifier DataVariableInitializationException { get; }
    public static TypeIdentifier DataVariableValueType { get; }
    public static TypeIdentifier EngineWarnings { get; }
    public static TypeIdentifier GenerateDataVariableDescriptorsAttribute { get; }
    public static TypeIdentifier GenerateDataVariablesContextAttribute { get; }
    public static TypeIdentifier IArrayDataVariable { get; }
    public static TypeIdentifier ICreateDataVariable { get; }
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
