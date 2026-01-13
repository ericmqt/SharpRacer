using System.Diagnostics.CodeAnalysis;

namespace SharpRacer.Commands.Chat;

/// <summary>
/// Represents a simulator command that executes a chat macro.
/// </summary>
public readonly struct MacroCommand : ISimulatorCommand<MacroCommand>
{
    /// <summary>
    /// Defines the smallest valid macro ID.
    /// </summary>
    public const ushort MinMacroId = 1;

    /// <summary>
    /// Defines the largest valid macro ID.
    /// </summary>
    public const ushort MaxMacroId = 15;

    /// <summary>
    /// Initializes a new <see cref="MacroCommand"/> instance with the specified macro ID.
    /// </summary>
    /// <param name="macroId">The ID of the macro to execute. Valid values are 1 through 15.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="macroId"/> is less than 1 or greater than 15.
    /// </exception>
    public MacroCommand(int macroId)
        : this((ushort)macroId)
    {

    }

    private MacroCommand(ushort macroId)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(macroId, MinMacroId);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(macroId, MaxMacroId);

        MacroId = macroId;
    }

    /// <inheritdoc />
    public static ushort CommandId { get; } = (ushort)SimulatorCommandId.ChatCommand;

    /// <summary>
    /// Gets the ID of the macro to execute.
    /// </summary>
    public readonly ushort MacroId { get; }

    /// <inheritdoc />
    public readonly CommandMessage ToCommandMessage()
    {
        return new CommandMessage(CommandId, (ushort)ChatCommandType.Macro, MacroId);
    }

    /// <inheritdoc />
    public static MacroCommand Parse(CommandMessage message)
    {
        CommandMessageParseException.ThrowIfCommandIdNotEqual(message, CommandId);

        var reader = new CommandMessageReader(ref message);

        ChatCommandType chatCommandType;

        try { chatCommandType = reader.ReadArg1<ChatCommandType>(); }
        catch (CommandMessageReaderException ex)
        {
            throw CommandMessageParseException.Arg1ReaderException(ex);
        }

        if (chatCommandType != ChatCommandType.Macro)
        {
            throw CommandMessageParseException.InvalidArg1Value(ChatCommandType.Macro, chatCommandType);
        }

        var macroId = reader.ReadArg2();

        if (macroId < MinMacroId || macroId > MaxMacroId)
        {
            throw new CommandMessageParseException(
                $"{nameof(CommandMessage)} has an invalid macro ID: {macroId}. The value must be greater than {MinMacroId} and less than {MaxMacroId}.");
        }

        return new MacroCommand(macroId);
    }

    /// <inheritdoc />
    public static bool TryParse(CommandMessage message, out MacroCommand result)
    {
        if (message.CommandId != CommandId)
        {
            result = default;
            return false;
        }

        var reader = new CommandMessageReader(ref message);

        if (!reader.TryReadArg1<ChatCommandType>(out var chatCommandType) || chatCommandType != ChatCommandType.Macro)
        {
            result = default;
            return false;
        }

        var macroId = reader.ReadArg2();

        if (macroId < MinMacroId || macroId > MaxMacroId)
        {
            result = default;
            return false;
        }

        result = new MacroCommand(macroId);
        return true;
    }

    /// <inheritdoc />
    public static implicit operator CommandMessage(MacroCommand command)
    {
        return command.ToCommandMessage();
    }

    #region IEquatable Implementation

    /// <inheritdoc />
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is MacroCommand command && Equals(command);
    }

    /// <inheritdoc />
    public readonly bool Equals(MacroCommand other)
    {
        return MacroId == other.MacroId;
    }

    /// <inheritdoc />
    public override readonly int GetHashCode()
    {
        return HashCode.Combine(MacroId);
    }

    /// <inheritdoc />
    public static bool operator ==(MacroCommand left, MacroCommand right)
    {
        return left.Equals(right);
    }

    /// <inheritdoc />
    public static bool operator !=(MacroCommand left, MacroCommand right)
    {
        return !(left == right);
    }

    #endregion IEquatable Implementation
}
