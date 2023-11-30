using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Syntax;
internal static class SharpRacerTypes
{
    private static string DataVariableDescriptor_TypeName = "DataVariableDescriptor";
    private static string DataVariableValueType_TypeName = "DataVariableValueType";

    private static QualifiedNameSyntax SharpRacerTelemetryNamespace()
    {
        return QualifiedName(IdentifierName("SharpRacer"), IdentifierName("Telemetry"));
    }

    private static QualifiedNameSyntax SharpRacerTelemetryVariablesNamespace()
    {
        return QualifiedName(SharpRacerTelemetryNamespace(), IdentifierName("Variables"));
    }

    public static TypeSyntax DataVariableDescriptor()
    {
        return IdentifierName(DataVariableDescriptor_TypeName);
    }

    public static TypeSyntax DataVariableValueType()
    {
        return IdentifierName(DataVariableValueType_TypeName);
    }
}
