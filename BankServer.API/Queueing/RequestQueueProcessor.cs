namespace BankServer.API.Queueing;

// The single consumer: for the whole lifetime of the app, this is the only thing that
// reads work items off SerialRequestQueue, one at a time, in the order they arrived.
// That's what makes "call next" and "transfer" safe under concurrent requests.
public class RequestQueueProcessor(SerialRequestQueue queue, ILogger<RequestQueueProcessor> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var work in queue.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                await work();
            }
            catch (Exception ex)
            {
                // work() already routes its own exceptions back to the caller via the
                // TaskCompletionSource in SerialRequestQueue -- this is just a safety net
                // so one bad item can't kill the loop for everyone after it.
                logger.LogError(ex, "Unhandled error processing a queued request");
            }
        }
    }
}
