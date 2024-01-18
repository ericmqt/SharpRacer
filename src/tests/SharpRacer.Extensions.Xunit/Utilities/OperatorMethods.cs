using System.Reflection;

namespace SharpRacer.Extensions.Xunit.Utilities;
internal static class OperatorMethods
{
    public static MethodInfo? GetEqualityOperator<T>()
    {
        return GetEqualityOperator(typeof(T));
    }

    public static MethodInfo? GetEqualityOperator(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return type.GetMethod("op_Equality", BindingFlags.Static | BindingFlags.Public);
    }

    public static MethodInfo? GetInequalityOperator<T>()
    {
        return GetInequalityOperator(typeof(T));
    }

    public static MethodInfo? GetInequalityOperator(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return type.GetMethod("op_Inequality", BindingFlags.Static | BindingFlags.Public);
    }
}
