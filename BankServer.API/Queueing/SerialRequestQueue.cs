using System.Threading.Channels;

namespace BankServer.API.Queueing;

// Every "call next" and "transfer" request is wrapped as a work item and dropped in here
// instead of being run directly by the endpoint that received it. RequestQueueProcessor
// pulls items out one at a time, so two requests that arrive at the same instant can never
// both read the same "before" state and step on each other.
public class SerialRequestQueue
{
    private readonly Channel<Func<Task>> _channel = Channel.CreateUnbounded<Func<Task>>();

    public ChannelReader<Func<Task>> Reader => _channel.Reader;

    public async Task<T> EnqueueAsync<T>(Func<Task<T>> work)
    {
        var completion = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);

        await _channel.Writer.WriteAsync(async () =>
        {
            try
            {
                completion.SetResult(await work());
            }
            catch (Exception ex)
            {
                completion.SetException(ex);
            }
        });

        return await completion.Task;
    }
}
