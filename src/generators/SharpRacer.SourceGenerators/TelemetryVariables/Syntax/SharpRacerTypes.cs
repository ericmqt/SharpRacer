using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Syntax;

internal static class SharpRacerTypes
{
    public static TypeSyntax ArrayTelemetryVariableType(TypeSyntax typeArgument, TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        return SharpRacerIdentifiers.ArrayTelemetryVariable.ToGenericTypeSyntax(
            TypeArgumentList(SingletonSeparatedList(typeArgument)), typeNameFormat);
    }

    public static TypeSyntax CameraState(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
        => SharpRacerIdentifiers.CameraState.ToTypeSyntax(typeNameFormat);

    public static TypeSyntax CarLeftRight(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
        => SharpRacerIdentifiers.CarLeftRight.ToTypeSyntax(typeNameFormat);

    public static TypeSyntax EngineWarnings(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
        => SharpRacerIdentifiers.EngineWarnings.ToTypeSyntax(typeNameFormat);

    public static TypeSyntax IArrayTelemetryVariableInterfaceType(TypeSyntax typeArgument, TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        return SharpRacerIdentifiers.IArrayTelemetryVariable.ToGenericTypeSyntax(TypeArgumentList(SingletonSeparatedList(typeArgument)), typeNameFormat);
    }

    public static TypeSyntax ITelemetryVariableInfoProvider(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        return SharpRacerIdentifiers.ITelemetryVariableInfoProvider.ToTypeSyntax(typeNameFormat);
    }

    public static TypeSyntax ITelemetryVariablesContextInterfaceType(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        return SharpRacerIdentifiers.ITelemetryVariablesContext.ToTypeSyntax(typeNameFormat);
    }

    public static TypeSyntax IScalarTelemetryVariableInterfaceType(TypeSyntax typeArgument, TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        return SharpRacerIdentifiers.IScalarTelemetryVariable.ToGenericTypeSyntax(
            TypeArgumentList(SingletonSeparatedList(typeArgument)), typeNameFormat);
    }

    public static TypeSyntax PaceMode(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
        => SharpRacerIdentifiers.PaceMode.ToTypeSyntax(typeNameFormat);

    public static TypeSyntax PaceRacingFlags(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
        => SharpRacerIdentifiers.PaceRacingFlags.ToTypeSyntax(typeNameFormat);

    public static TypeSyntax PitServiceOptions(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
        => SharpRacerIdentifiers.PitServiceOptions.ToTypeSyntax(typeNameFormat);

    public static TypeSyntax PitServiceStatus(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
        => SharpRacerIdentifiers.PitServiceStatus.ToTypeSyntax(typeNameFormat);

    public static TypeSyntax RacingFlags(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
        => SharpRacerIdentifiers.RacingFlags.ToTypeSyntax(typeNameFormat);

    public static TypeSyntax ScalarTelemetryVariableType(TypeSyntax typeArgument, TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        return SharpRacerIdentifiers.ScalarTelemetryVariable.ToGenericTypeSyntax(
            TypeArgumentList(SingletonSeparatedList(typeArgument)), typeNameFormat);
    }

    public static TypeSyntax SessionState(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
        => SharpRacerIdentifiers.SessionState.ToTypeSyntax(typeNameFormat);

    public static TypeSyntax TrackLocationType(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
        => SharpRacerIdentifiers.TrackLocationType.ToTypeSyntax(typeNameFormat);

    public static TypeSyntax TrackSurfaceType(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
        => SharpRacerIdentifiers.TrackSurfaceType.ToTypeSyntax(typeNameFormat);

    public static TypeSyntax TelemetryVariableTypeArgument(
        VariableValueType valueType,
        string? valueUnit,
        TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        return valueType switch
        {
            VariableValueType.Bitfield => VariableValueTypes.EnumerationOrInt(valueUnit, typeNameFormat),
            VariableValueType.Bool => VariableValueTypes.Bool(),
            VariableValueType.Byte => VariableValueTypes.Byte(),
            VariableValueType.Double => VariableValueTypes.Double(),
            VariableValueType.Float => VariableValueTypes.Float(),
            VariableValueType.Int => VariableValueTypes.EnumerationOrInt(valueUnit, typeNameFormat),

            _ => throw new ArgumentException($"{valueType} is not a valid {nameof(VariableValueType)} value.")
        };
    }

    public static TypeSyntax TelemetryVariableDescriptor(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        return SharpRacerIdentifiers.TelemetryVariableDescriptor.ToTypeSyntax(typeNameFormat);
    }

    public static TypeSyntax TelemetryVariableInfo(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        return SharpRacerIdentifiers.TelemetryVariableInfo.ToTypeSyntax(typeNameFormat);
    }

    public static TypeSyntax TelemetryVariableInitializationException(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        return SharpRacerIdentifiers.TelemetryVariableInitializationException.ToTypeSyntax(typeNameFormat);
    }

    public static TypeSyntax TelemetryVariableValueType(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        return SharpRacerIdentifiers.TelemetryVariableValueType.ToTypeSyntax(typeNameFormat);
    }
}
