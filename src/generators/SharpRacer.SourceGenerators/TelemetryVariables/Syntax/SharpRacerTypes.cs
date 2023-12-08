using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Syntax;
internal static class SharpRacerTypes
{
    private static string DataVariableDescriptor_TypeName = "DataVariableDescriptor";
    private static string DataVariableValueType_TypeName = "DataVariableValueType";
    private static string DataVariableInfo_TypeName = "DataVariableInfo";

    private static QualifiedNameSyntax SharpRacerTelemetryNamespace()
    {
        return QualifiedName(IdentifierName("SharpRacer"), IdentifierName("Telemetry"));
    }

    private static QualifiedNameSyntax SharpRacerTelemetryVariablesNamespace()
    {
        return QualifiedName(SharpRacerTelemetryNamespace(), IdentifierName("Variables"));
    }

    public static BaseTypeSyntax ArrayDataVariableBaseType(TypeSyntax typeArgument)
    {
        return SimpleBaseType(SharpRacerTypes.ArrayDataVariableType(typeArgument));
    }

    public static GenericNameSyntax ArrayDataVariableType(VariableInfo variableInfo)
    {
        var typeArg = DataVariableTypeArgument(variableInfo);

        return ArrayDataVariableType(typeArg);
    }

    public static GenericNameSyntax ArrayDataVariableType(TypeSyntax typeArgument)
    {
        return GenericName("ArrayDataVariable")
            .WithTypeArgumentList(
                TypeArgumentList(
                    SingletonSeparatedList(typeArgument)));
    }

    public static TypeSyntax DataVariableTypeArgument(VariableInfo variableInfo)
    {
        return DataVariableTypeArgument(variableInfo.ValueType, variableInfo.ValueUnit);
    }

    public static TypeSyntax DataVariableTypeArgument(VariableValueType valueType, string? valueUnit)
    {
        return valueType switch
        {
            VariableValueType.Bitfield => VariableValueTypes.EnumerationOrInt(valueUnit),
            VariableValueType.Bool => VariableValueTypes.Bool(),
            VariableValueType.Byte => VariableValueTypes.Byte(),
            VariableValueType.Double => VariableValueTypes.Double(),
            VariableValueType.Float => VariableValueTypes.Float(),
            VariableValueType.Int => VariableValueTypes.EnumerationOrInt(valueUnit),

            _ => throw new InvalidOperationException($"{valueType} is not a valid {nameof(VariableValueType)} value.")
        };
    }

    public static TypeSyntax DataVariableDescriptor()
    {
        return IdentifierName(DataVariableDescriptor_TypeName);
    }

    public static TypeSyntax DataVariableInfo()
    {
        return IdentifierName(DataVariableInfo_TypeName);
    }

    public static TypeSyntax DataVariableValueType()
    {
        return IdentifierName(DataVariableValueType_TypeName);
    }

    public static TypeSyntax ICreateDataVariableInterfaceType(TypeSyntax selfTypeArgument)
    {
        return GenericName("ICreateDataVariable")
            .WithTypeArgumentList(
                TypeArgumentList(
                    SingletonSeparatedList(selfTypeArgument)));
    }

    public static BaseTypeSyntax ScalarDataVariableBaseType(TypeSyntax typeArgument)
    {
        return SimpleBaseType(ScalarDataVariableType(typeArgument));
    }

    public static TypeSyntax ScalarDataVariableType(TypeSyntax typeArgument)
    {
        return GenericName("ScalarDataVariable")
            .WithTypeArgumentList(
                TypeArgumentList(
                    SingletonSeparatedList(typeArgument)));
    }

    public static class Enumerations
    {
        public static TypeSyntax CameraState() => ParseTypeName("SharpRacer.Telemetry.CameraState");
        public static TypeSyntax CarLeftRight() => ParseTypeName("SharpRacer.Telemetry.CarLeftRight");
        public static TypeSyntax EngineWarnings() => ParseTypeName("SharpRacer.Telemetry.EngineWarnings");
        public static TypeSyntax PaceMode() => ParseTypeName("SharpRacer.Telemetry.PaceMode");
        public static TypeSyntax PaceRacingFlags() => ParseTypeName("SharpRacer.Telemetry.PaceRacingFlags");
        public static TypeSyntax PitServiceOptions() => ParseTypeName("SharpRacer.Telemetry.PitServiceOptions");
        public static TypeSyntax PitServiceStatus() => ParseTypeName("SharpRacer.Telemetry.PitServiceStatus");
        public static TypeSyntax RacingFlags() => ParseTypeName("SharpRacer.Telemetry.RacingFlags");
        public static TypeSyntax SessionState() => ParseTypeName("SharpRacer.Telemetry.SessionState");
        public static TypeSyntax TrackLocationType() => ParseTypeName("SharpRacer.Telemetry.TrackLocationType");
        public static TypeSyntax TrackSurfaceType() => ParseTypeName("SharpRacer.Telemetry.TrackSurfaceType");
    }
}
