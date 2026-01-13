using System.Diagnostics.CodeAnalysis;
using SharpRacer.Interop;

namespace SharpRacer.Commands;

/// <summary>
/// Represents a simulator command and its arguments.
/// </summary>
public readonly struct CommandMessage : IEquatable<CommandMessage>
{
    /// <summary>
    /// Initializes a new <see cref="CommandMessage"/> instance from the specified values.
    /// </summary>
    /// <param name="commandId">The value that identifies the command.</param>
    public CommandMessage(ushort commandId)
    {
        CommandId = commandId;
    }

    /// <summary>
    /// Initializes a new <see cref="CommandMessage"/> instance from the specified values.
    /// </summary>
    /// <param name="commandId">The value that identifies the command.</param>
    /// <param name="arg1">The command argument.</param>
    public CommandMessage(ushort commandId, ushort arg1)
    {
        CommandId = commandId;
        Arg1 = arg1;
    }

    /// <summary>
    /// Initializes a new <see cref="CommandMessage"/> instance from the specified values.
    /// </summary>
    /// <param name="commandId">The value that identifies the command.</param>
    /// <param name="arg1">The first command argument.</param>
    /// <param name="arg2">The second command argument.</param>
    public CommandMessage(ushort commandId, ushort arg1, bool arg2)
    {
        CommandId = commandId;
        Arg1 = arg1;
        Arg2 = (ushort)(arg2 ? 1 : 0);
    }

    /// <summary>
    /// Initializes a new <see cref="CommandMessage"/> instance from the specified values.
    /// </summary>
    /// <param name="commandId">The value that identifies the command.</param>
    /// <param name="arg1">The first command argument.</param>
    /// <param name="arg2">The second command argument.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="arg2"/> is less than <see cref="CommandMessageConstants.FloatArgument.MinValue"/> or greater than
    /// <see cref="CommandMessageConstants.FloatArgument.MaxValue"/>.
    /// </exception>
    public CommandMessage(ushort commandId, ushort arg1, float arg2)
    {
        CommandId = commandId;
        Arg1 = arg1;

        var real = (int)(arg2 * CommandMessageConstants.FloatArgument.ScaleFactor);

        Arg2 = (ushort)(real >> 16);
        Arg3 = (ushort)(real & 0xFFFF);
    }

    /// <summary>
    /// Initializes a new <see cref="CommandMessage"/> instance from the specified values.
    /// </summary>
    /// <param name="commandId">The value that identifies the command.</param>
    /// <param name="arg1">The first command argument.</param>
    /// <param name="arg2">The second command argument.</param>
    public CommandMessage(ushort commandId, ushort arg1, int arg2)
    {
        CommandId = commandId;
        Arg1 = arg1;
        Arg2 = (ushort)(arg2 >> 16);
        Arg3 = (ushort)(arg2 & 0xFFFF);
    }

    /// <summary>
    /// Initializes a new <see cref="CommandMessage"/> instance from the specified values.
    /// </summary>
    /// <param name="commandId">The value that identifies the command.</param>
    /// <param name="arg1">The first command argument.</param>
    /// <param name="arg2">The second command argument.</param>
    public CommandMessage(ushort commandId, ushort arg1, ushort arg2)
    {
        CommandId = commandId;
        Arg1 = arg1;
        Arg2 = arg2;
    }

    /// <summary>
    /// Initializes a new <see cref="CommandMessage"/> instance from the specified values.
    /// </summary>
    /// <param name="commandId">The value that identifies the command.</param>
    /// <param name="arg1">The first command argument.</param>
    /// <param name="arg2">The second command argument.</param>
    /// <param name="arg3">The third command argument.</param>
    public CommandMessage(ushort commandId, ushort arg1, ushort arg2, ushort arg3)
    {
        CommandId = commandId;
        Arg1 = arg1;
        Arg2 = arg2;
        Arg3 = arg3;
    }

    /// <summary>
    /// Initializes a new <see cref="CommandMessage"/> instance from the specified <see cref="SimulatorNotifyMessageData"/>.
    /// </summary>
    /// <param name="notifyMessageData"></param>
    public CommandMessage(SimulatorNotifyMessageData notifyMessageData)
    {
        CommandId = (ushort)(notifyMessageData.WParam & 0xFFFF);
        Arg1 = (ushort)((notifyMessageData.WParam >> 16) & 0xFFFF);
        Arg2 = (ushort)((notifyMessageData.LParam >> 16) & 0xFFFF);
        Arg3 = (ushort)(notifyMessageData.LParam & 0xFFFF);
    }

    /// <summary>
    /// Gets the value of the first command argument.
    /// </summary>
    public readonly ushort Arg1 { get; }

    /// <summary>
    /// Gets the value of the second command argument. When a command uses a 32-bit value for its second argument, the value of this
    /// property is the upper half of the 32-bit argument.
    /// </summary>
    public readonly ushort Arg2 { get; }

    /// <summary>
    /// Gets the value of the third command argument. When a command uses a 32-bit value for its second argument, the value of this
    /// property is the lower half of the 32-bit argument.
    /// </summary>
    public readonly ushort Arg3 { get; }

    /// <summary>
    /// Gets the value that identifies the command represented by this instance.
    /// </summary>
    public readonly ushort CommandId { get; }

    /// <summary>
    /// Packs the command into a <see cref="SimulatorNotifyMessageData"/> value.
    /// </summary>
    /// <returns></returns>
    public readonly SimulatorNotifyMessageData ToNotifyMessage()
    {
        var wParam = ((uint)Arg1 << 16) | CommandId;

        var lParam = (Arg2 << 16) | Arg3;

        return new SimulatorNotifyMessageData(wParam, lParam);
    }

    /// <inheritdoc />
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is CommandMessage message && Equals(message);
    }

    /// <inheritdoc />
    public readonly bool Equals(CommandMessage other)
    {
        return CommandId == other.CommandId &&
               Arg1 == other.Arg1 &&
               Arg2 == other.Arg2 &&
               Arg3 == other.Arg3;
    }

    /// <inheritdoc />
    public override readonly int GetHashCode()
    {
        return HashCode.Combine(CommandId, Arg1, Arg2, Arg3);
    }

    /// <inheritdoc />
    public static bool operator ==(CommandMessage left, CommandMessage right)
    {
        return left.Equals(right);
    }

    /// <inheritdoc />
    public static bool operator !=(CommandMessage left, CommandMessage right)
    {
        return !(left == right);
    }
}
