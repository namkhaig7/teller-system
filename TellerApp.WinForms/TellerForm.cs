using BankServer.Shared;
using BankServer.Shared.Dtos;
using BankServer.Shared.Hubs;
using Microsoft.AspNetCore.SignalR.Client;

namespace TellerApp.WinForms;

public partial class TellerForm : Form
{
    private readonly BankApiClient _api;
    private readonly HubConnection _ratesHub;

    public TellerForm()
    {
        InitializeComponent();

        var httpClient = new HttpClient { BaseAddress = new Uri(ApiConfig.BaseUrl) };
        _api = new BankApiClient(httpClient);

        _ratesHub = new HubConnectionBuilder()
            .WithUrl(ApiConfig.BaseUrl + HubRoutes.Rates)
            .WithAutomaticReconnect()
            .Build();

        // Pushed whenever ANY teller changes a rate, so every open TellerApp window stays
        // in sync without anyone having to hit "refresh".
        _ratesHub.On<ExchangeRateDto>(nameof(IRatesClient.RateChanged), rate =>
        {
            Invoke(() => UpsertRateRow(rate));
        });

        Load += async (_, _) => await OnLoadAsync();
        FormClosed += async (_, _) => await _ratesHub.DisposeAsync();

        btnRefreshNextWaiting.Click += async (_, _) => await RefreshNextWaitingAsync();
        btnCallNext.Click += async (_, _) => await CallNextAsync();

        txtFromAccount.Leave += async (_, _) => await LookupAccountAsync(txtFromAccount, lblFromOwner);
        txtToAccount.Leave += async (_, _) => await LookupAccountAsync(txtToAccount, lblToOwner);
        btnTransfer.Click += async (_, _) => await TransferAsync();

        btnRefreshRates.Click += async (_, _) => await RefreshRatesAsync();
        btnUpdateRate.Click += async (_, _) => await UpdateRateAsync();
    }

    private async Task OnLoadAsync()
    {
        try
        {
            await _ratesHub.StartAsync();
            lblRateStatus.Text = "Сервертэй холбогдсон.";
        }
        catch (Exception ex)
        {
            lblRateStatus.Text = $"Сервертэй холбогдож чадсангүй: {ex.Message}";
        }

        await RefreshNextWaitingAsync();
        await RefreshRatesAsync();
    }

    // --- Call next customer ---

    private async Task RefreshNextWaitingAsync()
    {
        try
        {
            var next = await _api.GetNextWaitingTicketAsync();
            lblNextWaiting.Text = next is null
                ? "Хүлээгдэж буй дараагийн дугаар: байхгүй"
                : $"Хүлээгдэж буй дараагийн дугаар: №{next.Number}";
        }
        catch (Exception ex)
        {
            lblCallStatus.Text = $"Алдаа гарлаа: {ex.Message}";
        }
    }

    private async Task CallNextAsync()
    {
        btnCallNext.Enabled = false;
        try
        {
            var counterNumber = (int)numCounterNumber.Value;
            var called = await _api.CallNextAsync(counterNumber);

            lblCallResult.Text = called is null
                ? "Хүлээгдэж буй үйлчлүүлэгч алга байна."
                : $"Дуудсан дугаар: №{called.TicketNumber}  —  {called.CounterNumber}-р цонх";
            lblCallStatus.Text = string.Empty;
        }
        catch (Exception ex)
        {
            lblCallStatus.Text = $"Алдаа гарлаа: {ex.Message}";
        }
        finally
        {
            btnCallNext.Enabled = true;
            await RefreshNextWaitingAsync();
        }
    }

    // --- Transfer ---

    private async Task LookupAccountAsync(TextBox accountBox, Label ownerLabel)
    {
        var accountNumber = accountBox.Text.Trim();
        if (accountNumber.Length == 0)
        {
            ownerLabel.Text = string.Empty;
            return;
        }

        var account = await _api.GetAccountAsync(accountNumber);
        ownerLabel.Text = account is null
            ? "Олдсонгүй"
            : $"{account.OwnerName} ({account.Balance:N2} {account.CurrencyCode})";
    }

