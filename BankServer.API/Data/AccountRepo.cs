using BankServer.Shared.Dtos;
using Npgsql;

namespace BankServer.API.Data;

public class AccountRepo(string connectionString)
{
    public async Task<AccountDto?> GetByAccountNumberAsync(string accountNumber)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        return await ReadAccountAsync(connection, null, accountNumber);
    }

    // Moves money from one account to another as a single DB transaction, so the two
    // balance updates either both happen or neither does (e.g. if the app crashes
    // mid-transfer). Protection against two transfers racing each other comes from
    // SerialRequestQueue only ever running one of these at a time -- see Queueing/.
    public async Task<TransferResultDto> TransferAsync(string fromAccountNumber, string toAccountNumber, decimal amount)
    {
        if (amount <= 0)
            return new TransferResultDto(false, "Amount must be greater than zero.", 0, 0);

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        var from = await ReadAccountAsync(connection, transaction, fromAccountNumber);
        if (from is null)
            return new TransferResultDto(false, $"Account {fromAccountNumber} not found.", 0, 0);

        var to = await ReadAccountAsync(connection, transaction, toAccountNumber);
        if (to is null)
            return new TransferResultDto(false, $"Account {toAccountNumber} not found.", from.Balance, 0);

        if (from.Balance < amount)
            return new TransferResultDto(false, "Insufficient funds.", from.Balance, to.Balance);

        var fromNewBalance = from.Balance - amount;
        var toNewBalance = to.Balance + amount;

        await UpdateBalanceAsync(connection, transaction, fromAccountNumber, fromNewBalance);
        await UpdateBalanceAsync(connection, transaction, toAccountNumber, toNewBalance);

        await transaction.CommitAsync();

        return new TransferResultDto(true, null, fromNewBalance, toNewBalance);
    }

    private static async Task<AccountDto?> ReadAccountAsync(NpgsqlConnection connection, NpgsqlTransaction? transaction, string accountNumber)
    {
        await using var command = new NpgsqlCommand(
            "SELECT account_number, owner_name, balance, currency_code FROM accounts WHERE account_number = @accountNumber",
            connection, transaction);
        command.Parameters.AddWithValue("accountNumber", accountNumber);

        await using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync() ? Map(reader) : null;
    }

    private static async Task UpdateBalanceAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, string accountNumber, decimal newBalance)
    {
        await using var command = new NpgsqlCommand(
            "UPDATE accounts SET balance = @balance WHERE account_number = @accountNumber",
            connection, transaction);
        command.Parameters.AddWithValue("balance", newBalance);
        command.Parameters.AddWithValue("accountNumber", accountNumber);
        await command.ExecuteNonQueryAsync();
    }

    private static AccountDto Map(NpgsqlDataReader reader) => new(
        reader.GetString(0),
        reader.GetString(1),
        reader.GetDecimal(2),
        reader.GetString(3));
}
