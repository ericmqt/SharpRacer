namespace SharpRacer.SourceGenerators;
public static class SharpRacerIdentifiers
{
    static SharpRacerIdentifiers()
    {
        SharpRacerNamespace = new NamespaceIdentifier("SharpRacer");
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
