using static Tribe.Bib.ShopRelated.ShopStruckture;
using Tribe.Data;
using TribeApp.Repositories;

namespace Tribe.Controller.Services
{
    public interface IPaymentService
    {
        Task<(bool Success, string? PaymentUrl, string? Message)> InitiatePaymentAsync(ShopOrder order);
        Task<bool> MarkOrderAsPaidAsync(string orderId);
    }

    public class PaymentService : IPaymentService
    {
        private readonly ShopDbContext _context;
        private readonly IDownloadService _downloadService;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(ShopDbContext context, IDownloadService downloadService, ILogger<PaymentService> logger)
        {
            _context = context;
            _downloadService = downloadService;
            _logger = logger;
        }

        /// <summary>
        /// Initiates payment - returns a placeholder payment URL for MVP
        /// In production: integrate with Stripe, PayPal, etc.
        /// </summary>
        public async Task<(bool Success, string? PaymentUrl, string? Message)> InitiatePaymentAsync(ShopOrder order)
        {
            try
            {
                if (order == null || string.IsNullOrEmpty(order.Id))
                    return (false, null, "Order is invalid");

                // Generate payment URL (placeholder for MVP)
                // In production: call Stripe/PayPal API to get real payment URL
                var paymentUrl = $"https://payment.example.com/checkout?orderId={order.Id}&amount={order.TotalAmount}";

                _logger.LogInformation("Payment initiated for Order {OrderId}, Amount: {Amount}", order.Id, order.TotalAmount);
                return (true, paymentUrl, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initiating payment");
                return (false, null, ex.Message);
            }
        }

        /// <summary>
        /// Marks an order as paid and generates download tokens for digital products
        /// Call this after payment provider confirms payment (webhook, callback, etc.)
        /// </summary>
        public async Task<bool> MarkOrderAsPaidAsync(string orderId)
        {
            try
            {
                var order = await _context.ShopOrders.FindAsync(orderId);
                if (order == null)
                {
                    _logger.LogWarning("Order {OrderId} not found", orderId);
                    return false;
                }

                order.PaidAt = DateTime.UtcNow;
                order.Status = OrderStatus.Confirmed;
                order.UpdatedAt = DateTime.UtcNow;

                _context.ShopOrders.Update(order);
                await _context.SaveChangesAsync();

                // Auto-generate download tokens for digital items
                await GenerateDownloadTokensAsync(orderId);

                _logger.LogInformation("Order {OrderId} marked as paid", orderId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking order as paid");
                return false;
            }
        }

        private async Task GenerateDownloadTokensAsync(string orderId)
        {
            var items = _context.ShopOrderItems.Where(i => i.OrderId == orderId).ToList();
            foreach (var item in items)
            {
                var token = await _downloadService.GenerateTokenForOrderItemAsync(item.Id, validDays: 7, maxDownloads: 5);
                if (token != null)
                {
                    _logger.LogInformation("Download token generated for OrderItem {ItemId}", item.Id);
                }
                else
                {
                    _logger.LogWarning("Failed to generate token for OrderItem {ItemId}", item.Id);
                }
            }
        }
    }
}
