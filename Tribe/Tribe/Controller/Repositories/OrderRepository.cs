using Microsoft.EntityFrameworkCore;
using Tribe.Data;
using static Tribe.Bib.ShopRelated.ShopStruckture;

namespace TribeApp.Repositories
{
    public interface IOrderRepository
    {
        Task<string> CreateOrderAsync(ShopOrder order);
        Task<ShopOrder?> GetOrderByIdAsync(string orderId);
        Task<List<ShopOrder>> GetOrdersByCustomerAsync(string customerUserId);
        Task<List<ShopOrder>> GetOrdersByCreatorAsync(string creatorProfileId);
        Task<bool> UpdateOrderStatusAsync(string orderId, string status);
    }

    public class OrderRepository : IOrderRepository
    {
        private readonly ShopDbContext _context;

        public OrderRepository(ShopDbContext context)
        {
            _context = context;
        }

        public async Task<string> CreateOrderAsync(ShopOrder order)
        {
            order.Id = Guid.NewGuid().ToString();
            order.OrderNumber = "ORD-" + DateTime.UtcNow.Ticks;
            order.CreatedAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.ShopOrders.AddAsync(order);
            await _context.SaveChangesAsync();
            return order.Id;
        }

        public async Task<ShopOrder?> GetOrderByIdAsync(string orderId)
        {
            return await _context.ShopOrders.FindAsync(orderId);
        }

        public async Task<List<ShopOrder>> GetOrdersByCustomerAsync(string customerUserId)
        {
            return await _context.ShopOrders
                .Where(o => o.CustomerUserId == customerUserId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<ShopOrder>> GetOrdersByCreatorAsync(string creatorProfileId)
        {
            return await _context.ShopOrders
                .Where(o => o.CreatorProfileId == creatorProfileId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> UpdateOrderStatusAsync(string orderId, string status)
        {
            var existing = await _context.ShopOrders.FindAsync(orderId);
            if (existing == null) return false;
            existing.Status = status;
            existing.UpdatedAt = DateTime.UtcNow;
            _context.ShopOrders.Update(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
