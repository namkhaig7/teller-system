using System.Net;
using System.Net.Http.Json;
using BankServer.Shared.Dtos;

namespace BankServer.Shared.Tests;

public class BankApiClientTests
{
    [Fact]
    public async Task IssueTicketAsync_PostsToTicketsEndpoint_AndReturnsDeserializedTicket()
    {
        var ticket = new TicketDto(7, DateTime.UtcNow, TicketStatus.Waiting);
        HttpRequestMessage? captured = null;

        var client = CreateClient(req =>
        {
            captured = req;
            return JsonResponse(HttpStatusCode.OK, ticket);
        });

        var result = await client.IssueTicketAsync();

        Assert.Equal(HttpMethod.Post, captured!.Method);
        Assert.Equal("/api/tickets", captured.RequestUri!.AbsolutePath);
        Assert.Equal(7, result.Number);
    }

    [Fact]
    public async Task CallNextAsync_ReturnsNull_WhenServerRespondsNoContent()
    {
        // The server returns 204 No Content specifically to mean "nobody is waiting" --
        // this has to map to null, not throw, so the caller can show "no one waiting".
        var client = CreateClient(_ => new HttpResponseMessage(HttpStatusCode.NoContent));

        var result = await client.CallNextAsync(counterNumber: 3);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAccountAsync_ReturnsNull_WhenServerRespondsNotFound()
    {
        var client = CreateClient(_ => new HttpResponseMessage(HttpStatusCode.NotFound));

        var result = await client.GetAccountAsync("no-such-account");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAccountAsync_RequestsTheGivenAccountNumberInTheUrl()
    {
        HttpRequestMessage? captured = null;
        var account = new AccountDto("1000000001", "Bat", 500000m, "MNT");

        var client = CreateClient(req =>
        {
            captured = req;
            return JsonResponse(HttpStatusCode.OK, account);
        });

        await client.GetAccountAsync("1000000001");

        Assert.Equal("/api/accounts/1000000001", captured!.RequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task TransferAsync_StillDeserializesTheResult_WhenServerRespondsBadRequest()
    {
        // A rejected transfer (e.g. insufficient funds) comes back as 400, but the body is
        // still a normal TransferResultDto with Success = false -- the caller shouldn't
        // need to special-case the status code, just check the DTO.
        var failure = new TransferResultDto(false, "Insufficient funds.", 100m, 50m);
        var client = CreateClient(_ => JsonResponse(HttpStatusCode.BadRequest, failure));

        var result = await client.TransferAsync(new TransferRequestDto("A", "B", 999m));

        Assert.False(result.Success);
        Assert.Equal("Insufficient funds.", result.ErrorMessage);
    }

    [Fact]
    public async Task GetRatesAsync_SendsAGetToTheRatesEndpoint()
    {
        HttpRequestMessage? captured = null;
        var client = CreateClient(req =>
        {
            captured = req;
            return JsonResponse(HttpStatusCode.OK, new List<ExchangeRateDto>());
        });

        await client.GetRatesAsync();

        Assert.Equal(HttpMethod.Get, captured!.Method);
        Assert.Equal("/api/rates", captured.RequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task UpdateRateAsync_SendsAPutWithTheRequestBody()
    {
        HttpRequestMessage? captured = null;
        var updated = new ExchangeRateDto("USD", 3500m, 3530m, DateTime.UtcNow);

        var client = CreateClient(req =>
        {
            captured = req;
            return JsonResponse(HttpStatusCode.OK, updated);
        });

        var result = await client.UpdateRateAsync(new UpdateExchangeRateRequestDto("USD", 3500m, 3530m));

        Assert.Equal(HttpMethod.Put, captured!.Method);
        Assert.Equal("/api/rates", captured.RequestUri!.AbsolutePath);
        Assert.Equal(3500m, result.BuyRate);
    }

    private static BankApiClient CreateClient(Func<HttpRequestMessage, HttpResponseMessage> respond)
    {
        var httpClient = new HttpClient(new FakeHttpMessageHandler(respond))
        {
            BaseAddress = new Uri("http://localhost")
        };
        return new BankApiClient(httpClient);
    }

    private static HttpResponseMessage JsonResponse<T>(HttpStatusCode status, T body) => new(status)
    {
        Content = JsonContent.Create(body)
    };
}
