using Testcontainers.PostgreSql;

namespace BankServer.API.Tests.Data;

// Spins up a real, throwaway Postgres in Docker for the Repo tests to run against --
// same schema/seed data as production (BankServer.API/Data/init.sql), just a fresh
// instance per test run. This matters because the Repo classes are raw SQL, not an ORM:
// the only way to genuinely test a hand-written SQL query is to run it against a real
// database, not a mock. Shared by every test in the "Postgres" collection (see below) so
// the ~2 second container startup cost is only paid once, not once per test class.
public class PostgresFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:17-alpine")
        .WithDatabase("bankserver")
        .WithUsername("bankserver")
        .WithPassword("bankserver_test_pw")
        .WithBindMount(FindInitSqlPath(), "/docker-entrypoint-initdb.d/init.sql")
        .Build();

    public string ConnectionString => _container.GetConnectionString();

    public Task InitializeAsync() => _container.StartAsync();

    public Task DisposeAsync() => _container.DisposeAsync().AsTask();

    private static string FindInitSqlPath()
    {
        // Walk up from the test assembly's output folder to the repo root, then down
        // into BankServer.API/Data -- keeps this pointed at the one real init.sql
        // (the same file docker-compose.yml mounts for the actual app) instead of a
        // second copy that could quietly drift out of sync with it.
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "BankServer.slnx")))
        {
            dir = dir.Parent;
        }

        if (dir is null)
        {
            throw new InvalidOperationException("Could not find repo root (BankServer.slnx) above " + AppContext.BaseDirectory);
        }

        return Path.Combine(dir.FullName, "BankServer.API", "Data", "init.sql");
    }
}

// xUnit collection fixtures: every test class tagged [Collection("Postgres")] shares one
// PostgresFixture instance AND xUnit runs their tests sequentially against it (never in
// parallel), which is exactly what we want -- one shared database, no cross-test races.
[CollectionDefinition("Postgres")]
public class PostgresCollection : ICollectionFixture<PostgresFixture>;
