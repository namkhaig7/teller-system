using BankServer.Shared;
using BankServer.Shared.Dtos;
using BankServer.Shared.Hubs;
using Microsoft.AspNetCore.SignalR.Client;

namespace NumberDispenser.WinForms;

public partial class KioskForm : Form
{
    private readonly BankApiClient _api;
    private readonly HubConnection _queueHub;

    public KioskForm()
    {
        InitializeComponent();

        var httpClient = new HttpClient { BaseAddress = new Uri(ApiConfig.BaseUrl) };
        _api = new BankApiClient(httpClient);

        _queueHub = new HubConnectionBuilder()
            .WithUrl(ApiConfig.BaseUrl + HubRoutes.QueueDisplay)
            .WithAutomaticReconnect()
            .Build();

        // Pushed by the server every time a teller calls the next customer -- keeps
        // "now serving" live without the kiosk having to poll for it.
        _queueHub.On<CalledCustomerDto>(nameof(IQueueDisplayClient.CustomerCalled), called =>
        {
            Invoke(() => lblNowServing.Text = $"Одоо үйлчилж буй: №{called.TicketNumber} — {called.CounterNumber}-р цонх");
        });

        Load += async (_, _) => await ConnectToQueueHubAsync();
        FormClosed += async (_, _) => await _queueHub.DisposeAsync();
        btnTakeNumber.Click += async (_, _) => await TakeNumberAsync();
    }

    private async Task ConnectToQueueHubAsync()
    {
        try
        {
            await _queueHub.StartAsync();
            lblStatus.Text = "Сервертэй холбогдсон.";
        }
        catch (Exception ex)
        {
            lblStatus.Text = $"Сервертэй холбогдож чадсангүй: {ex.Message}";
        }
    }

    private async Task TakeNumberAsync()
    {
        btnTakeNumber.Enabled = false;
        try
        {
            var ticket = await _api.IssueTicketAsync();
            lblYourNumber.Text = ticket.Number.ToString();
            lblStatus.Text = $"Дугаар №{ticket.Number} хэвлэгдлээ.";
        }
        catch (Exception ex)
        {
            lblStatus.Text = $"Алдаа гарлаа: {ex.Message}";
        }
        finally
        {
            btnTakeNumber.Enabled = true;
        }
    }
}
