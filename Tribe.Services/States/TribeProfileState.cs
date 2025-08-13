using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Tribe.Bib.Models.TribeRelated;
using Tribe.Client.Services;


namespace Tribe.Services.States
{
    public class TribeProfileState : IDisposable
    {
        private readonly IClientApiService _clientApiService;
        private readonly AuthenticationStateProvider _authProvider;

        public TribeProfile? CurrentTribeProfile { get; private set; }
        public event Action? OnChange;

        public TribeProfileState(IClientApiService clientApiService,
                                 AuthenticationStateProvider authProvider)
        {
            _clientApiService = clientApiService;
            _authProvider = authProvider;

            _authProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
        }

        // 🔔 Event-Handler: async void ist hier OK (Ereignis-Callback)
        private async void OnAuthenticationStateChanged(Task<AuthenticationState> task)
        {
            await LoadProfileFromAuthenticationState(task);
        }

        // ✅ Diese Methode ist awaitable!
        private async Task LoadProfileFromAuthenticationState(Task<AuthenticationState> authTask)
        {
            try
            {
                var authState = await authTask;
                var userId = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    CurrentTribeProfile = await _clientApiService.GetByIdAsync<TribeProfile>(userId);
                }
                else
                {
                    CurrentTribeProfile = null;
                }

                NotifyStateChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading tribe profile: {ex.Message}");
                CurrentTribeProfile = null;
                NotifyStateChanged();
            }
        }

        // ✅ Jetzt kannst du das manuell aufrufen und awaiten
        public async Task LoadCurrentTribeProfileAsync()
        {
            await LoadProfileFromAuthenticationState(_authProvider.GetAuthenticationStateAsync());
        }

        private void NotifyStateChanged() => OnChange?.Invoke();

        public void Dispose()
        {
            _authProvider.AuthenticationStateChanged -= OnAuthenticationStateChanged;
        }
    }
}