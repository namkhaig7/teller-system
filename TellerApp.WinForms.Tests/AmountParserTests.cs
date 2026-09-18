using System.Globalization;

namespace TellerApp.WinForms.Tests;

public class AmountParserTests
{
    [Theory]
    [InlineData("1000", 1000)]
    [InlineData(" 1000 ", 1000)] // whitespace from an accidental extra space/paste
    [InlineData("1000.50", 1000.50)]
    [InlineData("1,000.50", 1000.50)] // thousands separator is fine, it's still unambiguous
    public void TryParse_AcceptsValidAmounts(string text, decimal expected)
    {
        var ok = AmountParser.TryParse(text, out var amount);

        Assert.True(ok);
        Assert.Equal(expected, amount);
    }

    [Theory]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("12.34.56")]
    public void TryParse_RejectsInvalidAmounts(string text)
    {
        var ok = AmountParser.TryParse(text, out _);

        Assert.False(ok);
    }

    [Fact]
    public void TryParse_IgnoresTheMachinesCurrentCulture_AndAlwaysUsesADotAsTheDecimalPoint()
    {
        // Some Windows regional settings (e.g. de-DE, and some mn-MN configurations) use a
        // comma as the decimal point and a dot as the thousands separator -- the opposite
        // of invariant culture. If TryParse ever accidentally used CultureInfo.CurrentCulture
        // instead of InvariantCulture, "1234.50" would misparse under one of those cultures
        // (interpreted as "123,450" grouped, or rejected). Temporarily switching the running
        // thread to such a culture and confirming the result is unchanged is what actually
        // proves AmountParser hard-codes invariant culture rather than just happening to
        // behave correctly on this one machine's default settings.
        var original = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");

            var ok = AmountParser.TryParse("1234.50", out var amount);

            Assert.True(ok);
            Assert.Equal(1234.50m, amount);
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }
    }
}
