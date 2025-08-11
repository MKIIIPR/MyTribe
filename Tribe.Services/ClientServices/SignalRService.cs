using Microsoft.AspNetCore.SignalR.Client;

namespace Tribe.Client.Services;

public interface ISignalRService
{
    event Action<string, string>? UserLoggedIn;
    event Action<string, string>? UserLoggedOut;
    Task StartAsync();
    Task StopAsync();
    bool IsConnected { get; }
}
public class SignalRService : ISignalRService, IAsyncDisposable
{
    private HubConnection? _hubConnection;
    private readonly string _baseUrl;

    public event Action<string, string>? UserLoggedIn;
    public event Action<string, string>? UserLoggedOut;

    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

    public SignalRService()
    {
        // FIXED: Verwende absolute URL für WebSocket
        _baseUrl = "https://localhost:7241"; // Ersetze mit deiner tatsächlichen URL
    }

    public async Task StartAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.DisposeAsync();
        }

        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{_baseUrl}/authHub") // ABSOLUTE URL
            .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(10) })
            .Build();

        _hubConnection.On<string, string>("UserLoggedIn", (userId, userName) =>
        {
            Console.WriteLine($"SignalR: UserLoggedIn - {userName}");
            UserLoggedIn?.Invoke(userId, userName);
        });

        _hubConnection.On<string, string>("UserLoggedOut", (userId, userName) =>
        {
            Console.WriteLine($"SignalR: UserLoggedOut - {userName}");
            UserLoggedOut?.Invoke(userId, userName);
        });

        try
        {
            await _hubConnection.StartAsync();
            Console.WriteLine("SignalR Connected Successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SignalR Connection Failed: {ex.Message}");
        }
    }

    public async Task StopAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.DisposeAsync();
            _hubConnection = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
}
