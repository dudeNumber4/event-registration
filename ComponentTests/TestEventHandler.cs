using Microsoft.AspNetCore.Components;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ComponentTests
{
    /// <summary>
    /// Currently hard coded for int; could be generic
    /// </summary>
    public class TestEventHandler : IHandleEvent
    {
        public int ValueReceived { get; set; }
        public Task HandleEventAsync(EventCallbackWorkItem item, object arg)
        {
            if (int.TryParse(arg.ToString(), out var n))
            {
                ValueReceived = n;
            }
            else
            {
                Debug.Fail($"Received unexpected value in {nameof(TestEventHandler)}");
            }
            return Task.CompletedTask;
        }
    }
}
