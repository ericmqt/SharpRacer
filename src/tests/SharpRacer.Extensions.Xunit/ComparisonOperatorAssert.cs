using System.Runtime.CompilerServices;
using SharpRacer.Extensions.Xunit.Utilities;

namespace SharpRacer.Extensions.Xunit;
public static class ComparisonOperatorAssert
{
    public static void GreaterThan<T>(
        bool expected,
        T first,
        T second,
        [CallerArgumentExpression(nameof(first))] string? firstArg = null,
        [CallerArgumentExpression(nameof(second))] string? secondArg = null)
    {
        OperatorMethods.GreaterThan.Assert(expected, first, second, firstArg, secondArg);
    }

    public static void GreaterThanOrEqual<T>(
        bool expected,
        T first,
        T second,
        [CallerArgumentExpression(nameof(first))] string? firstArg = null,
        [CallerArgumentExpression(nameof(second))] string? secondArg = null)
    {
        OperatorMethods.GreaterThanOrEqual.Assert(expected, first, second, firstArg, secondArg);
    }

    public static void LessThan<T>(
        bool expected,
        T first,
        T second,
        [CallerArgumentExpression(nameof(first))] string? firstArg = null,
        [CallerArgumentExpression(nameof(second))] string? secondArg = null)
    {
        OperatorMethods.LessThan.Assert(expected, first, second, firstArg, secondArg);
    }

    public static void LessThanOrEqual<T>(
        bool expected,
        T first,
        T second,
        [CallerArgumentExpression(nameof(first))] string? firstArg = null,
        [CallerArgumentExpression(nameof(second))] string? secondArg = null)
    {
        OperatorMethods.LessThanOrEqual.Assert(expected, first, second, firstArg, secondArg);
    }

    public static void OperandsEqual<T>(
        T first,
        T second,
        [CallerArgumentExpression(nameof(first))] string? firstArg = null,
        [CallerArgumentExpression(nameof(second))] string? secondArg = null)
    {
        // first > second == false
        GreaterThan(false, first, second, firstArg, secondArg);

        // second > first == false
        GreaterThan(false, second, first, secondArg, firstArg);

        // first >= second == true
        GreaterThanOrEqual(true, first, second, firstArg, secondArg);

        // second >= first == true
        GreaterThanOrEqual(true, second, first, secondArg, firstArg);

        // first < second == false
        LessThan(false, first, second, firstArg, secondArg);

        // second < first == false
        LessThan(false, second, first, secondArg, firstArg);

        // first <= second == true
        LessThanOrEqual(true, first, second, firstArg, secondArg);

        // second <= first == true
        LessThanOrEqual(true, second, first, secondArg, firstArg);
    }

    public static void OperandGreaterThan<T>(
        T first,
        T second,
        [CallerArgumentExpression(nameof(first))] string? firstArg = null,
        [CallerArgumentExpression(nameof(second))] string? secondArg = null)
    {
        // (first > second) == true
        GreaterThan(true, first, second, firstArg, secondArg);

        // (second > first) == false
        GreaterThan(false, second, first, secondArg, firstArg);

        // (first >= second) == true
        GreaterThanOrEqual(true, first, second, firstArg, secondArg);

        // (second >= first) == false
        GreaterThanOrEqual(false, second, first, secondArg, firstArg);

        // (first < second) == false
        LessThan(false, first, second, firstArg, secondArg);

        // (second < first) == true
        LessThan(true, second, first, secondArg, firstArg);

        // (first <= second) == false
        LessThanOrEqual(false, first, second, firstArg, secondArg);

        // (second <= first) == true
        LessThanOrEqual(true, second, first, secondArg, firstArg);
    }

    public static void OperandLessThan<T>(
        T first,
        T second,
        [CallerArgumentExpression(nameof(first))] string? firstArg = null,
        [CallerArgumentExpression(nameof(second))] string? secondArg = null)
    {
        // (first < second) == true
        LessThan(true, first, second, firstArg, secondArg);

        // (second < first) == false
        LessThan(false, second, first, secondArg, firstArg);

        // (first <= second) == true
        LessThanOrEqual(true, first, second, firstArg, secondArg);

        // (second <= first) == false
        LessThanOrEqual(false, second, first, secondArg, firstArg);

        // (first > second) == false
        GreaterThan(false, first, second, firstArg, secondArg);

        // (second > first) == true
        GreaterThan(true, second, first, secondArg, firstArg);

        // (first >= second) == false
        GreaterThanOrEqual(false, first, second, firstArg, secondArg);

        // (second >= first) == true
        GreaterThanOrEqual(true, second, first, secondArg, firstArg);
    }
}
