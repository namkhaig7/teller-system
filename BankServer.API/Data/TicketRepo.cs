using BankServer.Shared.Dtos;
using Npgsql;

namespace BankServer.API.Data;

public class TicketRepo(string connectionString)
{
    // Used by the number-dispenser kiosk. Doesn't need SerialRequestQueue: Postgres's
    // SERIAL column already hands out a unique, increasing number atomically for every
    // INSERT, so there's no read-then-write race to protect against here.
    public async Task<TicketDto> IssueNextTicketAsync()
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            """
            INSERT INTO tickets (issued_at_utc, status)
            VALUES (now(), 'Waiting')
            RETURNING number, issued_at_utc, status
            """,
            connection);

        await using var reader = await command.ExecuteReaderAsync();
        await reader.ReadAsync();
        return Map(reader);
    }

    public async Task<TicketDto?> GetNextWaitingAsync()
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            "SELECT number, issued_at_utc, status FROM tickets WHERE status = 'Waiting' ORDER BY number LIMIT 1",
            connection);

        await using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync() ? Map(reader) : null;
    }

    // Only ever called from inside SerialRequestQueue's single consumer (see Queueing/),
    // which is what guarantees two "call next" requests can't both grab the same ticket.
    public async Task<CalledCustomerDto?> CallNextAsync(int counterNumber)
    {
        var next = await GetNextWaitingAsync();
        if (next is null) return null;

        var calledAt = DateTime.UtcNow;

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            "UPDATE tickets SET status = 'Called', counter_number = @counterNumber, called_at_utc = @calledAt WHERE number = @number",
            connection);
        command.Parameters.AddWithValue("counterNumber", counterNumber);
        command.Parameters.AddWithValue("calledAt", calledAt);
        command.Parameters.AddWithValue("number", next.Number);
        await command.ExecuteNonQueryAsync();

        return new CalledCustomerDto(next.Number, counterNumber, calledAt);
    }

    private static TicketDto Map(NpgsqlDataReader reader) => new(
        reader.GetInt32(0),
        reader.GetDateTime(1),
        Enum.Parse<TicketStatus>(reader.GetString(2)));
}
