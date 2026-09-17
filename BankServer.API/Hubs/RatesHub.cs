using BankServer.Shared.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace BankServer.API.Hubs;

// Push-only: the currency board connects and listens, it never calls anything on this
// hub itself -- see "Contract design" in CLAUDE.md.
public class RatesHub : Hub<IRatesClient>;
