using System.Runtime.InteropServices;

namespace SharpRacer.Telemetry;
/// <summary>
/// Provides a type-safe representation of a telemetry variable whose value is a single value of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The telemetry variable value type.</typeparam>
public class ScalarTelemetryVariable<T> : TelemetryVariableBase<T>, IScalarTelemetryVariable<T>
    where T : unmanaged
{
    /// <summary>
    /// Initializes an instance of <see cref="ScalarTelemetryVariable{T}"/> from the specified <see cref="TelemetryVariableInfo"/>.
    /// </summary>
    /// <param name="variableInfo">
    /// The <see cref="TelemetryVariableInfo"/> from which to initialize the <see cref="ScalarTelemetryVariable{T}"/> instance.
    /// </param>
    /// <exception cref="ArgumentException">
    /// <paramref name="variableInfo"/> represents an array variable with a value for property <see cref="TelemetryVariableInfo.ValueCount"/> not equal to one.
    /// </exception>
    /// <exception cref="ArgumentNullException"><paramref name="variableInfo"/> is <see langword="null" />.</exception>
    /// <exception cref="TelemetryVariableInitializationException">
    /// Type parameter <typeparamref name="T"/> is not compatible with the value type specified by <paramref name="variableInfo"/>.
    /// </exception>
    public ScalarTelemetryVariable(TelemetryVariableInfo variableInfo)
        : base(variableInfo)
    {
        if (variableInfo.ValueCount > 1)
        {
            throw new ArgumentException($"Value '{nameof(variableInfo)}' represents an array variable.", nameof(variableInfo));
        }
    }

    /// <summary>
    /// Initializes an instance of <see cref="ScalarTelemetryVariable{T}"/> from the specified variable name and optional <see cref="TelemetryVariableInfo"/>.
    /// </summary>
    /// <param name="name">The telemetry variable name.</param>
    /// <param name="variableInfo">
    /// The <see cref="TelemetryVariableInfo"/> with which to initialize the instance. If <see langword="null" />, the resulting object
    /// represents a variable that is unavailable in the current context.
    /// </param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is an empty string.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null" />.</exception>
    /// <exception cref="TelemetryVariableInitializationException">
    /// <paramref name="name"/> is not equal to the variable name specified by <paramref name="variableInfo"/>.
    /// 
    /// -OR-
    /// 
    /// <paramref name="variableInfo"/> has a value for property <see cref="TelemetryVariableInfo.ValueCount"/> that is not equal to one.
    /// 
    /// -OR-
    /// 
    /// Type parameter <typeparamref name="T"/> is not compatible with the value type specified by <paramref name="variableInfo"/>.
    /// </exception>
    public ScalarTelemetryVariable(string name, TelemetryVariableInfo? variableInfo)
        : base(name, 1, variableInfo)
    {
    }

    /// <summary>
    /// Initializes an instance of <see cref="ScalarTelemetryVariable{T}"/> from the specified variable descriptor and optional <see cref="TelemetryVariableInfo"/>.
    /// </summary>
    /// <param name="variableDescriptor">
    /// The variable descriptor which provides required values in the event that <paramref name="variableInfo"/> is <see langword="null"/>
    /// (i.e. the variable is not available in the current context).
    /// </param>
    /// <param name="variableInfo">
    /// The <see cref="TelemetryVariableInfo"/> with which to initialize the instance. If <see langword="null" />, the resulting object
    /// represents a variable that is unavailable in the current context.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="variableDescriptor"/> is <see langword="null" />.</exception>
    /// <exception cref="TelemetryVariableInitializationException">
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
    public ScalarTelemetryVariable(TelemetryVariableDescriptor variableDescriptor, TelemetryVariableInfo? variableInfo)
        : base(variableDescriptor, variableInfo)
    {

    }

    /// <summary>
    /// Initializes an instance of <see cref="ScalarTelemetryVariable{T}"/> from the specified variable descriptor.
    /// </summary>
    /// <param name="variableDescriptor">The variable descriptor.</param>
    /// <param name="variableInfoProvider">
    /// The <see cref="ITelemetryVariableInfoProvider"/> instance used to notify this instance when the telemetry variable becomes available in
    /// the data source.
    /// </param>
    public ScalarTelemetryVariable(TelemetryVariableDescriptor variableDescriptor, ITelemetryVariableInfoProvider variableInfoProvider)
        : base(variableDescriptor, variableInfoProvider)
    {

    }

    /// <inheritdoc />
    public T Read(ReadOnlySpan<byte> source)
    {
        ThrowIfUnavailable();

        return MemoryMarshal.Read<T>(GetDataSpan(source));
    }
}
