namespace BankServer.Shared;

// One place for "where is BankServer.API" so every client project agrees on it. Override
// with the BANKSERVER_API_URL environment variable if the server isn't on localhost:5100
// (e.g. a teammate's machine, or the API running inside Docker while this client runs
// natively -- see CLAUDE.md's "Running everything on one machine" section).
public static class ApiConfig
{
    public static string BaseUrl =>
        Environment.GetEnvironmentVariable("BANKSERVER_API_URL") ?? "http://localhost:5100";
}
