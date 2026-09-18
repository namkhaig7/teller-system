namespace BankServer.Shared.Tests;

// A stand-in for the real network, so BankApiClient can be tested without a live
// BankServer.API. Give it a function that inspects the outgoing request and returns
// whatever response we want to pretend the server sent back.
public class FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> respond) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        => Task.FromResult(respond(request));
}
