using System.Diagnostics.CodeAnalysis;

namespace SharpRacer.Interop;

/// <summary>
/// Represents the contents of a message that can be sent to the simulator via the SendNotifyMessage Win32 API.
/// </summary>
public readonly struct SimulatorNotifyMessageData : IEquatable<SimulatorNotifyMessageData>
{
    /// <summary>
    /// Initializes a new <see cref="SimulatorNotifyMessageData"/> instance using the specified parameters.
    /// </summary>
    /// <param name="wParam">The WPARAM value of the Win32 window message.</param>
    /// <param name="lParam">The LPARAM value of the Win32 window message.</param>
    public SimulatorNotifyMessageData(nuint wParam, nint lParam)
    {
        WParam = wParam;
        LParam = lParam;
    }

    /// <summary>
    /// Gets the LPARAM value of the Win32 window message as a native signed integer.
    /// </summary>
    public readonly nint LParam { get; }

    /// <summary>
    /// Gets the WPARAM value of the Win32 window message as a native unsigned integer.
    /// </summary>
    public readonly nuint WParam { get; }

    /// <inheritdoc />
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is SimulatorNotifyMessageData data && Equals(data);
    }

    /// <inheritdoc />
    public readonly bool Equals(SimulatorNotifyMessageData other)
    {
        return WParam.Equals(other.WParam) &&
               LParam.Equals(other.LParam);
    }

    /// <inheritdoc />
    public override readonly int GetHashCode()
    {
        return HashCode.Combine(WParam, LParam);
    }

    /// <inheritdoc />
    public static bool operator ==(SimulatorNotifyMessageData left, SimulatorNotifyMessageData right)
    {
        return left.Equals(right);
    }

    /// <inheritdoc />
    public static bool operator !=(SimulatorNotifyMessageData left, SimulatorNotifyMessageData right)
    {
        return !(left == right);
    }
}
