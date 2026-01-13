namespace SharpRacer.Commands;

/// <summary>
/// Defines constants used for sending command messages to the simulator via the SendNotifyMessage Win32 API.
/// </summary>
public static class CommandMessageConstants
{
    /// <summary>
    /// Defines the name of the Win32 window message that the simulator uses to receive command messages.
    /// </summary>
    public const string WindowMessageName = "IRSDK_BROADCASTMSG";

    /// <summary>
    /// Defines constants for single-precision floating-point values used for simulator command arguments.
    /// </summary>
    public static class FloatArgument
    {
        /// <summary>
        /// Represents the smallest floating-point value possible under the simulator command argument packing rules.
        /// </summary>
        public const float MinValue = int.MinValue / ScaleFactor;

        /// <summary>
        /// Represents the largest floating-point value possible under the simulator command argument packing rules.
        /// </summary>
        public const float MaxValue = (int.MaxValue / ScaleFactor) - 1;

        /// <summary>
        /// Represents the scale factor used for converting floating-point arguments to and from <see cref="nint"/> as specified by the
        /// iRacing SDK.
        /// </summary>
        public const float ScaleFactor = (1 << 16);
    }
}
