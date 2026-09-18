using BankServer.API.Data;

namespace BankServer.API.Tests.Data;

[Collection("Postgres")]
public class ExchangeRateRepoTests(PostgresFixture postgres)
{
    private readonly ExchangeRateRepo _repo = new(postgres.ConnectionString);

    [Fact]
    public async Task GetAllAsync_IncludesSeededCurrencies()
    {
        var rates = await _repo.GetAllAsync();

        Assert.Contains(rates, r => r.CurrencyCode == "USD");
        Assert.Contains(rates, r => r.CurrencyCode == "EUR");
    }

    [Fact]
    public async Task UpdateRateAsync_InsertsNewCurrency_WhenItDoesNotExistYet()
    {
        var updated = await _repo.UpdateRateAsync("JPY", 24.50m, 25.10m);

        Assert.Equal("JPY", updated.CurrencyCode);
        Assert.Equal(24.50m, updated.BuyRate);

        var all = await _repo.GetAllAsync();
        Assert.Contains(all, r => r.CurrencyCode == "JPY");
    }

    [Fact]
    public async Task UpdateRateAsync_OverwritesExistingCurrency_InsteadOfDuplicatingIt()
    {
        await _repo.UpdateRateAsync("GBP", 1m, 2m);

        var updated = await _repo.UpdateRateAsync("GBP", 4700m, 4750m);

        Assert.Equal(4700m, updated.BuyRate);
        Assert.Equal(4750m, updated.SellRate);

        var all = await _repo.GetAllAsync();
        Assert.Single(all, r => r.CurrencyCode == "GBP");
    }
}
