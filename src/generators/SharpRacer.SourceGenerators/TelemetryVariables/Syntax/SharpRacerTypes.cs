using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Syntax;
internal static class SharpRacerTypes
{
    public static GenericNameSyntax ArrayDataVariableType(TypeSyntax typeArgument, TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        return GenericName(SharpRacerIdentifiers.ArrayDataVariable.ToString(typeNameFormat))
            .WithTypeArgumentList(
                TypeArgumentList(
                    SingletonSeparatedList(typeArgument)));
    }

    public static TypeSyntax CameraState(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
        => IdentifierName(SharpRacerIdentifiers.CameraState.ToString(typeNameFormat));

    public static TypeSyntax CarLeftRight(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
        => IdentifierName(SharpRacerIdentifiers.CarLeftRight.ToString(typeNameFormat));

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
        return IdentifierName(SharpRacerIdentifiers.DataVariableDescriptor.ToString(typeNameFormat));
    }

    public static TypeSyntax DataVariableFactory(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        return IdentifierName(SharpRacerIdentifiers.DataVariableFactory.ToString(typeNameFormat));
    }

    public static TypeSyntax DataVariableInfo(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        return IdentifierName(SharpRacerIdentifiers.DataVariableInfo.ToString(typeNameFormat));
    }

    public static TypeSyntax DataVariableInitializationException(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        return IdentifierName(SharpRacerIdentifiers.DataVariableInitializationException.ToString(typeNameFormat));
    }

    public static TypeSyntax DataVariableValueType(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        return IdentifierName(SharpRacerIdentifiers.DataVariableValueType.ToString(typeNameFormat));
    }

    public static TypeSyntax EngineWarnings(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
        => IdentifierName(SharpRacerIdentifiers.EngineWarnings.ToString(typeNameFormat));

    public static TypeSyntax IArrayDataVariableInterfaceType(TypeSyntax typeArgument, TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        return GenericName(SharpRacerIdentifiers.IArrayDataVariable.ToString(typeNameFormat))
            .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(typeArgument)));
    }

    public static TypeSyntax ICreateDataVariableInterfaceType(TypeSyntax selfTypeArgument, TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        return GenericName(SharpRacerIdentifiers.ICreateDataVariable.ToString(typeNameFormat))
            .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(selfTypeArgument)));
    }

    public static TypeSyntax IDataVariableInfoProvider(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        return IdentifierName(SharpRacerIdentifiers.IDataVariableInfoProvider.ToString(typeNameFormat));
    }

    public static TypeSyntax IDataVariablesContextInterfaceType(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        return IdentifierName(SharpRacerIdentifiers.IDataVariablesContext.ToString(typeNameFormat));
    }

    public static TypeSyntax IScalarDataVariableInterfaceType(TypeSyntax typeArgument, TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        return GenericName(SharpRacerIdentifiers.IScalarDataVariable.ToString(typeNameFormat))
            .WithTypeArgumentList(
                TypeArgumentList(
                    SingletonSeparatedList(typeArgument)));
    }

    public static TypeSyntax PaceMode(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
        => IdentifierName(SharpRacerIdentifiers.PaceMode.ToString(typeNameFormat));

    public static TypeSyntax PaceRacingFlags(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
        => IdentifierName(SharpRacerIdentifiers.PaceRacingFlags.ToString(typeNameFormat));

    public static TypeSyntax PitServiceOptions(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
        => IdentifierName(SharpRacerIdentifiers.PitServiceOptions.ToString(typeNameFormat));

    public static TypeSyntax PitServiceStatus(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
        => IdentifierName(SharpRacerIdentifiers.PitServiceStatus.ToString(typeNameFormat));

    public static TypeSyntax RacingFlags(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
        => IdentifierName(SharpRacerIdentifiers.RacingFlags.ToString(typeNameFormat));

    public static TypeSyntax ScalarDataVariableType(TypeSyntax typeArgument, TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        return GenericName(SharpRacerIdentifiers.ScalarDataVariable.ToString(typeNameFormat))
            .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(typeArgument)));
    }

    public static TypeSyntax SessionState(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
        => IdentifierName(SharpRacerIdentifiers.SessionState.ToString(typeNameFormat));

    public static TypeSyntax TrackLocationType(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
        => IdentifierName(SharpRacerIdentifiers.TrackLocationType.ToString(typeNameFormat));

    public static TypeSyntax TrackSurfaceType(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
        => IdentifierName(SharpRacerIdentifiers.TrackSurfaceType.ToString(typeNameFormat));

}
