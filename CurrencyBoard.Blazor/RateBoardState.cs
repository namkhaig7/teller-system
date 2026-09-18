using BankServer.Shared.Dtos;

namespace CurrencyBoard.Blazor;

// Holds the board's current rates and keeps them sorted for display. Pulled out of
// Home.razor so the "load, then insert-or-update on every push" logic can be unit tested
// without rendering a Blazor component.
public class RateBoardState
{
    private readonly Dictionary<string, ExchangeRateDto> _rates = new();

    public int Count => _rates.Count;

    public void Upsert(ExchangeRateDto rate) => _rates[rate.CurrencyCode] = rate;

    public void ReplaceAll(IEnumerable<ExchangeRateDto> rates)
    {
        _rates.Clear();
        foreach (var rate in rates)
        {
            Upsert(rate);
        }
    }

    public IReadOnlyList<ExchangeRateDto> OrderedByCurrency() =>
        _rates.Values.OrderBy(r => r.CurrencyCode).ToList();
}
