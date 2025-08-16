using System.Security.Claims;
using Tribe.Bib.CommunicationModels;
using Tribe.Bib.Models.TribeRelated;
using Tribe.Client.Services;
using Tribe.Services.States.Tribe.Services.States;
using static Tribe.Bib.CommunicationModels.ComModels;

namespace Tribe.Services.States
{
    public class UserState : IDisposable
    {
        public event Action? OnChange;

        public ClaimsPrincipal? CurrentUser { get; private set; }
        public TribeProfile? TribeProfile { get; private set; }
        public bool IsAuthenticated => CurrentUser?.Identity?.IsAuthenticated ?? false;
        public List<Claim> UserClaims { get; private set; } = new();
        public int UserClaimsCount => UserClaims.Count;
        public DateTime LastUpdate { get; private set; } = DateTime.Now;
        public int RefreshCount { get; private set; } = 0;

        public void UpdateState(UserStateUpdate update)
        {
            CurrentUser = update.CurrentUser;
            TribeProfile = update.TribeProfile;
            UserClaims = CurrentUser?.Claims?.ToList() ?? new List<Claim>();
            LastUpdate = DateTime.Now;
            RefreshCount++;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();

        public void Dispose()
        {
            OnChange = null;
        }
    }

    namespace Tribe.Services.States
    {
        public class UserStateUpdate
        {
            public ClaimsPrincipal? CurrentUser { get; }
            public TribeProfile? TribeProfile { get; set; }

            public UserStateUpdate(ClaimsPrincipal? user)
            {
                CurrentUser = user;
            }
        }
    }
}