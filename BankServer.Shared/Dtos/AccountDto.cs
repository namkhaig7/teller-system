namespace BankServer.Shared.Dtos;

public record AccountDto(string AccountNumber, string OwnerName, decimal Balance, string CurrencyCode);
