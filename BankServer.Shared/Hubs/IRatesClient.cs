using BankServer.Shared.Dtos;

namespace BankServer.Shared.Hubs;

// Methods the server calls ON the currency board — pushed the instant a teller changes a rate.
public interface IRatesClient
{
    Task RateChanged(ExchangeRateDto rate);
}
