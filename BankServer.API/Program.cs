using BankServer.API.Data;
using BankServer.API.Hubs;
using BankServer.API.Queueing;
using BankServer.Shared.Dtos;
using BankServer.Shared.Hubs;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Postgres")
    ?? throw new InvalidOperationException("Missing ConnectionStrings:Postgres configuration.");

builder.Services.AddOpenApi();
builder.Services.AddSignalR();
builder.Services.AddSingleton<SerialRequestQueue>();
builder.Services.AddHostedService<RequestQueueProcessor>();
builder.Services.AddSingleton(new AccountRepo(connectionString));
builder.Services.AddSingleton(new TicketRepo(connectionString));
builder.Services.AddSingleton(new ExchangeRateRepo(connectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// --- Number dispenser kiosk ---
app.MapPost("/api/tickets", async (TicketRepo tickets) =>
    Results.Ok(await tickets.IssueNextTicketAsync()));

app.MapGet("/api/tickets/next-waiting", async (TicketRepo tickets) =>
{
    var next = await tickets.GetNextWaitingAsync();
    return next is null ? Results.NoContent() : Results.Ok(next);
});

// --- Teller: call next customer ---
app.MapPost("/api/queue/call-next", async (
    CallNextRequestDto request,
    TicketRepo tickets,
    SerialRequestQueue queue,
    IHubContext<QueueHub, IQueueDisplayClient> hub) =>
{
    var called = await queue.EnqueueAsync(() => tickets.CallNextAsync(request.CounterNumber));
    if (called is null) return Results.NoContent(); // nobody waiting

    await hub.Clients.All.CustomerCalled(called);
    return Results.Ok(called);
});

// --- Teller: execute a transfer ---
app.MapGet("/api/accounts/{accountNumber}", async (string accountNumber, AccountRepo accounts) =>
{
    var account = await accounts.GetByAccountNumberAsync(accountNumber);
    return account is null ? Results.NotFound() : Results.Ok(account);
});

app.MapPost("/api/transfers", async (TransferRequestDto request, AccountRepo accounts, SerialRequestQueue queue) =>
{
    var result = await queue.EnqueueAsync(() =>
        accounts.TransferAsync(request.FromAccountNumber, request.ToAccountNumber, request.Amount));
    return result.Success ? Results.Ok(result) : Results.BadRequest(result);
});

// --- Teller: change exchange rates ---
app.MapGet("/api/rates", async (ExchangeRateRepo rates) => await rates.GetAllAsync());

app.MapPut("/api/rates", async (
    UpdateExchangeRateRequestDto request,
    ExchangeRateRepo rates,
    IHubContext<RatesHub, IRatesClient> hub) =>
{
    var updated = await rates.UpdateRateAsync(request.CurrencyCode, request.BuyRate, request.SellRate);
    await hub.Clients.All.RateChanged(updated);
    return Results.Ok(updated);
});

app.MapHub<QueueHub>(HubRoutes.QueueDisplay);
app.MapHub<RatesHub>(HubRoutes.Rates);

app.Run();
