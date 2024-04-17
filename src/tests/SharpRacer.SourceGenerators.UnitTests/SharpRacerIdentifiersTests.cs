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
            { SharpRacerIdentifiers.ArrayDataVariable, typeof(ArrayDataVariable<>) },
            { SharpRacerIdentifiers.CameraState, typeof(CameraState) },
            { SharpRacerIdentifiers.CarLeftRight, typeof(CarLeftRight) },
            { SharpRacerIdentifiers.DataVariableDescriptor, typeof(DataVariableDescriptor) },
            { SharpRacerIdentifiers.DataVariableFactory, typeof(DataVariableFactory) },
            { SharpRacerIdentifiers.DataVariableInfo, typeof(DataVariableInfo) },
            { SharpRacerIdentifiers.DataVariableInitializationException, typeof(DataVariableInitializationException) },
            { SharpRacerIdentifiers.DataVariableValueType, typeof(DataVariableValueType) },
            { SharpRacerIdentifiers.EngineWarnings, typeof(EngineWarnings) },
            { SharpRacerIdentifiers.GenerateDataVariableDescriptorsAttribute, typeof(GenerateDataVariableDescriptorsAttribute) },
            { SharpRacerIdentifiers.GenerateDataVariablesContextAttribute, typeof(GenerateDataVariablesContextAttribute) },
            { SharpRacerIdentifiers.IArrayDataVariable, typeof(IArrayDataVariable<>) },
            { SharpRacerIdentifiers.ICreateDataVariable, typeof(ICreateDataVariable<>) },
            { SharpRacerIdentifiers.IDataVariable, typeof(IDataVariable) },
            { SharpRacerIdentifiers.IDataVariablesContext, typeof(IDataVariablesContext) },
            { SharpRacerIdentifiers.IDataVariableInfoProvider, typeof(IDataVariableInfoProvider) },
            { SharpRacerIdentifiers.IScalarDataVariable, typeof(IScalarDataVariable<>) },
            { SharpRacerIdentifiers.PaceMode, typeof(PaceMode) },
            { SharpRacerIdentifiers.PaceRacingFlags, typeof(PaceRacingFlags) },
            { SharpRacerIdentifiers.PitServiceOptions, typeof(PitServiceOptions) },
            { SharpRacerIdentifiers.PitServiceStatus, typeof(PitServiceStatus) },
            { SharpRacerIdentifiers.RacingFlags, typeof(RacingFlags) },
            { SharpRacerIdentifiers.ScalarDataVariable, typeof(ScalarDataVariable<>) },
            { SharpRacerIdentifiers.SessionState, typeof(SessionState) },
            { SharpRacerIdentifiers.TrackLocationType, typeof(TrackLocationType) },
            { SharpRacerIdentifiers.TrackSurfaceType, typeof(TrackSurfaceType) }
        };
}
