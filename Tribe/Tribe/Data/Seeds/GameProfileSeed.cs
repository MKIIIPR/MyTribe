using System.Collections.Generic;
using System.Threading.Tasks;
using Tribe.Bib.Models.CommunityManagement;
using Microsoft.EntityFrameworkCore;

namespace Tribe.Data.Seeds
{
    public static class GameProfileSeed
    {
        private static readonly IReadOnlyList<GameProfile> Defaults = new[]
        {
            new GameProfile { GameKey = "wow", DisplayName = "World of Warcraft", Genre = "MMORPG", Platform = "PC", Description = "Fantasy PvE/PvP MMORPG" },
            new GameProfile { GameKey = "starcitizen", DisplayName = "Star Citizen", Genre = "Space Simulation", Platform = "PC", Description = "Ship-focused sandbox shooter" },
            new GameProfile { GameKey = "destiny", DisplayName = "Destiny 2", Genre = "Shooter/MMO", Platform = "PC/Console", Description = "Shared-world shooter" },
        };

        public static async Task EnsureSeedAsync(OrgaDbContext db)
        {
            foreach (var profile in Defaults)
            {
                if (await db.GameProfiles.AnyAsync(g => g.GameKey == profile.GameKey))
                {
                    continue;
                }

                db.GameProfiles.Add(profile);
            }

            await db.SaveChangesAsync();
        }
    }
}
