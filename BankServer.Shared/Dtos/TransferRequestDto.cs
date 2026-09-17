namespace BankServer.Shared.Dtos;

// What the teller app sends to move money from one account to another.
public record TransferRequestDto(string FromAccountNumber, string ToAccountNumber, decimal Amount);
