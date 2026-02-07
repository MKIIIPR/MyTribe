using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tribe.Data;
using Tribe.Controller.Services;
using TribeApp.Repositories;
using Tribe.Controller.ShopController.Validators;
using static Tribe.Bib.ShopRelated.ShopStruckture;

namespace Tribe.Controller.ShopController
{
    [ApiController]
    [Route("api/shop/orders")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrdersController : ControllerBase
    {
        private readonly ShopDbContext _context;
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(
            ShopDbContext context,
            IOrderService orderService,
            IPaymentService paymentService,
            ILogger<OrdersController> logger)
        {
            _context = context;
            _orderService = orderService;
            _paymentService = paymentService;
            _logger = logger;
        }

        private string GetUserId() => User.FindFirstValue("profileId") ?? string.Empty;

        // Create order (customer)
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateOrder([FromBody] ShopOrder order)
        {
            order.Id = Guid.NewGuid().ToString();
            order.OrderNumber = "ORD-" + DateTime.UtcNow.Ticks;
            order.CreatedAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;

            _context.ShopOrders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        [HttpPost("checkout")]
        [AllowAnonymous]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
        {
            if (request?.Order == null || request?.Items == null || request.Items.Count == 0)
                return BadRequest(new { message = "Invalid order or items" });

            // Validate
            var validationErrors = ProductValidator.ValidateOrder(request.Order, request.Items);
            if (validationErrors.Count > 0)
                return BadRequest(new { errors = validationErrors.Select(e => e.ErrorMessage) });

            // Create order
            var (success, orderId, total, message) = await _orderService.CreateOrderAsync(request.Order, request.Items);
            if (!success)
                return BadRequest(new { message = message });

            // Initiate payment (placeholder URL for MVP)
            var (paymentSuccess, paymentUrl, paymentMessage) = await _paymentService.InitiatePaymentAsync(request.Order);
            if (!paymentSuccess)
                return BadRequest(new { message = paymentMessage });

            _logger.LogInformation("Checkout initiated: OrderId {OrderId}, PaymentUrl: {PaymentUrl}", orderId, paymentUrl);
            return Ok(new
            {
                orderId = orderId,
                orderNumber = request.Order.OrderNumber,
                total = total,
                paymentUrl = paymentUrl,
                message = "Proceed to payment"
            });
        }

        [HttpPost("{orderId}/mark-paid")]
        public async Task<IActionResult> MarkOrderPaid(string orderId)
        {
            var order = await _context.ShopOrders.FindAsync(orderId);
            if (order == null) return NotFound();

            var userId = GetUserId();
            if (order.CreatorProfileId != userId) return Forbid();

            var success = await _paymentService.MarkOrderAsPaidAsync(orderId);
            if (!success)
                return BadRequest(new { message = "Failed to mark order as paid" });

            _logger.LogInformation("Order {OrderId} marked as paid by {UserId}", orderId, userId);
            return Ok(new { message = "Order marked as paid" });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(string id)
        {
            var order = await _context.ShopOrders.FindAsync(id);
            if (order == null) return NotFound();

            var userId = GetUserId();
            if (order.CustomerUserId != userId && order.CreatorProfileId != userId)
                return Forbid();

            return Ok(order);
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = GetUserId();
            var orders = _context.ShopOrders.Where(o => o.CustomerUserId == userId).OrderByDescending(o => o.CreatedAt).ToList();
            return Ok(orders);
        }

        [HttpGet("creator")]
        public async Task<IActionResult> GetCreatorOrders()
        {
            var userId = GetUserId();
            var orders = _context.ShopOrders.Where(o => o.CreatorProfileId == userId).OrderByDescending(o => o.CreatedAt).ToList();
            return Ok(orders);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(string id, [FromBody] string newStatus)
        {
            if (string.IsNullOrEmpty(newStatus))
                return BadRequest(new { error = "Status is required" });

            if (!OrderStatus.All.Contains(newStatus))
                return BadRequest(new { error = "Invalid status" });

            var order = await _context.ShopOrders.FindAsync(id);
            if (order == null) return NotFound();
            
            var userId = GetUserId();
            if (order.CreatorProfileId != userId) return Forbid();

            order.Status = newStatus;
            order.UpdatedAt = DateTime.UtcNow;
            _context.ShopOrders.Update(order);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Order {OrderId} status updated to {Status}", id, newStatus);
            return NoContent();
        }

        public class CheckoutRequest
        {
            public ShopOrder? Order { get; set; }
            public List<ShopOrderItem>? Items { get; set; }
        }
    }
}
