using System.Net;
using System.Net.Http.Json;
using BankServer.Shared.Dtos;

namespace BankServer.Shared;

// Thin wrapper over HttpClient for talking to BankServer.API's WebAPI. Shared by every
// client project (kiosk, teller, currency board) so the request/response shapes only
// need to be written once. Each caller supplies its own HttpClient with BaseAddress set
// to wherever BankServer.API is running.
public class BankApiClient(HttpClient httpClient)
{
    public async Task<TicketDto> IssueTicketAsync(CancellationToken ct = default)
    {
        var response = await httpClient.PostAsync("/api/tickets", content: null, ct);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TicketDto>(ct))!;
    }

    public async Task<TicketDto?> GetNextWaitingTicketAsync(CancellationToken ct = default)
    {
        var response = await httpClient.GetAsync("/api/tickets/next-waiting", ct);
        if (response.StatusCode == HttpStatusCode.NoContent) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TicketDto>(ct);
    }

    public async Task<CalledCustomerDto?> CallNextAsync(int counterNumber, CancellationToken ct = default)
    {
        var response = await httpClient.PostAsJsonAsync("/api/queue/call-next", new CallNextRequestDto(counterNumber), ct);
        if (response.StatusCode == HttpStatusCode.NoContent) return null; // nobody waiting
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CalledCustomerDto>(ct);
    }

    public async Task<AccountDto?> GetAccountAsync(string accountNumber, CancellationToken ct = default)
    {
        var response = await httpClient.GetAsync($"/api/accounts/{accountNumber}", ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AccountDto>(ct);
    }

    public async Task<TransferResultDto> TransferAsync(TransferRequestDto request, CancellationToken ct = default)
    {
        var response = await httpClient.PostAsJsonAsync("/api/transfers", request, ct);
        // 200 on success, 400 on a rejected transfer (insufficient funds, etc.) -- both
        // bodies deserialize to the same TransferResultDto, so no need to branch on status.
        return (await response.Content.ReadFromJsonAsync<TransferResultDto>(ct))!;
    }

    public async Task<List<ExchangeRateDto>> GetRatesAsync(CancellationToken ct = default)
    {
        var response = await httpClient.GetAsync("/api/rates", ct);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<List<ExchangeRateDto>>(ct))!;
    }

    public async Task<ExchangeRateDto> UpdateRateAsync(UpdateExchangeRateRequestDto request, CancellationToken ct = default)
    {
        var response = await httpClient.PutAsJsonAsync("/api/rates", request, ct);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ExchangeRateDto>(ct))!;
    }
}