    private async Task TransferAsync()
    {
        var from = txtFromAccount.Text.Trim();
        var to = txtToAccount.Text.Trim();

        if (!AmountParser.TryParse(txtAmount.Text, out var amount))
        {
            lblTransferResult.ForeColor = BankColors.Error;
            lblTransferResult.Text = "Дүн буруу байна.";
            return;
        }

        btnTransfer.Enabled = false;
        try
        {
            var result = await _api.TransferAsync(new TransferRequestDto(from, to, amount));

            if (result.Success)
            {
                lblTransferResult.ForeColor = BankColors.Success;
                lblTransferResult.Text = $"Шилжүүлэг амжилттай.\n{from}: {result.FromNewBalance:N2}   {to}: {result.ToNewBalance:N2}";
                await LookupAccountAsync(txtFromAccount, lblFromOwner);
                await LookupAccountAsync(txtToAccount, lblToOwner);
            }
            else
            {
                lblTransferResult.ForeColor = BankColors.Error;
                lblTransferResult.Text = result.ErrorMessage;
            }
        }
        catch (Exception ex)
        {
            lblTransferResult.ForeColor = BankColors.Error;
            lblTransferResult.Text = $"Алдаа гарлаа: {ex.Message}";
        }
        finally
        {
            btnTransfer.Enabled = true;
        }
    }

    // --- Exchange rates ---

    private async Task RefreshRatesAsync()
    {
        try
        {
            var rates = await _api.GetRatesAsync();
            lvRates.Items.Clear();
            foreach (var rate in rates)
            {
                lvRates.Items.Add(BuildRateRow(rate));
            }
            lblRateStatus.Text = string.Empty;
        }
        catch (Exception ex)
        {
            lblRateStatus.Text = $"Алдаа гарлаа: {ex.Message}";
        }
    }

    private async Task UpdateRateAsync()
    {
        var parsed = RateInputParser.TryParse(
            txtCurrencyCode.Text, txtBuyRate.Text, txtSellRate.Text,
            out var currencyCode, out var buyRate, out var sellRate);

        if (!parsed)
        {
            lblRateStatus.Text = "Валют, авах ханш, зарах ханшийг зөв бөглөнө үү.";
            return;
        }

        btnUpdateRate.Enabled = false;
        try
        {
            var updated = await _api.UpdateRateAsync(new UpdateExchangeRateRequestDto(currencyCode, buyRate, sellRate));
            UpsertRateRow(updated); // the hub push will also arrive and update this row again -- harmless, just a no-op overwrite
            lblRateStatus.Text = $"{currencyCode} ханш шинэчлэгдлээ.";
        }
        catch (Exception ex)
        {
            lblRateStatus.Text = $"Алдаа гарлаа: {ex.Message}";
        }
        finally
        {
            btnUpdateRate.Enabled = true;
        }
    }

    private void UpsertRateRow(ExchangeRateDto rate)
    {
        var existing = lvRates.Items[rate.CurrencyCode];
        if (existing is null)
        {
            lvRates.Items.Add(BuildRateRow(rate));
        }
        else
        {
            existing.SubItems[1].Text = rate.BuyRate.ToString("N2");
            existing.SubItems[2].Text = rate.SellRate.ToString("N2");
            existing.SubItems[3].Text = rate.UpdatedAtUtc.ToLocalTime().ToString("g");
        }
    }

    private static ListViewItem BuildRateRow(ExchangeRateDto rate)
    {
        var item = new ListViewItem(rate.CurrencyCode) { Name = rate.CurrencyCode };
        item.SubItems.Add(rate.BuyRate.ToString("N2"));
        item.SubItems.Add(rate.SellRate.ToString("N2"));
        item.SubItems.Add(rate.UpdatedAtUtc.ToLocalTime().ToString("g"));
        return item;
    }
}
