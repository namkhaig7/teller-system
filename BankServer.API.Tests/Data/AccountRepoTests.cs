using BankServer.API.Data;
using Npgsql;

namespace BankServer.API.Tests.Data;

[Collection("Postgres")]
public class AccountRepoTests(PostgresFixture postgres)
{
    private readonly AccountRepo _repo = new(postgres.ConnectionString);

    [Fact]
    public async Task GetByAccountNumberAsync_ReturnsSeededAccount()
    {
        var account = await _repo.GetByAccountNumberAsync("1000000001");

        Assert.NotNull(account);
        Assert.Equal("Bat", account.OwnerName);
    }

    [Fact]
    public async Task GetByAccountNumberAsync_ReturnsNull_WhenAccountDoesNotExist()
    {
        var account = await _repo.GetByAccountNumberAsync("no-such-account");

        Assert.Null(account);
    }

    [Fact]
    public async Task TransferAsync_MovesMoneyBetweenAccounts()
    {
        var from = await InsertAccountAsync(balance: 1000m);
        var to = await InsertAccountAsync(balance: 500m);

        var result = await _repo.TransferAsync(from, to, 300m);

        Assert.True(result.Success);
        Assert.Equal(700m, result.FromNewBalance);
        Assert.Equal(800m, result.ToNewBalance);

        // and it's actually persisted, not just reflected in the in-memory result
        var fromAccount = await _repo.GetByAccountNumberAsync(from);
        Assert.Equal(700m, fromAccount!.Balance);
    }

    [Fact]
    public async Task TransferAsync_FailsAndLeavesBalancesUnchanged_WhenFundsAreInsufficient()
    {
        var from = await InsertAccountAsync(balance: 100m);
        var to = await InsertAccountAsync(balance: 500m);

        var result = await _repo.TransferAsync(from, to, 500m);

        Assert.False(result.Success);
        Assert.Equal("Insufficient funds.", result.ErrorMessage);

        var fromAccount = await _repo.GetByAccountNumberAsync(from);
        var toAccount = await _repo.GetByAccountNumberAsync(to);
        Assert.Equal(100m, fromAccount!.Balance);
        Assert.Equal(500m, toAccount!.Balance);
    }

    [Fact]
    public async Task TransferAsync_Fails_WhenFromAccountDoesNotExist()
    {
        var to = await InsertAccountAsync(balance: 500m);

        var result = await _repo.TransferAsync("no-such-account", to, 10m);

        Assert.False(result.Success);
        Assert.Contains("not found", result.ErrorMessage);
    }

    [Fact]
    public async Task TransferAsync_Fails_WhenToAccountDoesNotExist()
    {
        var from = await InsertAccountAsync(balance: 500m);

        var result = await _repo.TransferAsync(from, "no-such-account", 10m);

        Assert.False(result.Success);
        Assert.Contains("not found", result.ErrorMessage);

        // the source account must not have been debited even though it existed
        var fromAccount = await _repo.GetByAccountNumberAsync(from);
        Assert.Equal(500m, fromAccount!.Balance);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-50)]
    public async Task TransferAsync_RejectsNonPositiveAmounts(decimal amount)
    {
        var from = await InsertAccountAsync(balance: 500m);
        var to = await InsertAccountAsync(balance: 500m);

        var result = await _repo.TransferAsync(from, to, amount);

        Assert.False(result.Success);
    }

    private async Task<string> InsertAccountAsync(decimal balance)
    {
        var accountNumber = $"test-{Guid.NewGuid():N}";

        await using var connection = new NpgsqlConnection(postgres.ConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            "INSERT INTO accounts (account_number, owner_name, balance, currency_code) VALUES (@num, 'Test', @bal, 'MNT')",
            connection);
        command.Parameters.AddWithValue("num", accountNumber);
        command.Parameters.AddWithValue("bal", balance);
        await command.ExecuteNonQueryAsync();

        return accountNumber;
    }
}
