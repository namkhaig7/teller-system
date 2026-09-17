namespace BankServer.Shared.Dtos;

public record ExchangeRateDto(string CurrencyCode, decimal BuyRate, decimal SellRate, DateTime UpdatedAtUtc);
