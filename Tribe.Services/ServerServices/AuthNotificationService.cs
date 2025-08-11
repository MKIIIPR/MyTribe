using Microsoft.AspNetCore.SignalR;
using Tribe.Services.Hubs;


namespace Tribe.Services.ServerServices;
public interface IAuthNotificationService
{
    Task NotifyUserLoggedInAsync(string userId, string userName);
    Task NotifyUserLoggedOutAsync(string userId, string userName);
}
public class AuthNotificationService : IAuthNotificationService
{
    private readonly IHubContext<AuthHub> _hubContext;

    public AuthNotificationService(IHubContext<AuthHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyUserLoggedInAsync(string userId, string userName)
    {
        await _hubContext.Clients.All.SendAsync("UserLoggedIn", userId, userName);
    }

    public async Task NotifyUserLoggedOutAsync(string userId, string userName)
    {
        await _hubContext.Clients.All.SendAsync("UserLoggedOut", userId, userName);
    }
}