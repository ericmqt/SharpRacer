﻿using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SharpRacer.Telemetry.Variables;

/// <inheritdoc cref="IDataVariableFactory" />
internal class DataVariableFactory : IDataVariableFactory
{
    private readonly IEnumerable<DataVariableInfo> _dataVariables;

    public DataVariableFactory(IEnumerable<DataVariableInfo> dataVariables)
    {
        _dataVariables = dataVariables ?? throw new ArgumentNullException(nameof(dataVariables));
    }

    /// <inheritdoc />
    public IArrayDataVariable<T> CreateArray<T>(string name, int arrayLength)
        where T : unmanaged
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        if (TryGetDataVariableInfo(name, out var dataVariableInfo))
        {
            return new ArrayDataVariable<T>(dataVariableInfo);
        }

        // Create unavailable variable
        return new ArrayDataVariable<T>(name, arrayLength, variableInfo: null);
    }

    /// <inheritdoc />
    public IScalarDataVariable<T> CreateScalar<T>(string name)
        where T : unmanaged
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        if (!TryGetDataVariableInfo(name, out DataVariableInfo? dataVariableInfo))
        {
            return new ScalarDataVariable<T>(name, null);
        }

        if (dataVariableInfo.ValueCount > 1)
        {
            throw new ArgumentException(
                $"Telemetry variable matching the value of parameter '{nameof(name)}' ({name}) is an array variable.", nameof(name));
        }

        return new ScalarDataVariable<T>(dataVariableInfo);
    }

    /// <inheritdoc />
    public TImplementation CreateType<TImplementation>(string name)
        where TImplementation : class, IDataVariable, ICreateDataVariable<TImplementation>, new()
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        if (!TryGetDataVariableInfo(name, out var dataVariableInfo))
        {
            return new TImplementation();
        }

        return TImplementation.Create(dataVariableInfo);
    }

    internal static bool IsValueTypeMatch<T>(DataVariableValueType valueType)
        where T : unmanaged
    {
        var typeArg = typeof(T);

        if (valueType == DataVariableValueType.Bitfield)
        {
            if (typeArg == typeof(float) || typeArg == typeof(double))
            {
                return false;
            }

            return Unsafe.SizeOf<T>() == Unsafe.SizeOf<int>();
        }

        if (valueType == DataVariableValueType.Bool)
        {
            return typeArg == typeof(bool);
        }

        if (valueType == DataVariableValueType.Byte)
        {
            return typeArg == typeof(byte);
        }

        if (valueType == DataVariableValueType.Int)
        {
            return typeArg == typeof(int);
        }

        if (valueType == DataVariableValueType.Float)
        {
            return typeArg == typeof(float);
        }

        if (valueType == DataVariableValueType.Double)
        {
            return typeArg == typeof(double);
        }

        return false;
    }

    private bool TryGetDataVariableInfo(string name, [NotNullWhen(true)] out DataVariableInfo? dataVariable)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        dataVariable = _dataVariables.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.Ordinal));

        return dataVariable != null;
    }
}
