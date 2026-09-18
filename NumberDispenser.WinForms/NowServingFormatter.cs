using BankServer.Shared.Dtos;

namespace NumberDispenser.WinForms;

// Pulled out of KioskForm so the "now serving" text has its own testable unit instead of
// being an inline string interpolation inside a SignalR callback.
public static class NowServingFormatter
{
    public static string Format(CalledCustomerDto called) =>
        $"Одоо үйлчилж буй: №{called.TicketNumber} — {called.CounterNumber}-р цонх";
}
