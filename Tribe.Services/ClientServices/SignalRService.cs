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
    private readonly string _hubUrl;

    public event Action<string, string>? UserLoggedIn;
    public event Action<string, string>? UserLoggedOut;

    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

    public SignalRService()
    {
        _hubUrl = "/authHub";
    }

    public async Task StartAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.DisposeAsync();
        }

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(_hubUrl)
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<string, string>("UserLoggedIn", (userId, userName) =>
        {
            UserLoggedIn?.Invoke(userId, userName);
        });

        _hubConnection.On<string, string>("UserLoggedOut", (userId, userName) =>
        {
            UserLoggedOut?.Invoke(userId, userName);
        });

        try
        {
            await _hubConnection.StartAsync();
        }
        catch (Exception)
        {
            // Log error if needed
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