using BankServer.Shared.Dtos;
using Npgsql;

namespace BankServer.API.Data;

public class ExchangeRateRepo(string connectionString)
{
    public async Task<List<ExchangeRateDto>> GetAllAsync()
    {
        var rates = new List<ExchangeRateDto>();

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            "SELECT currency_code, buy_rate, sell_rate, updated_at_utc FROM exchange_rates ORDER BY currency_code",
            connection);

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            rates.Add(Map(reader));
        }

        return rates;
    }

    // "Upsert": insert the currency if it's new, update it if it already exists. This is a
    // single atomic statement, so unlike transfers it doesn't need SerialRequestQueue --
    // there's no separate read-then-write step for two concurrent requests to race on.
    public async Task<ExchangeRateDto> UpdateRateAsync(string currencyCode, decimal buyRate, decimal sellRate)
    {
        var updatedAt = DateTime.UtcNow;

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            """
            INSERT INTO exchange_rates (currency_code, buy_rate, sell_rate, updated_at_utc)
            VALUES (@currencyCode, @buyRate, @sellRate, @updatedAt)
            ON CONFLICT (currency_code)
            DO UPDATE SET buy_rate = @buyRate, sell_rate = @sellRate, updated_at_utc = @updatedAt
            """,
            connection);
        command.Parameters.AddWithValue("currencyCode", currencyCode);
        command.Parameters.AddWithValue("buyRate", buyRate);
        command.Parameters.AddWithValue("sellRate", sellRate);
        command.Parameters.AddWithValue("updatedAt", updatedAt);
        await command.ExecuteNonQueryAsync();

        return new ExchangeRateDto(currencyCode, buyRate, sellRate, updatedAt);
    }

    private static ExchangeRateDto Map(NpgsqlDataReader reader) => new(
        reader.GetString(0),
        reader.GetDecimal(1),
        reader.GetDecimal(2),
        reader.GetDateTime(3));
}
