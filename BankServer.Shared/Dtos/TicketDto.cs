namespace BankServer.Shared.Dtos;

// One queue number issued by the NumberDispenser kiosk.
public record TicketDto(int Number, DateTime IssuedAtUtc, TicketStatus Status);
