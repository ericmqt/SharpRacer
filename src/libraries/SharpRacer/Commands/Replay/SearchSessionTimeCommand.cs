using System.Diagnostics.CodeAnalysis;

namespace SharpRacer.Commands.Replay;

/// <summary>
/// Represents a simulator command that searches the replay tape for a specific time in a specific session.
/// </summary>
public readonly struct SearchSessionTimeCommand : ISimulatorCommand<SearchSessionTimeCommand>
{
    /// <summary>
    /// Initializes a new <see cref="SearchSessionTimeCommand"/> instance with the specified session number and session time.
    /// </summary>
    /// <param name="sessionNumber">The session number to search for.</param>
    /// <param name="sessionTimeMs">The session elapsed time, in milliseconds, to seek.</param>
    public SearchSessionTimeCommand(ushort sessionNumber, int sessionTimeMs)
    {
        // TODO: Validate!

        SessionNumber = sessionNumber;
        SessionTimeMs = sessionTimeMs;
    }

    /// <inheritdoc />
    public static ushort CommandId { get; } = (ushort)SimulatorCommandId.ReplaySearchSessionTime;

    /// <summary>
    /// Gets the session number to search.
    /// </summary>
    public readonly ushort SessionNumber { get; }

    /// <summary>
    /// Gets the elapsed session time to seek within the session indicated by <see cref="SessionNumber">SessionNumber</see>.
    /// </summary>
    public readonly int SessionTimeMs { get; }

    /// <inheritdoc />
    public readonly CommandMessage ToCommandMessage()
    {
        return new CommandMessage(CommandId, SessionNumber, SessionTimeMs);
    }

    /// <inheritdoc />
    public static SearchSessionTimeCommand Parse(CommandMessage message)
    {
        CommandMessageParseException.ThrowIfCommandIdNotEqual(message, CommandId);

        var reader = new CommandMessageReader(ref message);

        var sessionNumber = reader.ReadArg1();
        var sessionTimeMs = reader.ReadArg2Int();

        return new SearchSessionTimeCommand(sessionNumber, sessionTimeMs);
    }

    /// <inheritdoc />
    public static bool TryParse(CommandMessage message, out SearchSessionTimeCommand result)
    {
        if (message.CommandId != CommandId)
        {
            result = default;
            return false;
        }

        var reader = new CommandMessageReader(ref message);

        var sessionNumber = reader.ReadArg1();
        var sessionTimeMs = reader.ReadArg2Int();

        result = new SearchSessionTimeCommand(sessionNumber, sessionTimeMs);
        return true;
    }

    /// <inheritdoc />
    public static implicit operator CommandMessage(SearchSessionTimeCommand command)
    {
        return command.ToCommandMessage();
    }

    #region IEquatable Implementation

    /// <inheritdoc />
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is SearchSessionTimeCommand command && Equals(command);
    }

    /// <inheritdoc />
    public readonly bool Equals(SearchSessionTimeCommand other)
    {
        return SessionTimeMs == other.SessionTimeMs &&
               SessionNumber == other.SessionNumber;
    }

    /// <inheritdoc />
    public override readonly int GetHashCode()
    {
        return HashCode.Combine(SessionTimeMs, SessionNumber);
    }

    /// <inheritdoc />
    public static bool operator ==(SearchSessionTimeCommand left, SearchSessionTimeCommand right)
    {
        return left.Equals(right);
    }

    /// <inheritdoc />
    public static bool operator !=(SearchSessionTimeCommand left, SearchSessionTimeCommand right)
    {
        return !(left == right);
    }

    #endregion IEquatable Implementation
}
