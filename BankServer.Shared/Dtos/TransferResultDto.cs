namespace BankServer.Shared.Dtos;

public record TransferResultDto(bool Success, string? ErrorMessage, decimal FromNewBalance, decimal ToNewBalance);
