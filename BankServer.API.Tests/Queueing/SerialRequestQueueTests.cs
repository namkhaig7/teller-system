using BankServer.API.Queueing;
using Microsoft.Extensions.Logging.Abstractions;

namespace BankServer.API.Tests.Queueing;

public class SerialRequestQueueTests
{
    // Simulates many "read balance, pause, write back new balance" requests arriving at
    // (almost) the same time -- the same shape as a real transfer. Without
    // SerialRequestQueue serializing them, several would read the same starting value
    // and some writes would be lost. This is the concurrency guarantee the assignment
    // asks for: "prevent duplicate processing of a transaction".
    [Fact]
    public async Task EnqueueAsync_SerializesReadThenWriteWorkItems_NoUpdatesAreLost()
    {
        var queue = new SerialRequestQueue();
        StartProcessing(queue);

        var sharedBalance = 0;
        const int requestCount = 50;

        var requests = Enumerable.Range(0, requestCount).Select(_ => queue.EnqueueAsync(async () =>
        {
            var current = sharedBalance;
            await Task.Delay(1); // widen the race window a real DB round-trip would also have
            sharedBalance = current + 1;
            return true;
        }));

        await Task.WhenAll(requests);

        Assert.Equal(requestCount, sharedBalance);
    }

    // Proves work items never run concurrently: if two ever overlapped, the second one
    // in would see "busy == true" and fail.
    [Fact]
    public async Task EnqueueAsync_NeverRunsTwoWorkItemsAtTheSameTime()
    {
        var queue = new SerialRequestQueue();
        StartProcessing(queue);

        var busy = false;
        const int requestCount = 20;

        var requests = Enumerable.Range(0, requestCount).Select(_ => queue.EnqueueAsync(async () =>
        {
            Assert.False(busy, "a second work item started before the first one finished");
            busy = true;
            await Task.Delay(1);
            busy = false;
            return true;
        }));

        await Task.WhenAll(requests);
    }

    [Fact]
    public async Task EnqueueAsync_PropagatesExceptionsFromTheWorkItemToTheCaller()
    {
        var queue = new SerialRequestQueue();
        StartProcessing(queue);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            queue.EnqueueAsync<int>(() => throw new InvalidOperationException("boom")));
    }

    private static void StartProcessing(SerialRequestQueue queue)
    {
        var processor = new RequestQueueProcessor(queue, NullLogger<RequestQueueProcessor>.Instance);
        _ = processor.StartAsync(CancellationToken.None);
    }
}
