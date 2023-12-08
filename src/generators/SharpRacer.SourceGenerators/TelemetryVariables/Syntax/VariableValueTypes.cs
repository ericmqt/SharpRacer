using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Syntax;
internal static class VariableValueTypes
{
    public static TypeSyntax Bool() => ParseTypeName("bool");
    public static TypeSyntax Byte() => ParseTypeName("byte");
    public static TypeSyntax Float() => ParseTypeName("float");
    public static TypeSyntax Double() => ParseTypeName("double");
    public static TypeSyntax Int() => ParseTypeName("int");

    public static TypeSyntax? Enumeration(string variableUnit)
    {
        if (string.IsNullOrEmpty(variableUnit))
        {
            throw new ArgumentException($"'{nameof(variableUnit)}' cannot be null or empty.", nameof(variableUnit));
        }

        return variableUnit switch
        {
            BitfieldUnitNames.irsdk_CameraState => SharpRacerTypes.Enumerations.CameraState(),
            BitfieldUnitNames.irsdk_CarLeftRight => SharpRacerTypes.Enumerations.CarLeftRight(),
            BitfieldUnitNames.irsdk_EngineWarnings => SharpRacerTypes.Enumerations.EngineWarnings(),
            BitfieldUnitNames.irsdk_Flags => SharpRacerTypes.Enumerations.RacingFlags(),
            BitfieldUnitNames.irsdk_PitSvFlags => SharpRacerTypes.Enumerations.PitServiceOptions(),

            EnumerationUnitNames.irsdk_PaceFlags => SharpRacerTypes.Enumerations.PaceRacingFlags(),
            EnumerationUnitNames.irsdk_PaceMode => SharpRacerTypes.Enumerations.PaceMode(),
            EnumerationUnitNames.irsdk_PitSvStatus => SharpRacerTypes.Enumerations.PitServiceStatus(),
            EnumerationUnitNames.irsdk_SessionState => SharpRacerTypes.Enumerations.SessionState(),
            EnumerationUnitNames.irsdk_TrkLoc => SharpRacerTypes.Enumerations.TrackLocationType(),
            EnumerationUnitNames.irsdk_TrkSurf => SharpRacerTypes.Enumerations.TrackSurfaceType(),

            _ => null
        };
    }

    public static TypeSyntax EnumerationOrInt(string? variableUnit)
    {
        return variableUnit switch
        {
            BitfieldUnitNames.irsdk_CameraState => SharpRacerTypes.Enumerations.CameraState(),
            BitfieldUnitNames.irsdk_CarLeftRight => SharpRacerTypes.Enumerations.CarLeftRight(),
            BitfieldUnitNames.irsdk_EngineWarnings => SharpRacerTypes.Enumerations.EngineWarnings(),
            BitfieldUnitNames.irsdk_Flags => SharpRacerTypes.Enumerations.RacingFlags(),
            BitfieldUnitNames.irsdk_PitSvFlags => SharpRacerTypes.Enumerations.PitServiceOptions(),

            EnumerationUnitNames.irsdk_PaceFlags => SharpRacerTypes.Enumerations.PaceRacingFlags(),
            EnumerationUnitNames.irsdk_PaceMode => SharpRacerTypes.Enumerations.PaceMode(),
            EnumerationUnitNames.irsdk_PitSvStatus => SharpRacerTypes.Enumerations.PitServiceStatus(),
            EnumerationUnitNames.irsdk_SessionState => SharpRacerTypes.Enumerations.SessionState(),
            EnumerationUnitNames.irsdk_TrkLoc => SharpRacerTypes.Enumerations.TrackLocationType(),
            EnumerationUnitNames.irsdk_TrkSurf => SharpRacerTypes.Enumerations.TrackSurfaceType(),

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
