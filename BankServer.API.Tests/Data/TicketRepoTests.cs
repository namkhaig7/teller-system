using BankServer.API.Data;
using BankServer.Shared.Dtos;

namespace BankServer.API.Tests.Data;

[Collection("Postgres")]
public class TicketRepoTests(PostgresFixture postgres)
{
    private readonly TicketRepo _repo = new(postgres.ConnectionString);

    [Fact]
    public async Task IssueNextTicketAsync_ReturnsIncreasingWaitingNumbers()
    {
        var first = await _repo.IssueNextTicketAsync();
        var second = await _repo.IssueNextTicketAsync();

        Assert.True(second.Number > first.Number);
        Assert.Equal(TicketStatus.Waiting, first.Status);
    }

    [Fact]
    public async Task CallNextAsync_CallsTheOldestWaitingTicketFirst()
    {
        // Drain whatever's currently waiting (from earlier tests / seed data) so this
        // test starts from a known, empty queue.
        while (await _repo.CallNextAsync(counterNumber: 99) is not null) { }

        var ticket = await _repo.IssueNextTicketAsync();

        var called = await _repo.CallNextAsync(counterNumber: 5);

        Assert.NotNull(called);
        Assert.Equal(ticket.Number, called.TicketNumber);
        Assert.Equal(5, called.CounterNumber);
    }

    [Fact]
    public async Task CallNextAsync_ReturnsNull_WhenNoTicketsAreWaiting()
    {
        while (await _repo.CallNextAsync(counterNumber: 99) is not null) { }

        var called = await _repo.CallNextAsync(counterNumber: 1);

        Assert.Null(called);
    }

    [Fact]
    public async Task GetNextWaitingAsync_ReflectsWhatCallNextAsyncWillPickUp()
    {
        while (await _repo.CallNextAsync(counterNumber: 99) is not null) { }

        var issued = await _repo.IssueNextTicketAsync();
        var peeked = await _repo.GetNextWaitingAsync();

        Assert.NotNull(peeked);
        Assert.Equal(issued.Number, peeked.Number);
    }
}
