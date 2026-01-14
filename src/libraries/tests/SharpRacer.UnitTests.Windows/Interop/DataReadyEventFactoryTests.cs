using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpRacer.Interop;

public class DataReadyEventFactoryTests
{
    [Fact]
    public void CreateAutoResetEvent_Test()
    {
        var factory = new DataReadyEventFactory();

        var dataReadyEvent = factory.CreateAutoResetEvent(false);

        Assert.NotNull(dataReadyEvent);
        Assert.False(dataReadyEvent.SafeWaitHandle.IsClosed);
        Assert.False(dataReadyEvent.SafeWaitHandle.IsInvalid);
    }

    [Fact]
    public void CreateSafeWaitHandle_Test()
    {
        var handle = DataReadyEventFactory.CreateSafeWaitHandle();

        Assert.False(handle.IsClosed);
        Assert.False(handle.IsInvalid);
    }
}
