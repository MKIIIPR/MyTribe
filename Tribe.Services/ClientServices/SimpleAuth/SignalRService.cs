using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace Tribe.Services.ClientServices.SimpleAuth;

public interface ISignalRService
{
    event Action<string, string>? UserLoggedIn;
    event Action<string, string>? UserLoggedOut;
    Task StartAsync();
    Task StopAsync();
    bool IsConnected { get; }
}
// ===== 3. SIGNALR SERVICE MIT LOGGING =====

    public class SignalRService : ISignalRService, IAsyncDisposable
    {
        private HubConnection? _hubConnection;
        private readonly string _baseUrl;
        private readonly ILogger<SignalRService> _logger;
        private readonly List<(DateTime Timestamp, string Event, string Details)> _connectionHistory = new();

        public event Action<string, string>? UserLoggedIn;
        public event Action<string, string>? UserLoggedOut;

        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

        public SignalRService(ILogger<SignalRService> logger)
        {
            _baseUrl = "https://localhost:7241";
            _logger = logger;
            _logger.LogInformation("SignalRService initialized with base URL: {BaseUrl}", _baseUrl);
        }

        public async Task StartAsync()
        {
            _logger.LogInformation("Starting SignalR connection to {BaseUrl}/authHub", _baseUrl);

            try
            {
                if (_hubConnection != null)
                {
                    _logger.LogDebug("Disposing existing connection");
                    await _hubConnection.DisposeAsync();
                }

                _hubConnection = new HubConnectionBuilder()
                    .WithUrl($"{_baseUrl}/authHub")
                    .WithAutomaticReconnect(new[] {
                        TimeSpan.Zero,
                        TimeSpan.FromSeconds(2),
                        TimeSpan.FromSeconds(10)
                    })
                    .ConfigureLogging(logging =>
                    {
                        logging.SetMinimumLevel(LogLevel.Information);
                    })
                    .Build();

                // Connection events
                _hubConnection.Closed += OnConnectionClosed;
                _hubConnection.Reconnecting += OnReconnecting;
                _hubConnection.Reconnected += OnReconnected;

                // Auth events
                _hubConnection.On<string, string>("UserLoggedIn", OnUserLoggedInReceived);
                _hubConnection.On<string, string>("UserLoggedOut", OnUserLoggedOutReceived);

                await _hubConnection.StartAsync();

                RecordConnectionEvent("Connected", "Successfully connected to SignalR hub");
                _logger.LogInformation("SIGNALR_CONNECTION: Status=Connected, ConnectionId={ConnectionId}",
                    _hubConnection.ConnectionId);
            }
            catch (Exception ex)
            {
                RecordConnectionEvent("ConnectionFailed", ex.Message);
                _logger.LogError(ex, "SIGNALR_CONNECTION: Status=Failed, Error={ErrorMessage}", ex.Message);
                throw;
            }
        }

        public async Task StopAsync()
        {
            _logger.LogInformation("Stopping SignalR connection");

            try
            {
                if (_hubConnection != null)
                {
                    await _hubConnection.DisposeAsync();
                    _hubConnection = null;
                    RecordConnectionEvent("Disconnected", "Connection stopped manually");
                    _logger.LogInformation("SIGNALR_CONNECTION: Status=Stopped");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping SignalR connection");
            }
        }

        private Task OnConnectionClosed(Exception? exception)
        {
            var errorMessage = exception?.Message ?? "Connection closed normally";
            RecordConnectionEvent("Closed", errorMessage);
            _logger.LogWarning("SIGNALR_CONNECTION: Status=Closed, Reason={Reason}", errorMessage);
            return Task.CompletedTask;
        }

        private Task OnReconnecting(Exception? exception)
        {
            var errorMessage = exception?.Message ?? "Reconnecting";
            RecordConnectionEvent("Reconnecting", errorMessage);
            _logger.LogInformation("SIGNALR_CONNECTION: Status=Reconnecting, Reason={Reason}", errorMessage);
            return Task.CompletedTask;
        }

        private Task OnReconnected(string? connectionId)
        {
            RecordConnectionEvent("Reconnected", $"Reconnected with ID: {connectionId}");
            _logger.LogInformation("SIGNALR_CONNECTION: Status=Reconnected, ConnectionId={ConnectionId}", connectionId);
            return Task.CompletedTask;
        }

        private void OnUserLoggedInReceived(string userId, string userName)
        {
            RecordConnectionEvent("UserLoggedIn", $"User: {userName} (ID: {userId})");
            _logger.LogInformation("SIGNALR_EVENT: UserLoggedIn - UserId={UserId}, UserName={UserName}, Timestamp={Timestamp}",
                userId, userName, DateTime.UtcNow);

            UserLoggedIn?.Invoke(userId, userName);
        }

        private void OnUserLoggedOutReceived(string userId, string userName)
        {
            RecordConnectionEvent("UserLoggedOut", $"User: {userName} (ID: {userId})");
            _logger.LogInformation("SIGNALR_EVENT: UserLoggedOut - UserId={UserId}, UserName={UserName}, Timestamp={Timestamp}",
                userId, userName, DateTime.UtcNow);

            UserLoggedOut?.Invoke(userId, userName);
        }

        private void RecordConnectionEvent(string eventType, string details)
        {
            _connectionHistory.Add((DateTime.UtcNow, eventType, details));

            // Nur letzte 50 Events behalten
            if (_connectionHistory.Count > 50)
            {
                _connectionHistory.RemoveRange(0, _connectionHistory.Count - 50);
            }
        }

        public IReadOnlyList<(DateTime Timestamp, string Event, string Details)> GetConnectionHistory()
        {
            return _connectionHistory.AsReadOnly();
        }

        public async ValueTask DisposeAsync()
        {
            _logger.LogInformation("Disposing SignalRService");

            if (_hubConnection != null)
            {
                await _hubConnection.DisposeAsync();
                RecordConnectionEvent("Disposed", "Service disposed");
            }
        }
    }

