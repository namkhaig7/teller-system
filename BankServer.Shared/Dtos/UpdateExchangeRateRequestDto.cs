namespace BankServer.Shared.Dtos;

// What the teller app sends to change a currency's buy/sell rate.
public record UpdateExchangeRateRequestDto(string CurrencyCode, decimal BuyRate, decimal SellRate);
