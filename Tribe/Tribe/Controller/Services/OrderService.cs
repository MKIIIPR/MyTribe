using Tribe.Data;
using static Tribe.Bib.ShopRelated.ShopStruckture;

namespace Tribe.Controller.Services
{
    public interface IOrderService
    {
        Task<(bool Success, string? OrderId, decimal Total, string? Message)> CreateOrderAsync(
            ShopOrder order, 
            List<ShopOrderItem> items);
        Task<bool> GenerateDownloadTokensForOrderAsync(string orderId);
    }

    public class OrderService : IOrderService
    {
        private readonly ShopDbContext _context;
        private readonly IDownloadService _downloadService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(ShopDbContext context, IDownloadService downloadService, ILogger<OrderService> logger)
        {
            _context = context;
            _downloadService = downloadService;
            _logger = logger;
        }

        public async Task<(bool Success, string? OrderId, decimal Total, string? Message)> CreateOrderAsync(
            ShopOrder order, 
            List<ShopOrderItem> items)
        {
            if (order == null || items == null || items.Count == 0)
                return (false, null, 0, "Order or items are empty");

            try
            {
                order.Id = Guid.NewGuid().ToString();
                order.OrderNumber = "ORD-" + DateTime.UtcNow.Ticks;
                order.CreatedAt = DateTime.UtcNow;
                order.UpdatedAt = DateTime.UtcNow;
                order.Status = OrderStatus.Pending;

                // Calculate totals
                decimal subtotal = items.Sum(i => i.TotalPrice);
                order.TotalAmount = subtotal + order.ShippingAmount + order.TaxAmount;

                // Add order & items
                await _context.ShopOrders.AddAsync(order);
                foreach (var item in items)
                {
                    item.Id = Guid.NewGuid().ToString();
                    item.OrderId = order.Id;
                    await _context.ShopOrderItems.AddAsync(item);
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Order created: {OrderId}, Total: {Total}", order.Id, order.TotalAmount);

                return (true, order.Id, order.TotalAmount, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return (false, null, 0, ex.Message);
            }
        }

        public async Task<bool> GenerateDownloadTokensForOrderAsync(string orderId)
        {
            var order = await _context.ShopOrders.FindAsync(orderId);
            if (order == null) return false;

            var items = _context.ShopOrderItems.Where(i => i.OrderId == orderId).ToList();
            foreach (var item in items)
            {
                var token = await _downloadService.GenerateTokenForOrderItemAsync(item.Id, validDays: 7, maxDownloads: 5);
                if (token == null)
                {
                    _logger.LogWarning("Failed to generate token for OrderItem {ItemId}", item.Id);
                }
            }

            return true;
        }
    }
}
