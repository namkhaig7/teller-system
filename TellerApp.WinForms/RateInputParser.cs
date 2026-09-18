using System.Globalization;

namespace TellerApp.WinForms;

// Parses the currency code / buy rate / sell rate fields on the Rates tab. Pulled out of
// TellerForm for the same reason as AmountParser: testable without a Windows Form.
public static class RateInputParser
{
    public static bool TryParse(
        string currencyCodeText,
        string buyRateText,
        string sellRateText,
        out string currencyCode,
        out decimal buyRate,
        out decimal sellRate)
    {
        currencyCode = currencyCodeText.Trim().ToUpperInvariant();
        var buyOk = decimal.TryParse(buyRateText.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out buyRate);
        var sellOk = decimal.TryParse(sellRateText.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out sellRate);

        return currencyCode.Length > 0 && buyOk && sellOk;
    }
}
