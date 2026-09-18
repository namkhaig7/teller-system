using BankServer.Shared.Dtos;

namespace CurrencyBoard.Blazor.Tests;

public class RateBoardStateTests
{
    [Fact]
    public void ReplaceAll_PopulatesTheBoard()
    {
        var board = new RateBoardState();

        board.ReplaceAll([
            new ExchangeRateDto("USD", 3500m, 3530m, DateTime.UtcNow),
            new ExchangeRateDto("EUR", 3700m, 3740m, DateTime.UtcNow),
        ]);

        Assert.Equal(2, board.Count);
    }

    [Fact]
    public void OrderedByCurrency_SortsAlphabetically()
    {
        var board = new RateBoardState();
        board.ReplaceAll([
            new ExchangeRateDto("USD", 3500m, 3530m, DateTime.UtcNow),
            new ExchangeRateDto("EUR", 3700m, 3740m, DateTime.UtcNow),
            new ExchangeRateDto("JPY", 24m, 25m, DateTime.UtcNow),
        ]);

        var ordered = board.OrderedByCurrency();

        Assert.Equal(["EUR", "JPY", "USD"], ordered.Select(r => r.CurrencyCode));
    }

    [Fact]
    public void Upsert_AddsANewCurrency_WithoutAffectingExistingOnes()
    {
        var board = new RateBoardState();
        board.ReplaceAll([new ExchangeRateDto("USD", 3500m, 3530m, DateTime.UtcNow)]);

        board.Upsert(new ExchangeRateDto("EUR", 3700m, 3740m, DateTime.UtcNow));

        Assert.Equal(2, board.Count);
    }

    [Fact]
    public void Upsert_ReplacesAnExistingCurrencysRate_RatherThanAddingADuplicateRow()
    {
        // This is the realtime-push path: the currency board should update USD's row in
        // place when RatesHub pushes a new rate for a currency it already has, not grow a
        // second row for the same currency.
        var board = new RateBoardState();
        board.ReplaceAll([new ExchangeRateDto("USD", 3500m, 3530m, DateTime.UtcNow)]);

        board.Upsert(new ExchangeRateDto("USD", 3600m, 3630m, DateTime.UtcNow));

        Assert.Equal(1, board.Count);
        Assert.Equal(3600m, board.OrderedByCurrency().Single().BuyRate);
    }
}
