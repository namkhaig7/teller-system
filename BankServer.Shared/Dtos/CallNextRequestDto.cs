namespace BankServer.Shared.Dtos;

// What the teller app sends to call the next waiting customer to its counter.
public record CallNextRequestDto(int CounterNumber);
