using System.Reflection;
using System.Runtime.CompilerServices;
using SharpRacer.Extensions.Xunit.Utilities;
using Xunit.Sdk;

namespace SharpRacer.Extensions.Xunit;
public static class OperatorAssert
{
    public static void Equality<T>(
        bool expected,
        T first,
        T second,
        [CallerArgumentExpression(nameof(first))] string firstArg = null!,
        [CallerArgumentExpression(nameof(second))] string secondArg = null!)
    {
        var result = InvokeEqualityOperator(first, second);

        if (result != expected)
        {
            throw new XunitException($"Expression '{firstArg} == {secondArg}' returned {result}.");
        }
    }

    public static void Inequality<T>(
        bool expected,
        T first,
        T second,
        [CallerArgumentExpression(nameof(first))] string firstArg = null!,
        [CallerArgumentExpression(nameof(second))] string secondArg = null!)
    {
        var result = InvokeInequalityOperator(first, second);

        if (result != expected)
        {
            throw new XunitException($"Expression '{firstArg} != {secondArg}' returned {result}.");
        }
    }

    public static void OperandsEqual<T>(
        T first,
        T second,
        [CallerArgumentExpression(nameof(first))] string firstArg = null!,
        [CallerArgumentExpression(nameof(second))] string secondArg = null!)
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
        [CallerArgumentExpression(nameof(first))] string firstArg = null!,
        [CallerArgumentExpression(nameof(second))] string secondArg = null!)
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

    private static MethodInfo GetEqualityOperatorOrThrow<T>()
    {
        var opMethod = OperatorMethods.GetEqualityOperator<T>()
            ?? throw new XunitException($"{typeof(T)} does not implement the equality operator (==).");

        if (opMethod.ReturnType != typeof(bool))
        {
            throw new XunitException(
                $"{typeof(T)} implements the equality operator (==) but the return type is {opMethod.ReturnType} instead of {typeof(bool)}.");
        }

        return opMethod;
    }

    private static MethodInfo GetInequalityOperatorOrThrow<T>()
    {
        var opMethod = OperatorMethods.GetInequalityOperator<T>()
            ?? throw new XunitException($"{typeof(T)} does not implement the inequality operator (!=).");

        if (opMethod.ReturnType != typeof(bool))
        {
            throw new XunitException(
                $"{typeof(T)} implements the inequality operator (!=) but the return type is {opMethod.ReturnType} instead of {typeof(bool)}.");
        }

        return opMethod;
    }

    private static bool InvokeEqualityOperator<T>(T first, T second)
    {
        var operatorMethod = GetEqualityOperatorOrThrow<T>();

        var methodResult = operatorMethod.Invoke(null, [first, second]);

        if (methodResult is not bool operatorResult)
        {
            throw new XunitException(
            $"{typeof(T)} equality operator (==) returned a value of type '{methodResult?.GetType()}' instead of bool.");
        }

        return operatorResult;
    }

    private static bool InvokeInequalityOperator<T>(T first, T second)
    {
        var operatorMethod = GetInequalityOperatorOrThrow<T>();

        var methodResult = operatorMethod.Invoke(null, [first, second]);

        if (methodResult is not bool operatorResult)
        {
            throw new XunitException(
            $"{typeof(T)} inequality operator (!=) returned a value of type '{methodResult?.GetType()}' instead of bool.");
        }

        return operatorResult;
    }
}
