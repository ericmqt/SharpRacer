using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Syntax;
internal static class VariableValueTypes
{
    public static TypeSyntax Bool() => ParseTypeName("bool");
    public static TypeSyntax Byte() => ParseTypeName("byte");
    public static TypeSyntax Double() => ParseTypeName("double");
    public static TypeSyntax Float() => ParseTypeName("float");
    public static TypeSyntax Int() => ParseTypeName("int");

    public static TypeSyntax? Enumeration(string variableUnit, TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        if (string.IsNullOrEmpty(variableUnit))
        {
            throw new ArgumentException($"'{nameof(variableUnit)}' cannot be null or empty.", nameof(variableUnit));
        }

        return variableUnit switch
        {
            BitfieldUnitNames.irsdk_CameraState => SharpRacerTypes.CameraState(typeNameFormat),
            BitfieldUnitNames.irsdk_CarLeftRight => SharpRacerTypes.CarLeftRight(typeNameFormat),
            BitfieldUnitNames.irsdk_EngineWarnings => SharpRacerTypes.EngineWarnings(typeNameFormat),
            BitfieldUnitNames.irsdk_Flags => SharpRacerTypes.RacingFlags(typeNameFormat),
            BitfieldUnitNames.irsdk_PitSvFlags => SharpRacerTypes.PitServiceOptions(typeNameFormat),

            EnumerationUnitNames.irsdk_PaceFlags => SharpRacerTypes.PaceRacingFlags(typeNameFormat),
            EnumerationUnitNames.irsdk_PaceMode => SharpRacerTypes.PaceMode(typeNameFormat),
            EnumerationUnitNames.irsdk_PitSvStatus => SharpRacerTypes.PitServiceStatus(typeNameFormat),
            EnumerationUnitNames.irsdk_SessionState => SharpRacerTypes.SessionState(typeNameFormat),
            EnumerationUnitNames.irsdk_TrkLoc => SharpRacerTypes.TrackLocationType(typeNameFormat),
            EnumerationUnitNames.irsdk_TrkSurf => SharpRacerTypes.TrackSurfaceType(typeNameFormat),

            _ => null
        };
    }

    public static TypeSyntax EnumerationOrInt(string? variableUnit, TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        return variableUnit switch
        {
            BitfieldUnitNames.irsdk_CameraState => SharpRacerTypes.CameraState(typeNameFormat),
            BitfieldUnitNames.irsdk_CarLeftRight => SharpRacerTypes.CarLeftRight(typeNameFormat),
            BitfieldUnitNames.irsdk_EngineWarnings => SharpRacerTypes.EngineWarnings(typeNameFormat),
            BitfieldUnitNames.irsdk_Flags => SharpRacerTypes.RacingFlags(typeNameFormat),
            BitfieldUnitNames.irsdk_PitSvFlags => SharpRacerTypes.PitServiceOptions(typeNameFormat),

            EnumerationUnitNames.irsdk_PaceFlags => SharpRacerTypes.PaceRacingFlags(typeNameFormat),
            EnumerationUnitNames.irsdk_PaceMode => SharpRacerTypes.PaceMode(typeNameFormat),
            EnumerationUnitNames.irsdk_PitSvStatus => SharpRacerTypes.PitServiceStatus(typeNameFormat),
            EnumerationUnitNames.irsdk_SessionState => SharpRacerTypes.SessionState(typeNameFormat),
            EnumerationUnitNames.irsdk_TrkLoc => SharpRacerTypes.TrackLocationType(typeNameFormat),
            EnumerationUnitNames.irsdk_TrkSurf => SharpRacerTypes.TrackSurfaceType(typeNameFormat),

            _ => Int()
        };
    }

    private static class BitfieldUnitNames
    {
        public const string irsdk_CameraState = "irsdk_CameraState";
        public const string irsdk_CarLeftRight = "irsdk_CarLeftRight";
        public const string irsdk_EngineWarnings = "irsdk_EngineWarnings";
        public const string irsdk_Flags = "irsdk_Flags";
        public const string irsdk_PitSvFlags = "irsdk_PitSvFlags";
    }

    private static class EnumerationUnitNames
    {
        public const string irsdk_PaceFlags = "irsdk_PaceFlags";
        public const string irsdk_PaceMode = "irsdk_PaceMode";
        public const string irsdk_PitSvStatus = "irsdk_PitSvStatus";
        public const string irsdk_SessionState = "irsdk_SessionState";
        public const string irsdk_TrkLoc = "irsdk_TrkLoc";
        public const string irsdk_TrkSurf = "irsdk_TrkSurf";
    }
}
