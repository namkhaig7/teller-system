# BankServer — Teller System (Team Project)

## What this is

A university team assignment (2nd-year CS): a bank teller queueing system, in the style of the
"ProductsAPI/ProductsFront" example the professor demoed in lab. Original assignment text is
Mongolian; translated summary below. **The person you're working with is a 2nd-year CS student
and total beginner with this stack.** See "How to work with me" at the bottom — read it before
writing code.

### Required parts (from the assignment)

1. **Number dispenser terminal** (Дугаар олгох) — WinForms app standing in for the ticket kiosk
   at a bank's entrance. Gets the next queue number from the server, "prints" it (just display/
   simulate), and the server records that the number was issued. Talks to the server over WebAPI.
2. **Teller app** (Теллерийн апп) — WinForms (see decision below). Talks to the server over WebAPI
   (+ SignalR for realtime). Three core actions:
   - **Call next customer** — tells the server to advance the queue; the number-display screens
     must update, and the teller should see what the next number will be.
   - **Execute a customer transaction** — transfer money from account A to account B.
   - **Change the currency exchange rate.**
3. **Currency exchange rate board** (Валютын ханшийн дэлгэц) — Blazor web app. Reads data via
   WebAPI, and must update **in realtime** the instant a teller changes a rate (push, not polling).
