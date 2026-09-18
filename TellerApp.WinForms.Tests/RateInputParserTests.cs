namespace TellerApp.WinForms.Tests;

public class RateInputParserTests
{
    [Fact]
    public void TryParse_SucceedsAndUppercasesTheCurrencyCode()
    {
        var ok = RateInputParser.TryParse("usd", "3500", "3530", out var code, out var buy, out var sell);

        Assert.True(ok);
        Assert.Equal("USD", code);
        Assert.Equal(3500m, buy);
        Assert.Equal(3530m, sell);
    }

    [Fact]
    public void TryParse_TrimsWhitespaceFromTheCurrencyCode()
    {
        var ok = RateInputParser.TryParse("  eur  ", "3700", "3740", out var code, out _, out _);

        Assert.True(ok);
        Assert.Equal("EUR", code);
    }

    [Fact]
    public void TryParse_Fails_WhenCurrencyCodeIsEmpty()
    {
        var ok = RateInputParser.TryParse("   ", "3500", "3530", out _, out _, out _);

        Assert.False(ok);
    }

    [Theory]
    [InlineData("abc", "3530")]
    [InlineData("3500", "xyz")]
    public void TryParse_Fails_WhenEitherRateIsNotANumber(string buyText, string sellText)
    {
        var ok = RateInputParser.TryParse("USD", buyText, sellText, out _, out _, out _);

        Assert.False(ok);
    }
}
