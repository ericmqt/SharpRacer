using System.Runtime.InteropServices;

namespace SharpRacer.Telemetry.Variables;
/// <summary>
/// Provides a type-safe representation of a telemetry variable whose value is a single value of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The telemetry variable value type.</typeparam>
public class ScalarDataVariable<T> : DataVariableBase<T>, IScalarDataVariable<T>
    where T : unmanaged
{
    /// <summary>
    /// Initializes an instance of <see cref="ScalarDataVariable{T}"/> from the specified <see cref="DataVariableInfo"/>.
    /// </summary>
    /// <param name="variableInfo">
    /// The <see cref="DataVariableInfo"/> from which to initialize the <see cref="ScalarDataVariable{T}"/> instance.
    /// </param>
    /// <exception cref="ArgumentException">
    /// <paramref name="variableInfo"/> represents an array variable with a value for property <see cref="DataVariableInfo.ValueCount"/> not equal to one.
    /// </exception>
    /// <exception cref="ArgumentNullException"><paramref name="variableInfo"/> is <see langword="null" />.</exception>
    /// <exception cref="DataVariableInitializationException">
    /// Type parameter <typeparamref name="T"/> is not compatible with the value type specified by <paramref name="variableInfo"/>.
    /// </exception>
    public ScalarDataVariable(DataVariableInfo variableInfo)
        : base(variableInfo)
    {
        if (variableInfo.ValueCount > 1)
        {
            throw new ArgumentException($"Value '{nameof(variableInfo)}' represents an array variable.", nameof(variableInfo));
        }
    }

    /// <summary>
    /// Initializes an instance of <see cref="ScalarDataVariable{T}"/> from the specified variable name that represents a telemetry variable
    /// that is not available in the current context.
    /// </summary>
    /// <param name="name">The telemetry variable name.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is an empty string.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null" />.</exception>
    public ScalarDataVariable(string name)
        : this(name, variableInfo: null)
    {

    }

    /// <summary>
    /// Initializes an instance of <see cref="ScalarDataVariable{T}"/> from the specified variable name and optional <see cref="DataVariableInfo"/>.
    /// </summary>
    /// <param name="name">The telemetry variable name.</param>
    /// <param name="variableInfo">
    /// The <see cref="DataVariableInfo"/> with which to initialize the instance. If <see langword="null" />, the resulting object
    /// represents a variable that is unavailable in the current context.
    /// </param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is an empty string.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null" />.</exception>
    /// <exception cref="DataVariableInitializationException">
    /// <paramref name="name"/> is not equal to the variable name specified by <paramref name="variableInfo"/>.
    /// 
    /// -OR-
    /// 
    /// <paramref name="variableInfo"/> has a value for property <see cref="DataVariableInfo.ValueCount"/> that is not equal to one.
    /// 
    /// -OR-
    /// 
    /// Type parameter <typeparamref name="T"/> is not compatible with the value type specified by <paramref name="variableInfo"/>.
    /// </exception>
    protected internal ScalarDataVariable(string name, DataVariableInfo? variableInfo)
        : base(name, 1, variableInfo)
    {
    }

    /// <summary>
    /// Initializes an instance of <see cref="ScalarDataVariable{T}"/> from the specified variable descriptor and optional <see cref="DataVariableInfo"/>.
    /// </summary>
    /// <param name="variableDescriptor">
    /// The variable descriptor which provides required values in the event that <paramref name="variableInfo"/> is <see langword="null"/>
    /// (i.e. the variable is not available in the current context).
    /// </param>
    /// <param name="variableInfo">
    /// The <see cref="DataVariableInfo"/> with which to initialize the instance. If <see langword="null" />, the resulting object
    /// represents a variable that is unavailable in the current context.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="variableDescriptor"/> is <see langword="null" />.</exception>
    /// <exception cref="DataVariableInitializationException">
    /// The variable names specified by <paramref name="variableDescriptor"/> and <paramref name="variableInfo"/> do not match.
    /// 
    /// -OR-
    /// 
    /// The variable value counts specified by <paramref name="variableDescriptor"/> and <paramref name="variableInfo"/> do not match or
    /// are greater than one.
    /// 
    /// -OR-
    /// 
    /// Type parameter <typeparamref name="T"/> is not compatible with the value type specified by either
    /// <paramref name="variableDescriptor"/> or <paramref name="variableInfo"/>.
    /// </exception>
    protected internal ScalarDataVariable(DataVariableDescriptor variableDescriptor, DataVariableInfo? variableInfo)
        : base(variableDescriptor, variableInfo)
    {

    }

    /// <inheritdoc />
    public T Read(ReadOnlySpan<byte> source)
    {
        ThrowIfUnavailable();

        var valueBytes = GetDataSpan(source);

        return MemoryMarshal.Read<T>(valueBytes);
    }
}
