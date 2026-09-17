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
| Solution format | Classic `.sln` via `dotnet sln` (professor used `.slnx`, either works — `.sln` is more predictable from the CLI) | |
| Running everything on one machine | `BankServer.API` and `CurrencyBoard.Blazor` run in **Docker containers** (via `docker-compose.yml`); `NumberDispenser.WinForms` and `TellerApp.WinForms` run **natively** on the host, pointed at `http://localhost:5100` | WinForms apps need a visible Windows desktop to draw their window — a container has no screen, so they can't usefully run inside one. The two server processes (API + Blazor) are ordinary headless services and containerize fine. |

## Solution structure

```
BankServer.API/              (repo root, solution: BankServer.slnx)
├── BankServer.slnx
├── BankServer.API/          WebAPI + SignalR hubs + Channel-based queue (the "server")
├── BankServer.Shared/       Class library: DTOs/contracts shared by every project
│                            (TicketDto, TransferRequestDto, ExchangeRateDto, hub method names, etc.)
├── NumberDispenser.WinForms/  Ticket kiosk client
├── TellerApp.WinForms/        Teller client (3 actions above)
├── CurrencyBoard.Blazor/      Public exchange-rate display, realtime via SignalR
└── BankServer.API.Tests/      xUnit tests (queue/channel logic, transfer logic, rate logic)
```

All client projects (`NumberDispenser.WinForms`, `TellerApp.WinForms`, `CurrencyBoard.Blazor`)
reference `BankServer.Shared` so DTOs are never duplicated/hand-typed twice. `BankServer.API.Tests`
references `BankServer.API` + `BankServer.Shared`.

**Status as of this writing:** solution/project skeleton scaffolded and building cleanly
(`dotnet build BankServer.slnx`). No domain logic yet — `BankServer.API` still has the default
`/weatherforecast` template endpoint, and the client apps are template blank forms/pages. Next
work is the actual domain model and endpoints (see Roadmap).

## Reference example

`C:\Users\Namkhai\Downloads\asp\asp` is the professor's in-lab demo (`ProductsAPI`/`ProductsFront`,
a `BaraaAPI`/`BaraaFront` variant of the same thing): a minimal WebAPI + Blazor Server pair reading/
writing a SQLite DB through a `Repo` class. Useful as a sanity check for "what does the professor
consider a normal, gradeable shape for a WebAPI+Blazor pair" — our server/currency-board pair should
look recognizably like that, just extended with SignalR + the Channel queue + the two WinForms apps.

## Running everything on one machine

- `docker-compose up --build` starts `BankServer.API` (host port **5100**) and `CurrencyBoard.Blazor`
  (host port **5200**, open in a browser).
- Inside the compose network, `CurrencyBoard.Blazor` reaches the API at `http://bankserver-api:8080`
  (compose service name, set via the `BankServerApi__BaseUrl` env var in `docker-compose.yml` — read
  this into config once the Blazor app actually calls the API/hub).
- Run `NumberDispenser.WinForms` and `TellerApp.WinForms` normally from Visual Studio / `dotnet run`
  on the host (they can't run in a container — no desktop to draw a window on). Point them at
  `http://localhost:5100` for the API/SignalR hub.
- Docker was **not installed** on the dev machine as of this setup — install Docker Desktop for
  Windows (needs WSL2 backend enabled) before `docker-compose up` will work. The Dockerfiles/
  compose file are written and ready; just untested against an actual daemon yet.

## Contract design (`BankServer.Shared`)

All 6 DTOs and the two hub client interfaces live in `BankServer.Shared/Dtos` and
`BankServer.Shared/Hubs`. Key design point to remember (and be ready to explain):

- **Actions go through WebAPI, not through the hubs.** The Teller app calls plain HTTP endpoints
  to call-next / transfer / change a rate (matches "Теллерийн апп ... WebAPI ашиглан харилцана" in
  the assignment). The hubs (`IQueueDisplayClient`, `IRatesClient`) are **push-only, one-way,
  server → client** — display screens and the currency board just connect and listen, they never
  call anything on the hub themselves. The WebAPI controller, after the Channel-queue processes an
  action, is what calls `hubContext.Clients.All.CustomerCalled(...)` / `RateChanged(...)`.
- This keeps each side simple to explain: "WebAPI = requests that change something", "SignalR =
  read-only live feed of state changes" — no mixing the two.

## Roadmap (rough, will get more detailed as we go)

- [x] `BankServer.Shared`: core DTOs (`TicketDto`, `AccountDto`, `TransferRequestDto`/`TransferResultDto`,
      `ExchangeRateDto`/`UpdateExchangeRateRequestDto`, `CalledCustomerDto`) and SignalR hub contract
      — `IQueueDisplayClient`/`IRatesClient` (strongly-typed `Hub<T>` client interfaces, so a typo in
      a method name fails to compile instead of silently no-op'ing) + `HubRoutes` constants.
- [ ] `BankServer.API`: in-memory (or SQLite, matching the professor's example) domain store for
      accounts, tickets, exchange rates.
- [ ] `BankServer.API`: `Channel<T>`-based queue + single background `IHostedService` consumer for
      (a) "give me next ticket number" and (b) "process this transfer" — this is what makes both
      operations safe under concurrent requests.
- [ ] `BankServer.API`: SignalR hub(s) — one for number-display push updates, one (or shared) for
      exchange-rate push updates.
- [ ] `NumberDispenser.WinForms`: call WebAPI for "give me next number", display/"print" it.
- [ ] `TellerApp.WinForms`: the 3 actions, wired to WebAPI + SignalR client.
- [ ] `CurrencyBoard.Blazor`: read current rates via WebAPI on load, subscribe to SignalR hub for
      live updates.
- [ ] Unit tests per part, especially concurrency tests for the Channel-based queue (fire N
      concurrent requests, assert no double-processing).

## How to work with me (important — read before writing code)

This is a graded assignment for a 2nd-year student who **must be able to explain any AI-assisted
code to the professor.** That changes how I should help, compared to normal professional work:

- **Prefer the boring, standard-library way over the clever way.** `System.Threading.Channels`
  over a custom lock-free queue; built-in SignalR over hand-rolled sockets; simple LINQ over dense
  one-liners. Something a 2nd-year should be able to read and defend beats something impressive.
- **Explain non-obvious concurrency/realtime concepts as we introduce them**, briefly, inline —
  not a wall of text, but enough that the student isn't just copy-pasting. Things like "why a
  single consumer loop prevents double-processing" or "why SignalR groups are used for the number
  display screens" are exactly the kind of thing a professor will ask about in a demo.
- **Don't over-engineer.** No generic repository-pattern-over-repository-pattern, no premature
  abstractions "in case we need it later." This is a course project with a fixed, known scope.
- **Keep comments minimal** (project convention) but where a comment IS warranted, prefer it be
  the kind of thing that helps explain the code to the professor later, not restate the obvious.
- Team project — multiple people will touch this. Keep naming and structure predictable rather
  than personally clever.
