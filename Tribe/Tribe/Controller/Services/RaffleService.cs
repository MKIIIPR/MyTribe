using static Tribe.Bib.ShopRelated.ShopStruckture;
using Tribe.Data;
using Tribe.Bib.Models.TribeRelated;
using Microsoft.EntityFrameworkCore;

namespace Tribe.Controller.Services
{
    public interface IRaffleService
    {
        // Raffle Management
        Task<string> CreateRaffleAsync(Raffle raffle);
        Task<Raffle?> GetRaffleByIdAsync(string raffleId);
        Task<List<Raffle>> GetCreatorRafflesAsync(string creatorId);
        Task<bool> UpdateRaffleAsync(Raffle raffle);
        Task<bool> DeleteRaffleAsync(string raffleId);

        // Product-Raffle Binding
        Task<bool> BindRaffleToProductAsync(string productId, string raffleId);
        Task<bool> UnbindRaffleFromProductAsync(string productId);
        Task<Raffle?> GetProductRaffleAsync(string productId);

        // Entry Management
        Task<bool> AddRaffleEntryAsync(string raffleId, string profileId, int quantity = 1);
        Task<int> GetRaffleEntriesAsync(string raffleId);
    }

    public class RaffleService : IRaffleService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RaffleService> _logger;

        public RaffleService(ApplicationDbContext context, ILogger<RaffleService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<string> CreateRaffleAsync(Raffle raffle)
        {
            try
            {
                if (raffle == null)
                    throw new ArgumentNullException(nameof(raffle));

                raffle.Id = Guid.NewGuid().ToString();
                raffle.CreatedAt = DateTime.UtcNow;
                raffle.Status = "active";

                _context.Raffles.Add(raffle);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Raffle created: {RaffleId} for Creator {CreatorId}", raffle.Id, raffle.CreatorProfileId);
                return raffle.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating raffle");
                throw;
            }
        }

        public async Task<Raffle?> GetRaffleByIdAsync(string raffleId)
        {
            return await _context.Raffles.FindAsync(raffleId);
        }

        public async Task<List<Raffle>> GetCreatorRafflesAsync(string creatorId)
        {
            return await _context.Raffles
                .Where(r => r.CreatorProfileId == creatorId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> UpdateRaffleAsync(Raffle raffle)
        {
            try
            {
                _context.Raffles.Update(raffle);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Raffle updated: {RaffleId}", raffle.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating raffle");
                return false;
            }
        }

        public async Task<bool> DeleteRaffleAsync(string raffleId)
        {
            try
            {
                var raffle = await _context.Raffles.FindAsync(raffleId);
                if (raffle == null) return false;

                _context.Raffles.Remove(raffle);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Raffle deleted: {RaffleId}", raffleId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting raffle");
                return false;
            }
        }

        public async Task<bool> BindRaffleToProductAsync(string productId, string raffleId)
        {
            try
            {
                var binding = await _context.ProductRaffleBindings
                    .FirstOrDefaultAsync(b => b.ProductId == productId);

                if (binding != null)
                {
                    binding.RaffleId = raffleId;
                    binding.UpdatedAt = DateTime.UtcNow;
                    _context.ProductRaffleBindings.Update(binding);
                }
                else
                {
                    var newBinding = new Tribe.Data.ProductRaffleBinding
                    {
                        ProductId = productId,
                        RaffleId = raffleId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.ProductRaffleBindings.Add(newBinding);
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Raffle {RaffleId} bound to Product {ProductId}", raffleId, productId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error binding raffle to product");
                return false;
            }
        }

        public async Task<bool> UnbindRaffleFromProductAsync(string productId)
        {
            try
            {
                var binding = await _context.ProductRaffleBindings
                    .FirstOrDefaultAsync(b => b.ProductId == productId);

                if (binding == null) return false;

                _context.ProductRaffleBindings.Remove(binding);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Raffle unbound from Product {ProductId}", productId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unbinding raffle from product");
                return false;
            }
        }

        public async Task<Raffle?> GetProductRaffleAsync(string productId)
        {
            var binding = await _context.ProductRaffleBindings
                .FirstOrDefaultAsync(b => b.ProductId == productId);

            if (binding == null) return null;

            return await _context.Raffles.FindAsync(binding.RaffleId);
        }

        public async Task<bool> AddRaffleEntryAsync(string raffleId, string profileId, int quantity = 1)
        {
            try
            {
                var raffle = await _context.Raffles.FindAsync(raffleId);
                if (raffle == null) return false;

                var existingEntry = await _context.RaffleEntries
                    .FirstOrDefaultAsync(e => e.RaffleId == raffleId && e.ProfileId == profileId);

                if (existingEntry != null)
                {
                    existingEntry.EntryCount += quantity;
                    _context.RaffleEntries.Update(existingEntry);
                }
                else
                {
                    var entry = new RaffleEntry
                    {
                        RaffleId = raffleId,
                        ProfileId = profileId,
                        EntryCount = quantity,
                        EntryNumbers = GenerateEntryNumbers(raffle.CurrentEntries, quantity),
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.RaffleEntries.Add(entry);
                }

                raffle.CurrentEntries += quantity;
                _context.Raffles.Update(raffle);

                await _context.SaveChangesAsync();
                _logger.LogInformation("Raffle entry added: {Quantity} for User {ProfileId}", quantity, profileId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding raffle entry");
                return false;
            }
        }

        public async Task<int> GetRaffleEntriesAsync(string raffleId)
        {
            var raffle = await _context.Raffles.FindAsync(raffleId);
            return raffle?.CurrentEntries ?? 0;
        }

        private string GenerateEntryNumbers(int startNumber, int quantity)
        {
            var numbers = Enumerable.Range(startNumber, quantity)
                .Select(n => n.ToString())
                .ToList();

            return System.Text.Json.JsonSerializer.Serialize(numbers);
        }
    }
}
