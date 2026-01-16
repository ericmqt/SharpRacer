using System.Runtime.CompilerServices;
using SharpRacer.Extensions.Xunit.Utilities;

namespace SharpRacer.Extensions.Xunit;

public static class OperatorAssert
{
    public static void Equality<T>(
        bool expected,
        T first,
        T second,
        [CallerArgumentExpression(nameof(first))] string? firstArg = null,
        [CallerArgumentExpression(nameof(second))] string? secondArg = null)
    {
        OperatorMethods.Equality.Assert(expected, first, second, firstArg, secondArg);
    }

    public static void Inequality<T>(
        bool expected,
        T first,
        T second,
        [CallerArgumentExpression(nameof(first))] string? firstArg = null,
        [CallerArgumentExpression(nameof(second))] string? secondArg = null)
    {
        OperatorMethods.Inequality.Assert(expected, first, second, firstArg, secondArg);
    }

    public static void OperandsEqual<T>(
        T first,
        T second,
        [CallerArgumentExpression(nameof(first))] string? firstArg = null,
        [CallerArgumentExpression(nameof(second))] string? secondArg = null)
    {
        // (first == second) == true
        Equality(true, first, second, firstArg, secondArg);

        // (second == first) == true
        Equality(true, second, first, secondArg, firstArg);

        // (first != second) == false
        Inequality(false, first, second, firstArg, secondArg);

        // (second != first) == false
        Inequality(false, second, first, secondArg, firstArg);
    }

    public static void OperandsNotEqual<T>(
        T first,
        T second,
        [CallerArgumentExpression(nameof(first))] string? firstArg = null,
        [CallerArgumentExpression(nameof(second))] string? secondArg = null)
    {
        // (first != second) == true
        Inequality(true, first, second, firstArg, secondArg);

        // (second != first) == true
        Inequality(true, second, first, secondArg, firstArg);

        // (first == second) == false
        Equality(false, first, second, firstArg, secondArg);

        // (second == first) == false
        Equality(false, second, first, secondArg, firstArg);
    }
}
