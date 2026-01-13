using System.Diagnostics.CodeAnalysis;

namespace SharpRacer.Commands.Replay;

/// <summary>
/// Represents a simulator command that performs a search on the replay tape for a specified event.
/// </summary>
public readonly struct SearchCommand : ISimulatorCommand<SearchCommand>
{
    /// <summary>
    /// Initializes a new <see cref="SearchCommand"/> instance with the specified search mode.
    /// </summary>
    /// <param name="searchMode"></param>
    public SearchCommand(ReplaySearchMode searchMode)
    {
        SearchMode = searchMode;
    }

    /// <inheritdoc />
    public static ushort CommandId { get; } = (ushort)SimulatorCommandId.ReplaySearch;

    /// <summary>
    /// Gets the replay tape event to search for when the command is executed.
    /// </summary>
    public readonly ReplaySearchMode SearchMode { get; }

    /// <inheritdoc />
    public readonly CommandMessage ToCommandMessage()
    {
        return new CommandMessage(CommandId, (ushort)SearchMode);
    }

    /// <inheritdoc />
    public static SearchCommand Parse(CommandMessage message)
    {
        CommandMessageParseException.ThrowIfCommandIdNotEqual(message, CommandId);

        var reader = new CommandMessageReader(ref message);

        ReplaySearchMode searchMode;

        try { searchMode = reader.ReadArg1<ReplaySearchMode>(); }
        catch (CommandMessageReaderException ex)
        {
            throw CommandMessageParseException.Arg1ReaderException(ex);
        }

        return new SearchCommand(searchMode);
    }

    /// <inheritdoc />
    public static bool TryParse(CommandMessage message, out SearchCommand result)
    {
        if (message.CommandId != CommandId)
        {
            result = default;
            return false;
        }

        var reader = new CommandMessageReader(ref message);

        if (!reader.TryReadArg1<ReplaySearchMode>(out var searchMode))
        {
            result = default;
            return false;
        }

        result = new SearchCommand(searchMode);
        return true;
    }

    /// <inheritdoc />
    public static implicit operator CommandMessage(SearchCommand command)
    {
        return command.ToCommandMessage();
    }

    #region IEquatable Implementation

    /// <inheritdoc />
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is SearchCommand command && Equals(command);
    }

    /// <inheritdoc />
    public readonly bool Equals(SearchCommand other)
    {
        return SearchMode == other.SearchMode;
    }

    /// <inheritdoc />
    public override readonly int GetHashCode()
    {
        return HashCode.Combine(SearchMode);
    }

    /// <inheritdoc />
    public static bool operator ==(SearchCommand left, SearchCommand right)
    {
        return left.Equals(right);
    }

    /// <inheritdoc />
    public static bool operator !=(SearchCommand left, SearchCommand right)
    {
        return !(left == right);
    }

    #endregion IEquatable Implementation
}
