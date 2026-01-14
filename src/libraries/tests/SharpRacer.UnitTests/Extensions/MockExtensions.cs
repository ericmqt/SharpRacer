using Moq;

namespace SharpRacer;
internal static class MockExtensions
{
    internal static Mock<IDisposable> RequireDisposed(this Mock<IDisposable> mock)
    {
        return mock.RequireDisposed(Times.AtLeastOnce(), $"{nameof(RequireDisposed)}: Dispose not invoked");
    }

    internal static Mock<IDisposable> RequireDisposed(this Mock<IDisposable> mock, Times times)
    {
        return mock.RequireDisposed(times, $"{nameof(RequireDisposed)}: Dispose not invoked");
    }

    internal static Mock<IDisposable> RequireDisposed(this Mock<IDisposable> mock, Times times, string failMessage)
    {
        mock.Setup(x => x.Dispose()).Verifiable(times, failMessage);

        return mock;
    }

    internal static Mock<IDisposable> RequireNotDisposed(this Mock<IDisposable> mock)
    {
        return mock.RequireNotDisposed($"{nameof(RequireNotDisposed)}: Dispose invoked on IDisposable mock");
    }

    internal static Mock<IDisposable> RequireNotDisposed(this Mock<IDisposable> mock, string failMessage)
    {
        mock.Setup(x => x.Dispose()).Verifiable(Times.Never(), failMessage);

        return mock;
    }
}
