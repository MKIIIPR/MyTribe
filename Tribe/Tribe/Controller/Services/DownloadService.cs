using Microsoft.EntityFrameworkCore;
using Tribe.Data;
using static Tribe.Bib.ShopRelated.ShopStruckture;

namespace Tribe.Controller.Services
{
    public interface IDownloadService
    {
        Task<string?> GenerateTokenForOrderItemAsync(string orderItemId, int validDays = 7, int maxDownloads = 3);
        Task<(bool Success, string? Url, string? Reason)> ValidateAndGetDownloadUrlAsync(string token);
    }

    public class DownloadService : IDownloadService
    {
        private readonly ShopDbContext _context;
        private readonly ILogger<DownloadService> _logger;

        public DownloadService(ShopDbContext context, ILogger<DownloadService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<string?> GenerateTokenForOrderItemAsync(string orderItemId, int validDays = 7, int maxDownloads = 3)
        {
            var item = await _context.ShopOrderItems.FindAsync(orderItemId);
            if (item == null)
            {
                _logger.LogWarning("OrderItem {OrderItemId} not found for token generation", orderItemId);
                return null;
            }

            // generate token
            var token = Guid.NewGuid().ToString("N");
            item.DownloadToken = token;
            item.DownloadExpiry = DateTime.UtcNow.AddDays(validDays);
            item.MaxDownloads = maxDownloads;
            item.DownloadCount = 0;

            _context.ShopOrderItems.Update(item);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Generated download token for OrderItem {OrderItemId}", orderItemId);
            return token;
        }

        public async Task<(bool Success, string? Url, string? Reason)> ValidateAndGetDownloadUrlAsync(string token)
        {
            if (string.IsNullOrEmpty(token)) return (false, null, "missing token");

            var item = await _context.ShopOrderItems
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.DownloadToken == token);

            if (item == null) return (false, null, "token not found");

            if (item.DownloadExpiry.HasValue && item.DownloadExpiry.Value < DateTime.UtcNow)
            {
                return (false, null, "token expired");
            }

            if (item.MaxDownloads > 0 && item.DownloadCount >= item.MaxDownloads)
            {
                return (false, null, "download limit reached");
            }

            // Ensure order is paid or confirmed
            var order = await _context.ShopOrders.FindAsync(item.OrderId);
            if (order == null)
            {
                return (false, null, "order not found");
            }

            // For MVP: require either PaidAt or Status == Confirmed
            if (order.PaidAt == null && !string.Equals(order.Status, "confirmed", StringComparison.OrdinalIgnoreCase))
            {
                return (false, null, "order not paid");
            }

            // Get product download url
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product == null)
            {
                return (false, null, "product not found");
            }

            string? downloadUrl = null;
            switch (product)
            {
                case ImageProduct img:
                    downloadUrl = img.DownloadUrl ?? img.ZipDownloadUrl;
                    break;
                case VideoProduct vid:
                    downloadUrl = vid.DownloadUrl;
                    break;
                default:
                    downloadUrl = null;
                    break;
            }

            if (string.IsNullOrEmpty(downloadUrl))
            {
                return (false, null, "no download available for product type");
            }

            // increment download count (update tracked entity)
            var tracked = await _context.ShopOrderItems.FindAsync(item.Id);
            if (tracked != null)
            {
                tracked.DownloadCount = (tracked.DownloadCount) + 1;
                _context.ShopOrderItems.Update(tracked);
                await _context.SaveChangesAsync();
            }

            return (true, downloadUrl, null);
        }
    }
}
