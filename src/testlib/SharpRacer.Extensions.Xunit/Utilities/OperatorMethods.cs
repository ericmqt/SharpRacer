namespace SharpRacer.Extensions.Xunit.Utilities;

internal static class OperatorMethods
{
    private static string _EqualityOperatorName = "op_Equality";
    private static string _GreaterThanOperatorName = "op_GreaterThan";
    private static string _GreaterThanOrEqualOperatorName = "op_GreaterThanOrEqual";
    private static string _InequalityOperatorName = "op_Inequality";
    private static string _LessThanOperatorName = "op_LessThan";
    private static string _LessThanOrEqualOperatorName = "op_LessThanOrEqual";

    public static OperatorMethod Equality { get; } = new OperatorMethod(_EqualityOperatorName, "==");
    public static OperatorMethod GreaterThan { get; } = new OperatorMethod(_GreaterThanOperatorName, ">");
    public static OperatorMethod GreaterThanOrEqual { get; } = new OperatorMethod(_GreaterThanOrEqualOperatorName, ">=");
    public static OperatorMethod Inequality { get; } = new OperatorMethod(_InequalityOperatorName, "!=");
    public static OperatorMethod LessThan { get; } = new OperatorMethod(_LessThanOperatorName, "<");
    public static OperatorMethod LessThanOrEqual { get; } = new OperatorMethod(_LessThanOrEqualOperatorName, "<=");
}