4. **Server** — the hub connecting all of the above. Assignment explicitly asks for several
   server-side pieces:
   - **WebAPI** — carries the main data (accounts, transfers, rates, ticket numbers).
   - **Socket server** — talks to the number-display screens in realtime, and to the WebAPI/rest
     of the system. (We're using **SignalR** for this — see decisions below.)
   - **Queue/Pipeline/Channel** — serializes concurrent requests so that (a) two simultaneous
     transfer requests can't double-process the same transaction, and (b) the same queue number
     never gets shown/advanced twice on different screens at once.

### Grading requirements to keep in mind

- **Every part needs unit tests.**
- **The team must be able to fully explain any AI-generated code.** This is explicit in the
  assignment. Treat it as a hard constraint on *how* Claude should help (see bottom section).

## Architecture decisions made so far

| Decision | Choice | Why |
|---|---|---|
| Teller app UI | **WinForms**, not WinUI 3 | Same toolkit as the (mandatory-WinForms) number dispenser — one UI framework to learn, not two. WinUI 3 needs the Windows App SDK workload (not installed) and MSIX-style packaging, which is fragile outside Visual Studio. User picked this explicitly. |
| Realtime transport | **SignalR** (server hub, `Microsoft.AspNetCore.SignalR.Client` in the WinForms/Blazor clients) | Satisfies the "socket server" requirement without hand-rolling raw TCP/WebSocket framing. Built into ASP.NET Core, well documented, realistic for a beginner team to actually finish. |
| Request serialization ("Queue/Pipeline/Channel" requirement) | **`System.Threading.Channels`** inside `BankServer.API` | The assignment literally names "Channel" as an acceptable option. It's in-process, no external broker (no RabbitMQ/Kafka) to stand up, and is small enough for the team to read and explain end-to-end in a viva. A single background consumer processes queue-advance and transfer requests one at a time, which is what prevents double-processing. |
| Currency board rendering | **Blazor Server**, global interactive render mode | Simplest realtime story (no WASM/CORS complexity) and matches the professor's own demo structure (`ProductsFront`). |
| Solution format | Classic `.sln`/`.slnx` via `dotnet sln` | |
| Running everything on one machine | `BankServer.API`, `CurrencyBoard.Blazor`, and `postgres` run in **Docker containers** (via `docker-compose.yml`); `NumberDispenser.WinForms` and `TellerApp.WinForms` run **natively** on the host, pointed at `http://localhost:5100` | WinForms apps need a visible Windows desktop to draw their window — a container has no screen, so they can't usefully run inside one. The server-side pieces are ordinary headless services and containerize fine. |
| Database | **PostgreSQL**, containerized (also part of `docker-compose.yml`) | User's choice. Schema/seed data lives in `BankServer.API/Data/init.sql`, auto-run once by the official postgres image on first container start — no EF migrations needed. Same file is reused by the test suite (see Testing below). |
| DB access | **Raw ADO.NET via `Npgsql`**, small `Repo` classes — not Entity Framework | Matches the professor's own example exactly (`ProductRepo.cs` uses raw `System.Data.SQLite`, hand-written SQL, no ORM). Same idiom, just Postgres instead of SQLite. Easiest to defend line-by-line in a viva; no change-tracking/migration machinery to explain. |
| UI theme | Shared navy (`#0B2545`) / gold (`#C9A227`) "bank" palette across all 3 client apps — header bar + accent stripe pattern, gold primary buttons | User asked to make the apps look less like bare templates. Each WinForms project has its own tiny local `BankColors` class (not shared via `BankServer.Shared`, since `Color` needs a GDI dependency that a headless server project shouldn't pull in); the Blazor app defines the same palette as CSS variables in `wwwroot/app.css`. |
| Repo-level testing | **Testcontainers.PostgreSql** — real ephemeral Postgres per test run, not mocks | The Repo classes are raw SQL; a mock can't catch a typo'd column name or a wrong `ON CONFLICT` clause, only a real database can. Testcontainers spins one up in Docker automatically, using the *same* `init.sql` the real app uses (see Testing below), so the schema tested is always the one that actually ships. |

## Solution structure

```
BankServer.API/              (repo root, solution: BankServer.slnx)
├── BankServer.slnx
├── BankServer.API/          WebAPI + SignalR hubs + Channel-based queue (the "server")
│   ├── Data/                Repo classes (AccountRepo, TicketRepo, ExchangeRateRepo) + init.sql
│   ├── Hubs/                QueueHub, RatesHub (push-only SignalR hubs)
│   └── Queueing/             SerialRequestQueue + RequestQueueProcessor (the Channel-based queue)
├── BankServer.Shared/       Class library: DTOs, hub contracts, BankApiClient, ApiConfig --
│                            shared by every client project so nothing's duplicated 3x
├── NumberDispenser.WinForms/  Ticket kiosk client (KioskForm)
├── TellerApp.WinForms/        Teller client (TellerForm) -- 3-tab UI for the 3 actions
├── CurrencyBoard.Blazor/      Public exchange-rate display, realtime via SignalR (Home.razor)
└── BankServer.API.Tests/      xUnit tests
    ├── Queueing/              SerialRequestQueueTests -- concurrency proof (no real DB needed)
    └── Data/                  Repo tests against a real Testcontainers Postgres
```

All client projects (`NumberDispenser.WinForms`, `TellerApp.WinForms`, `CurrencyBoard.Blazor`)
reference `BankServer.Shared` so DTOs, the hub contracts, and the HTTP-calling code
(`BankApiClient`) are never duplicated three times. `BankServer.API.Tests` references
`BankServer.API` + `BankServer.Shared`.

**Status as of this writing:** All 4 required parts exist and have been verified working
end-to-end against the real Docker stack (Postgres + BankServer.API + CurrencyBoard.Blazor
containerized; both WinForms apps run natively and were confirmed live against it). 21 unit tests
passing (3 queue concurrency + 18 real-Postgres repo tests). Code's pushed to
`github.com/namkhaig7/teller-system` on `main`. Remaining roadmap items are endpoint-level tests
for `Program.cs` and continued UI polish.

## Reference example

`C:\Users\Namkhai\Downloads\asp\asp` is the professor's in-lab demo (`ProductsAPI`/`ProductsFront`,
a `BaraaAPI`/`BaraaFront` variant of the same thing): a minimal WebAPI + Blazor Server pair reading/
writing a SQLite DB through a `Repo` class. Useful as a sanity check for "what does the professor
consider a normal, gradeable shape for a WebAPI+Blazor pair" — our server/currency-board pair should
look recognizably like that, just extended with SignalR + the Channel queue + the two WinForms apps.

## Running everything on one machine

`docker-compose.yml` has 3 services: `postgres`, `bankserver-api`, `currencyboard`.

- `docker compose up --build` starts all three: Postgres, `BankServer.API` (host port **5100**),
  and `CurrencyBoard.Blazor` (host port **5200**, open in a browser).
- Inside the compose network, containers reach each other by service name, not `localhost`:
  `bankserver-api` connects to Postgres at `Host=postgres;...` and `currencyboard` reaches the API
  at `http://bankserver-api:8080` via the `BANKSERVER_API_URL` env var. **Watch this one** — it
  broke once already (see "Bugs found & fixed" below).
- Postgres also publishes port **5432** to the host, so `BankServer.API` can alternatively be run
  **natively** (`dotnet run`, or F5 in the IDE) against the same database — useful for fast
  edit/debug loops without rebuilding a Docker image every time. `appsettings.Development.json`
  already has a `ConnectionStrings:Postgres` pointing at `localhost:5432` for this.
- Run `NumberDispenser.WinForms` and `TellerApp.WinForms` normally from Visual Studio / `dotnet run`
  on the host (they can't run in a container — no desktop to draw a window on). Point them at
  `http://localhost:5100` for the API/SignalR hub (`ApiConfig.BaseUrl`, overridable via the
  `BANKSERVER_API_URL` env var).
- Docker Desktop installs **per-user** on this machine (`%LOCALAPPDATA%\Programs\DockerDesktop`)
  and does **not** auto-launch after install — start it manually (Start Menu → "Docker Desktop")
  before `docker compose up` will work.
- Dev-only credentials (`bankserver` / `bankserver_dev_pw`) sit in `docker-compose.yml` and
  `appsettings.Development.json` in plain text. Fine for a local course project; don't reuse them
  anywhere real.

## Bugs found & fixed (worth knowing for the demo)

- **Env var name mismatch.** `docker-compose.yml` originally set `BankServerApi__BaseUrl` for
  `currencyboard`'s container-to-container API URL, but the code (`BankServer.Shared/ApiConfig.cs`)
  only ever read `BANKSERVER_API_URL`. The names never matched, so inside the container it silently
  fell back to `localhost:5100` (itself) and every API call failed with connection-refused. Caught
  by actually running the container and reading its logs, not by inspecting the code. Fixed by
  renaming the compose env var to match. **This is good evidence to cite if asked "how do you know
  it works"** — it's a real bug that was found and fixed, not a hypothetical.

## Contract design (`BankServer.Shared`)

DTOs live in `BankServer.Shared/Dtos`, hub contracts in `BankServer.Shared/Hubs`. Key design point
to remember (and be ready to explain):

- **Actions go through WebAPI, not through the hubs.** The Teller app calls plain HTTP endpoints
  to call-next / transfer / change a rate (matches "Теллерийн апп ... WebAPI ашиглан харилцана" in
  the assignment). The hubs (`IQueueDisplayClient`, `IRatesClient`) are **push-only, one-way,
  server → client** — display screens and the currency board just connect and listen, they never
  call anything on the hub themselves. The WebAPI endpoint, after the Channel-queue processes an
  action, is what calls `hubContext.Clients.All.CustomerCalled(...)` / `RateChanged(...)`.
- This keeps each side simple to explain: "WebAPI = requests that change something", "SignalR =
  read-only live feed of state changes" — no mixing the two.
- **`BankApiClient` + `ApiConfig`** (also in `BankServer.Shared`) are the one place every client
  project's HTTP-calling code and "where's the server" logic live, so it's written once instead of
  three times.

## Queue design (`BankServer.API/Queueing`)

`SerialRequestQueue` wraps a `Channel<Func<Task>>`; `RequestQueueProcessor` (a `BackgroundService`)
is the single consumer that drains it for the whole lifetime of the app. `call-next` and `transfer`
requests are wrapped as work items and run through it — that's what stops two requests arriving at
the same instant from both reading the same "before" state and stepping on each other.

**Not everything goes through the queue** — and knowing *why* is a good thing to be able to explain:
- Ticket issuance (`TicketRepo.IssueNextTicketAsync`) doesn't need it: Postgres's `SERIAL` column
  already hands out a unique, increasing number atomically per `INSERT`, so there's no read-then-
  write race to protect against.
- Rate updates (`ExchangeRateRepo.UpdateRateAsync`) don't need it either: it's a single atomic
  `INSERT ... ON CONFLICT DO UPDATE` (upsert) statement, not a separate read-then-write.
- Only `AccountRepo.TransferAsync` (read two balances, then write two balances) and
  `TicketRepo.CallNextAsync` (read the next waiting ticket, then mark it called) have a real
  read-then-write race to close, which is exactly what the queue's serialization closes.

## Testing (`BankServer.API.Tests`)

Two kinds of tests, deliberately kept separate:

- **`Queueing/SerialRequestQueueTests.cs`** — no database involved. Proves `SerialRequestQueue`
  actually serializes work items: fires many concurrent "read a shared value, pause, write it
  back" work items (the same shape as a transfer) through the queue and asserts zero updates are
  lost, and a second test asserts no two work items ever run at the same time. This is the
  strongest evidence for the assignment's "prevent duplicate processing" requirement.
- **`Data/*RepoTests.cs`** — real Postgres, via **Testcontainers.PostgreSql**
  (`Data/PostgresFixture.cs`). The Repo classes are raw hand-written SQL, not an ORM, so the only
  way to genuinely test a query is to run it against a real database — a mock can't catch a
  typo'd column name. The fixture spins up a throwaway `postgres:17-alpine` container in Docker
  and mounts the **same** `BankServer.API/Data/init.sql` the real app/`docker-compose.yml` use
  (found by walking up from the test binary's folder to `BankServer.slnx`, then down into
  `BankServer.API/Data`), so the schema under test can never quietly drift from what actually
  ships. All Repo test classes share one `[Collection("Postgres")]` — one container for the whole
  run (fast), and xUnit runs same-collection tests sequentially (safe, no cross-test races on the
  shared DB). Each test inserts its own uniquely-named test accounts rather than depending on
  exact starting balances, so tests can't interfere with each other.
- **Requires Docker running** to execute (`dotnet test` will fail to start the container
  otherwise) — same Docker Desktop already needed for `docker compose up`.
- **21 tests total, all passing**: 3 queue + 8 `AccountRepo` + 4 `TicketRepo` + 3 `ExchangeRateRepo`.
  Run `dotnet test` to confirm the current count as more get added.

## Roadmap (rough, will get more detailed as we go)

- [x] `BankServer.Shared`: core DTOs, SignalR hub contracts (`IQueueDisplayClient`/`IRatesClient`),
      `BankApiClient`, `ApiConfig`.
- [x] `BankServer.API`: `Repo` classes, `Channel`-based queue + `RequestQueueProcessor`, SignalR
      hubs, all endpoints wired up in `Program.cs`, PostgreSQL schema/seed (`Data/init.sql`).
- [x] `NumberDispenser.WinForms` (`KioskForm`): take-a-number button + live "now serving" via
      `QueueHub`. Bank-themed (navy header, gold accent, card layout).
- [x] `TellerApp.WinForms` (`TellerForm`): 3-tab UI (call next, transfer, rates), live rate sync
      via `RatesHub`. Bank-themed to match the kiosk.
- [x] `CurrencyBoard.Blazor` (`Home.razor`): live rate board via `RatesHub`, navy/gold themed with
      a "Шууд эфир / Live" connection-status badge.
- [x] `docker compose up --build` verified end-to-end against real HTTP requests (tickets,
      call-next, accounts, transfers incl. insufficient-funds, rate updates), both hub negotiate
      endpoints, both WinForms apps confirmed live against the containerized stack by the user.
- [x] Unit tests: queue concurrency (mock-free, proves the actual race is prevented) + Repo tests
      against real Postgres via Testcontainers (no database mocking — see Testing above).
- [x] Pushed to `github.com/namkhaig7/teller-system` (`main` branch).
- [ ] Endpoint-level tests for `Program.cs` (e.g. via `WebApplicationFactory<Program>` — would need
      a `public partial class Program;` marker line added since it's currently top-level
      statements, plus a real or Testcontainers Postgres to run against).
- [ ] Unit tests for the WinForms/Blazor client code itself (currently untested — mostly UI glue
      calling already-tested `BankApiClient`/`Repo` methods, but worth a pass).

## How to work with me (important — read before writing code)

This is a graded assignment for a 2nd-year student who **must be able to explain any AI-assisted
code to the professor.** That changes how I should help, compared to normal professional work:

- **Prefer the boring, standard-library way over the clever way.** `System.Threading.Channels`
  over a custom lock-free queue; built-in SignalR over hand-rolled sockets; simple LINQ over dense
  one-liners. Something a 2nd-year should be able to read and defend beats something impressive.
- **Explain non-obvious concurrency/realtime concepts as we introduce them**, briefly, inline —
  not a wall of text, but enough that the student isn't just copy-pasting. Things like "why a
  single consumer loop prevents double-processing" or "why repo tests need a real database, not a
  mock" are exactly the kind of thing a professor will ask about in a demo.
- **Don't over-engineer.** No generic repository-pattern-over-repository-pattern, no premature
  abstractions "in case we need it later." This is a course project with a fixed, known scope.
- **Keep comments minimal** (project convention) but where a comment IS warranted, prefer it be
  the kind of thing that helps explain the code to the professor later, not restate the obvious.
- Team project — multiple people will touch this. Keep naming and structure predictable rather
  than personally clever.
- **Verify by actually running things**, not just by reading code — several real bugs (see "Bugs
  found & fixed") were only caught this way. Docker is installed and working on this machine; use
  it.
