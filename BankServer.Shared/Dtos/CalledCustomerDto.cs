namespace BankServer.Shared.Dtos;

// Pushed to the number-display screens (and shown on the teller's own screen) whenever
// a teller calls the next customer: "ticket number N, please go to counter C".
public record CalledCustomerDto(int TicketNumber, int CounterNumber, DateTime CalledAtUtc);
