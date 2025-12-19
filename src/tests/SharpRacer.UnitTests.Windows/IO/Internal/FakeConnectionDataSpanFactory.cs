using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpRacer.IO.Internal;

internal sealed class FakeConnectionDataSpanFactory : IConnectionDataSpanFactory
{
    private readonly byte[] _spanValue;

    public FakeConnectionDataSpanFactory(byte[] spanValue)
    {
        _spanValue = spanValue;
    }

    public bool IsDisposed { get; private set; }

    public ReadOnlySpan<byte> Create()
    {
        return _spanValue;
    }

    public void Dispose()
    {
        IsDisposed = true;
    }
}
