using BankServer.Shared.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace BankServer.API.Hubs;

// Push-only: number-display screens (and the teller's own queue view) connect and listen,
// they never call anything on this hub themselves -- see "Contract design" in CLAUDE.md.
public class QueueHub : Hub<IQueueDisplayClient>;
