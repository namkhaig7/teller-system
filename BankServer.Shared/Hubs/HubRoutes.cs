namespace BankServer.Shared.Hubs;

// Single source of truth for hub URLs, so the server (app.MapHub<...>(...)) and every
// client (new HubConnectionBuilder().WithUrl(...)) reference the same string.
public static class HubRoutes
{
    public const string QueueDisplay = "/hubs/queue";
    public const string Rates = "/hubs/rates";
}
