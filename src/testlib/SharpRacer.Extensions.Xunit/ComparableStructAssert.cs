using System.Runtime.CompilerServices;
using Xunit.Sdk;

namespace SharpRacer.Extensions.Xunit;
public static class ComparableStructAssert
{
    public static void Equal<T>(
        T first,
        T second,
        [CallerArgumentExpression(nameof(first))] string? firstArg = null,
        [CallerArgumentExpression(nameof(second))] string? secondArg = null)
        where T : struct, IComparable<T>
    {
        CompareToMethod_Equal(first, second, firstArg, secondArg);
        CompareToMethod_GreaterThanOrEqual(first, second, firstArg, secondArg);
        CompareToMethod_LessThanOrEqual(first, second, firstArg, secondArg);

        ComparisonOperatorAssert.OperandsEqual(first, second, firstArg, secondArg);
    }

    public static void NotEqual<T>(
        T first,
        T second,
        [CallerArgumentExpression(nameof(first))] string? firstArg = null,
        [CallerArgumentExpression(nameof(second))] string? secondArg = null)
        where T : struct, IComparable<T>
    {
        CompareToMethod_NotEqual(first, second, firstArg, secondArg);
    }

    public static void GreaterThan<T>(
        T first,
        T second,
        [CallerArgumentExpression(nameof(first))] string? firstArg = null,
        [CallerArgumentExpression(nameof(second))] string? secondArg = null)
        where T : struct, IComparable<T>
    {
        CompareToMethod_GreaterThan(first, second, firstArg, secondArg);
        CompareToMethod_GreaterThanOrEqual(first, second, firstArg, secondArg);

        ComparisonOperatorAssert.OperandGreaterThan(first, second, firstArg, secondArg);
    }

    public static void LessThan<T>(
        T first,
        T second,
        [CallerArgumentExpression(nameof(first))] string? firstArg = null,
        [CallerArgumentExpression(nameof(second))] string? secondArg = null)
        where T : struct, IComparable<T>
    {
        CompareToMethod_LessThan(first, second, firstArg, secondArg);
        CompareToMethod_LessThanOrEqual(first, second, firstArg, secondArg);

        ComparisonOperatorAssert.OperandLessThan(first, second, firstArg, secondArg);
    }

    public static void CompareToMethod_Equal<T>(
        T first,
        T second,
        [CallerArgumentExpression(nameof(first))] string? firstArg = null,
        [CallerArgumentExpression(nameof(second))] string? secondArg = null)
        where T : struct, IComparable<T>
    {
        var result = first.CompareTo(second) == 0;

        if (!result)
        {
            throw new XunitException($"Expression '{firstArg}.CompareTo({secondArg}) == 0' evaluates to {result}.");
        }
    }

    public static void CompareToMethod_NotEqual<T>(
        T first,
        T second,
        [CallerArgumentExpression(nameof(first))] string? firstArg = null,
        [CallerArgumentExpression(nameof(second))] string? secondArg = null)
        where T : struct, IComparable<T>
    {
        var result = first.CompareTo(second) != 0;

        if (!result)
        {
            throw new XunitException($"Expression '{firstArg}.CompareTo({secondArg}) != 0' evaluates to {result}.");
        }
    }

    public static void CompareToMethod_LessThan<T>(
        T first,
        T second,
        [CallerArgumentExpression(nameof(first))] string? firstArg = null,
        [CallerArgumentExpression(nameof(second))] string? secondArg = null)
        where T : struct, IComparable<T>
    {
        var result = first.CompareTo(second) < 0;

        if (!result)
        {
            throw new XunitException($"Expression '{firstArg}.CompareTo({secondArg}) < 0' evaluates to {result}.");
        }
    }

    public static void CompareToMethod_LessThanOrEqual<T>(
        T first,
        T second,
        [CallerArgumentExpression(nameof(first))] string? firstArg = null,
        [CallerArgumentExpression(nameof(second))] string? secondArg = null)
        where T : struct, IComparable<T>
    {
        var result = first.CompareTo(second) <= 0;

        if (!result)
        {
            throw new XunitException($"Expression '{firstArg}.CompareTo({secondArg}) <= 0' evaluates to {result}.");
        }
    }

    public static void CompareToMethod_GreaterThan<T>(
        T first,
        T second,
        [CallerArgumentExpression(nameof(first))] string? firstArg = null,
        [CallerArgumentExpression(nameof(second))] string? secondArg = null)
        where T : struct, IComparable<T>
    {
        var result = first.CompareTo(second) > 0;

        if (!result)
        {
            throw new XunitException($"Expression '{firstArg}.CompareTo({secondArg}) > 0' evaluates to {result}.");
        }
    }

    public static void CompareToMethod_GreaterThanOrEqual<T>(
        T first,
        T second,
        [CallerArgumentExpression(nameof(first))] string? firstArg = null,
        [CallerArgumentExpression(nameof(second))] string? secondArg = null)
        where T : struct, IComparable<T>
    {
        var result = first.CompareTo(second) >= 0;

        if (!result)
        {
            throw new XunitException($"Expression '{firstArg}.CompareTo({secondArg}) >= 0' evaluates to {result}.");
        }
    }
}
