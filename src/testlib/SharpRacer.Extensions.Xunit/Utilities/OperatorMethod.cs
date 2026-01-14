using System.Reflection;
using System.Runtime.CompilerServices;
using Xunit.Sdk;

namespace SharpRacer.Extensions.Xunit.Utilities;
internal class OperatorMethod
{
    public OperatorMethod(string methodName, string operatorExpression)
    {
        MethodName = methodName;
        OperatorExpression = operatorExpression;
    }

    public string MethodName { get; }
    public string OperatorExpression { get; }

    public void Assert<T>(
        bool expected,
        T first,
        T second,
        [CallerArgumentExpression(nameof(first))] string? firstArg = null,
        [CallerArgumentExpression(nameof(second))] string? secondArg = null)
    {
        var result = Invoke(first, second);

        if (result != expected)
        {
            throw new XunitException($"Expression '{firstArg} {OperatorExpression} {secondArg}' returned {result}. Expected: {expected}");
        }
    }

    public MethodInfo GetOrThrow<T>()
    {
        var opMethod = typeof(T).GetMethod(MethodName, BindingFlags.Static | BindingFlags.Public);

        if (opMethod is null)
        {
            throw new XunitException($"{typeof(T)} does not implement operator '{MethodName}' ({OperatorExpression}).");
        }

        if (opMethod.ReturnType != typeof(bool))
        {
            throw new XunitException(
                $"{typeof(T)} implements operator '{MethodName}' but the return type is '{opMethod.ReturnType}' instead of '{typeof(bool)}'.");
        }

        return opMethod;
    }

    public bool Invoke<T>(T first, T second)
    {
        return (bool)GetOrThrow<T>().Invoke(null, [first, second])!;
    }
}
