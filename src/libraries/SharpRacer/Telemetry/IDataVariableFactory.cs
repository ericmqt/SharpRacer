﻿namespace SharpRacer.Telemetry;

/// <summary>
/// Creates strongly-typed telemetry variable objects implementing <see cref="IDataVariable"/> from a simulator session or telemetry file.
/// </summary>
/// <remarks>
/// If a given telemetry variable is not available in the associated simulator session or telemetry file, a placeholder is created which
/// cannot be read from, denoted by <see cref="IDataVariable.IsAvailable"/>.
/// </remarks>
public interface IDataVariableFactory
{
    /// <summary>
    /// Creates an instance of <see cref="IDataVariable{T}"/> that represents the telemetry variable with the specified name.
    /// </summary>
    /// <typeparam name="T">The telemetry variable value type.</typeparam>
    /// <param name="name">The name of the telemetry variable.</param>
    /// <returns>An instance of <see cref="IDataVariable{T}"/> that represents the telemetry variable.</returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="name"/> is an empty string.
    /// 
    /// -OR-
    /// 
    /// The telemetry variable matched by <paramref name="name"/> is an array variable.
    /// 
    /// -OR-
    /// 
    /// Type parameter <typeparamref name="T"/> is not compatible with the telemetry variable matched by <paramref name="name"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null" />.</exception>
    IDataVariable<T> Create<T>(string name)
        where T : unmanaged;

    /// <summary>
    /// Creates an instance of <see cref="IArrayDataVariable{T}"/> that represents the array telemetry variable with the specified name
    /// and array length.
    /// </summary>
    /// <typeparam name="T">The telemetry variable array element type.</typeparam>
    /// <param name="name">The name of the telemetry variable.</param>
    /// <param name="arrayLength">The length of the array of values represented by the telemetry variable.</param>
    /// <param name="isTimeSliceArray">If <see langword="true" />, the array represents a single value over time.</param>
    /// <returns>An instance of <see cref="IArrayDataVariable{T}"/> that represents the specified array telemetry variable.</returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="name"/> is an empty string.
    /// 
    /// -OR-
    /// 
    /// <paramref name="arrayLength"/> does not match the telemetry variable obtained from the current simulator session or telemetry file.
    /// 
    /// -OR-
    /// 
    /// <paramref name="isTimeSliceArray"/> does not match telemetry variable obtained from the current simulator session or telemetry file.
    /// 
    /// -OR-
    /// 
    /// Type parameter <typeparamref name="T"/> is not compatible with the telemetry variable matched by <paramref name="name"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayLength"/> is not greater than 1.</exception>
    IArrayDataVariable<T> CreateArray<T>(string name, int arrayLength, bool isTimeSliceArray)
        where T : unmanaged;

    /// <summary>
    /// Creates an instance of <typeparamref name="TImplementation"/> that represents the telemetry variable with the specified name.
    /// </summary>
    /// <typeparam name="TImplementation">A type implementing <see cref="IDataVariable{T}"/> and <see cref="ICreateDataVariable{TSelf, T}"/>.</typeparam>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns>An instance of <typeparamref name="TImplementation"/> that represents the specified telemetry variable.</returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="name"/> is an empty string.
    /// 
    /// -OR-
    /// 
    /// The telemetry variable matched by <paramref name="name"/> is an array variable.
    /// 
    /// -OR-
    /// 
    /// Type parameter <typeparamref name="T"/> is not compatible with the telemetry variable matched by <paramref name="name"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null" />.</exception>
    TImplementation Create<TImplementation, T>(string name)
        where TImplementation : IDataVariable<T>, ICreateDataVariable<TImplementation, T>
        where T : unmanaged;

    /// <summary>
    /// Creates an instance of <typeparamref name="TImplementation"/> that represents the array telemetry variable with the specified name
    /// and array length.
    /// </summary>
    /// <typeparam name="TImplementation"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <param name="name">The name of the telemetry variable.</param>
    /// <param name="arrayLength">The length of the array of values represented by the telemetry variable.</param>
    /// <param name="isTimeSliceArray">If <see langword="true" />, the array represents a single value over time.</param>
    /// <returns>An instance of <typeparamref name="TImplementation"/> that represents the specified array telemetry variable.</returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="name"/> is an empty string.
    /// 
    /// -OR-
    /// 
    /// <paramref name="arrayLength"/> does not match the telemetry variable obtained from the current simulator session or telemetry file.
    /// 
    /// -OR-
    /// 
    /// <paramref name="isTimeSliceArray"/> does not match telemetry variable obtained from the current simulator session or telemetry file.
    /// 
    /// -OR-
    /// 
    /// Type parameter <typeparamref name="T"/> is not compatible with the telemetry variable matched by <paramref name="name"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayLength"/> is not greater than 1.</exception>
    TImplementation CreateArray<TImplementation, T>(string name, int arrayLength, bool isTimeSliceArray)
        where TImplementation : IArrayDataVariable<T>, ICreateArrayDataVariable<TImplementation, T>
        where T : unmanaged;
}