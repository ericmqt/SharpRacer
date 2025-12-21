using SharpRacer.Telemetry;

namespace SharpRacer.SourceGenerators;

public class SharpRacerIdentifiersTests
{
    [Fact]
    public void TelemetryNamespace_Test()
    {
        Assert.Equal("SharpRacer.Telemetry", SharpRacerIdentifiers.TelemetryNamespace);
    }

    [Theory]
    [MemberData(nameof(TypeIdentifiersAndExpectedTypes))]
    public void TypeIdentifier_Test(TypeIdentifier typeIdentifier, Type expectedType)
    {
        var expectedTypeName = expectedType.IsGenericType
            ? expectedType.Name[..expectedType.Name.IndexOf('`')]
            : expectedType.Name;

        var expectedNamespace = expectedType.Namespace;

        Assert.Equal(expectedTypeName, typeIdentifier.TypeName);
        Assert.Equal(expectedNamespace, typeIdentifier.Namespace);
    }

    public static IEnumerable<object[]> TypeIdentifiersAndExpectedTypes =>
        new TheoryData<TypeIdentifier, Type>()
        {
            { SharpRacerIdentifiers.ArrayTelemetryVariable, typeof(ArrayTelemetryVariable<>) },
            { SharpRacerIdentifiers.CameraState, typeof(CameraState) },
            { SharpRacerIdentifiers.CarLeftRight, typeof(CarLeftRight) },
            { SharpRacerIdentifiers.EngineWarnings, typeof(EngineWarnings) },
            { SharpRacerIdentifiers.GenerateTelemetryVariableDescriptorsAttribute, typeof(GenerateTelemetryVariableDescriptorsAttribute) },
            { SharpRacerIdentifiers.GenerateTelemetryVariablesContextAttribute, typeof(GenerateTelemetryVariablesContextAttribute) },
            { SharpRacerIdentifiers.IArrayTelemetryVariable, typeof(IArrayTelemetryVariable<>) },
            { SharpRacerIdentifiers.ITelemetryVariable, typeof(ITelemetryVariable) },
            { SharpRacerIdentifiers.ITelemetryVariablesContext, typeof(ITelemetryVariablesContext) },
            { SharpRacerIdentifiers.ITelemetryVariableInfoProvider, typeof(ITelemetryVariableInfoProvider) },
            { SharpRacerIdentifiers.IScalarTelemetryVariable, typeof(IScalarTelemetryVariable<>) },
            { SharpRacerIdentifiers.PaceMode, typeof(PaceMode) },
            { SharpRacerIdentifiers.PaceRacingFlags, typeof(PaceRacingFlags) },
            { SharpRacerIdentifiers.PitServiceOptions, typeof(PitServiceOptions) },
            { SharpRacerIdentifiers.PitServiceStatus, typeof(PitServiceStatus) },
            { SharpRacerIdentifiers.RacingFlags, typeof(RacingFlags) },
            { SharpRacerIdentifiers.ScalarTelemetryVariable, typeof(ScalarTelemetryVariable<>) },
            { SharpRacerIdentifiers.SessionState, typeof(SessionState) },
            { SharpRacerIdentifiers.TrackLocationType, typeof(TrackLocationType) },
            { SharpRacerIdentifiers.TrackSurfaceType, typeof(TrackSurfaceType) },
            { SharpRacerIdentifiers.TelemetryVariableDescriptor, typeof(TelemetryVariableDescriptor) },
            { SharpRacerIdentifiers.TelemetryVariableInfo, typeof(TelemetryVariableInfo) },
            { SharpRacerIdentifiers.TelemetryVariableInitializationException, typeof(TelemetryVariableInitializationException) },
            { SharpRacerIdentifiers.TelemetryVariableValueType, typeof(TelemetryVariableValueType) }
        };
}
