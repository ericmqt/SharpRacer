using System.Runtime.CompilerServices;
using Xunit.Sdk;

namespace SharpRacer.Extensions.Xunit;
public static class EquatableStructAssert
{
    public static void Equal<T>(
        T first,
        T second,
        [CallerArgumentExpression(nameof(first))] string firstArg = null!,
        [CallerArgumentExpression(nameof(second))] string secondArg = null!)
        where T : struct, IEquatable<T>
    {
        EqualsMethod(true, first, second, firstArg, secondArg);
        HashCodeEqual(first, second, firstArg, secondArg);
        ObjectEqualsMethod(true, first, (object?)second, firstArg, secondArg);

        OperatorAssert.OperandsEqual(first, second, firstArg, secondArg);

        // Swap operands
        EqualsMethod(true, second, first, secondArg, firstArg);
        ObjectEqualsMethod(true, second, (object?)first, secondArg, firstArg);

        OperatorAssert.OperandsEqual(second, first, secondArg, firstArg);
    }

    public static void NotEqual<T>(
        T first,
        T second,
        [CallerArgumentExpression(nameof(first))] string firstArg = null!,
        [CallerArgumentExpression(nameof(second))] string secondArg = null!)
        where T : struct, IEquatable<T>
    {
        EqualsMethod(false, first, second, firstArg, secondArg);
        HashCodeNotEqual(first, second, firstArg, secondArg);
        ObjectEqualsMethod(false, first, (object?)second, firstArg, secondArg);

        OperatorAssert.OperandsNotEqual(first, second, firstArg, secondArg);

        // Swap operands
        EqualsMethod(false, second, first, secondArg, firstArg);
        ObjectEqualsMethod(false, second, (object?)first, secondArg, firstArg);

        OperatorAssert.OperandsNotEqual(second, first, secondArg, firstArg);
    }

    public static void EqualsMethod<T>(
        bool expected,
        T first,
        T second,
        [CallerArgumentExpression(nameof(first))] string firstArg = null!,
        [CallerArgumentExpression(nameof(second))] string secondArg = null!)
        where T : struct, IEquatable<T>
    {
        var result = first.Equals(second);

        if (result != expected)
        {
            throw new XunitException($"Return value of {firstArg}.Equals({secondArg}) is {result} but expected {expected}.");
        }
    }

    public static void HashCode<T>(
        bool expectEqual,
        T first,
        T second,
        [CallerArgumentExpression(nameof(first))] string firstArg = null!,
        [CallerArgumentExpression(nameof(second))] string secondArg = null!)
        where T : struct
    {
        var hashCodeCompare = first.GetHashCode() == second.GetHashCode();

        if (hashCodeCompare != expectEqual)
        {
            throw new XunitException(
                $"Expression '{firstArg}.GetHashCode() == {secondArg}.GetHashCode()' returned {hashCodeCompare} but expected {expectEqual}.");
        }
    }

    public static void HashCodeEqual<T>(
        T first,
        T second,
        [CallerArgumentExpression(nameof(first))] string firstArg = null!,
        [CallerArgumentExpression(nameof(second))] string secondArg = null!)
        where T : struct
    {
        if (first.GetHashCode() != second.GetHashCode())
        {
            throw new XunitException($"Return values of {firstArg}.GetHashCode() and {secondArg}.GetHashCode() are not equal.");
        }
    }

    public static void HashCodeNotEqual<T>(
        T first,
        T second,
        [CallerArgumentExpression(nameof(first))] string firstArg = null!,
        [CallerArgumentExpression(nameof(second))] string secondArg = null!)
        where T : struct
    {
        if (first.GetHashCode() == second.GetHashCode())
        {
            throw new XunitException($"Return values of {firstArg}.GetHashCode() and {secondArg}.GetHashCode() are equal.");
        }
    }

    public static void ObjectEqualsMethod<T>(
        bool expected,
        T value,
        object? comparison,
        [CallerArgumentExpression(nameof(value))] string valueArg = null!,
        [CallerArgumentExpression(nameof(comparison))] string comparisonArg = null!)
        where T : struct
    {
        var result = value.Equals(obj: comparison);

        if (result != expected)
        {
            throw new XunitException($"Return value of {valueArg}.Equals(obj: {comparisonArg}) is {result} but expected {expected}.");
        }
    }
}
