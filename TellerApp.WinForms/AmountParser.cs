using System.Globalization;

namespace TellerApp.WinForms;

// Parses the free-text amount field on the Transfer tab. Pulled out of TellerForm so the
// parsing rule (invariant culture, so a "3,500.25"-style entry isn't misread depending on
// the machine's regional settings) can be unit tested without spinning up a Windows Form.
public static class AmountParser
{
    public static bool TryParse(string text, out decimal amount) =>
        decimal.TryParse(text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out amount);
}
