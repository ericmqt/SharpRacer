using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Syntax;
internal static class SharpRacerTypes
{
    public static TypeSyntax ArrayDataVariableType(TypeSyntax typeArgument, TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        return SharpRacerIdentifiers.ArrayDataVariable.ToGenericTypeSyntax(
            TypeArgumentList(SingletonSeparatedList(typeArgument)), typeNameFormat);
    }

    public static TypeSyntax CameraState(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
        => SharpRacerIdentifiers.CameraState.ToTypeSyntax(typeNameFormat);

    public static TypeSyntax CarLeftRight(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
        => SharpRacerIdentifiers.CarLeftRight.ToTypeSyntax(typeNameFormat);

    public static TypeSyntax DataVariableTypeArgument(
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

    public static TypeSyntax DataVariableDescriptor(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        return SharpRacerIdentifiers.DataVariableDescriptor.ToTypeSyntax(typeNameFormat);
    }

    public static TypeSyntax DataVariableInfo(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        return SharpRacerIdentifiers.DataVariableInfo.ToTypeSyntax(typeNameFormat);
    }

    public static TypeSyntax DataVariableInitializationException(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        return SharpRacerIdentifiers.DataVariableInitializationException.ToTypeSyntax(typeNameFormat);
    }

    public static TypeSyntax DataVariableValueType(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        return SharpRacerIdentifiers.DataVariableValueType.ToTypeSyntax(typeNameFormat);
    }

    public static TypeSyntax EngineWarnings(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
        => SharpRacerIdentifiers.EngineWarnings.ToTypeSyntax(typeNameFormat);

    public static TypeSyntax IArrayDataVariableInterfaceType(TypeSyntax typeArgument, TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        return SharpRacerIdentifiers.IArrayDataVariable.ToGenericTypeSyntax(TypeArgumentList(SingletonSeparatedList(typeArgument)), typeNameFormat);
    }

    public static TypeSyntax ICreateDataVariableInterfaceType(TypeSyntax selfTypeArgument, TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        return SharpRacerIdentifiers.ICreateDataVariable.ToGenericTypeSyntax(TypeArgumentList(SingletonSeparatedList(selfTypeArgument)), typeNameFormat);
    }

    public static TypeSyntax IDataVariableInfoProvider(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        return SharpRacerIdentifiers.IDataVariableInfoProvider.ToTypeSyntax(typeNameFormat);
    }

    public static TypeSyntax IDataVariablesContextInterfaceType(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        return SharpRacerIdentifiers.IDataVariablesContext.ToTypeSyntax(typeNameFormat);
    }

    public static TypeSyntax IScalarDataVariableInterfaceType(TypeSyntax typeArgument, TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        return SharpRacerIdentifiers.IScalarDataVariable.ToGenericTypeSyntax(
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

    public static TypeSyntax ScalarDataVariableType(TypeSyntax typeArgument, TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        return SharpRacerIdentifiers.ScalarDataVariable.ToGenericTypeSyntax(
            TypeArgumentList(SingletonSeparatedList(typeArgument)), typeNameFormat);
    }

    public static TypeSyntax SessionState(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
        => SharpRacerIdentifiers.SessionState.ToTypeSyntax(typeNameFormat);

    public static TypeSyntax TrackLocationType(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
        => SharpRacerIdentifiers.TrackLocationType.ToTypeSyntax(typeNameFormat);

    public static TypeSyntax TrackSurfaceType(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
        => SharpRacerIdentifiers.TrackSurfaceType.ToTypeSyntax(typeNameFormat);
}
