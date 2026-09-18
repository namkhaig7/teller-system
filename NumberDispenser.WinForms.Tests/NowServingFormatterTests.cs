using BankServer.Shared.Dtos;

namespace NumberDispenser.WinForms.Tests;

public class NowServingFormatterTests
{
    [Fact]
    public void Format_IncludesTicketNumberAndCounterNumber()
    {
        var called = new CalledCustomerDto(TicketNumber: 12, CounterNumber: 3, CalledAtUtc: DateTime.UtcNow);

        var text = NowServingFormatter.Format(called);

        Assert.Contains("12", text);
        Assert.Contains("3", text);
    }
}
