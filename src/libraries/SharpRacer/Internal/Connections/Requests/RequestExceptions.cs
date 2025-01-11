using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SharpRacer.Internal.Connections.Requests;
internal static class RequestExceptions
{
    internal static TimeoutException RequestTimeout()
    {
        return new TimeoutException("The timeout period elapsed before a connection could be established.");
    }

    [StackTraceHidden]
    internal static TimeSpan ThrowIfTimeoutArgumentIsNegative(TimeSpan timeout, [CallerArgumentExpression(nameof(timeout))] string? paramName = null)
    {
        if (timeout != Timeout.InfiniteTimeSpan && timeout < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                paramName, $"'{paramName}' cannot be less than TimeSpan.Zero when '{paramName}' is not infinite.");
        }

        return timeout;
    }
}
