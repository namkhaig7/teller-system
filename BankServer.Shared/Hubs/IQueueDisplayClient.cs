using BankServer.Shared.Dtos;

namespace BankServer.Shared.Hubs;

// Methods the server calls ON the number-display screens (and the teller's own queue view).
// Server implements: Hub<IQueueDisplayClient>. Clients implement this via
// connection.On<CalledCustomerDto>(nameof(IQueueDisplayClient.CustomerCalled), ...).
// Sharing this interface means a typo in a method name is a compile error, not a silent
// runtime no-op like it would be with plain hub.Clients.All.SendAsync("CustomerCalled", ...).
public interface IQueueDisplayClient
{
    Task CustomerCalled(CalledCustomerDto info);
}
