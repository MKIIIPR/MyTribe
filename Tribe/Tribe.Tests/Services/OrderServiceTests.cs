using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tribe.Data;
using Tribe.Controller.Services;
using TribeApp.Repositories;
using static Tribe.Bib.ShopRelated.ShopStruckture;

namespace Tribe.Tests.Services
{
    public class OrderServiceTests
    {
        private ShopDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ShopDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;
            return new ShopDbContext(options);
        }

        [Fact]
        public async Task CreateOrderAsync_Should_Create_Order_With_Correct_Total()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var downloadServiceMock = new Mock<IDownloadService>();
            var loggerMock = new Mock<ILogger<OrderService>>();
            var service = new OrderService(context, downloadServiceMock.Object, loggerMock.Object);

            var order = new ShopOrder 
            { 
                OrderNumber = "ORD1",
                CreatorProfileId = "creator1",
                CustomerUserId = "user1",
                CustomerEmail = "test@example.com",
                CustomerName = "Test User",
                ShippingAmount = 5m,
                TaxAmount = 10m
            };

            var items = new List<ShopOrderItem>
            {
                new ShopOrderItem { ProductId = "p1", ProductTitle = "Product 1", Quantity = 1, UnitPrice = 50m, TotalPrice = 50m },
                new ShopOrderItem { ProductId = "p2", ProductTitle = "Product 2", Quantity = 2, UnitPrice = 25m, TotalPrice = 50m }
            };

            // Act
            var (success, orderId, total, message) = await service.CreateOrderAsync(order, items);

            // Assert
            Assert.True(success);
            Assert.NotEmpty(orderId ?? string.Empty);
            Assert.Equal(115m, total); // 50 + 50 + 5 + 10

            var createdOrder = context.ShopOrders.Find(orderId);
            Assert.NotNull(createdOrder);
            Assert.Equal(115m, createdOrder.TotalAmount);
            Assert.Equal(OrderStatus.Pending, createdOrder.Status);
        }

        [Fact]
        public async Task CreateOrderAsync_Should_Fail_With_Empty_Items()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var downloadServiceMock = new Mock<IDownloadService>();
            var loggerMock = new Mock<ILogger<OrderService>>();
            var service = new OrderService(context, downloadServiceMock.Object, loggerMock.Object);

            var order = new ShopOrder();
            var items = new List<ShopOrderItem>();

            // Act
            var (success, orderId, total, message) = await service.CreateOrderAsync(order, items);

            // Assert
            Assert.False(success);
            Assert.Null(orderId);
            Assert.NotEmpty(message ?? string.Empty);
        }

        [Fact]
        public async Task GenerateDownloadTokensForOrderAsync_Should_Create_Tokens_For_All_Items()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var downloadServiceMock = new Mock<IDownloadService>();
            downloadServiceMock.Setup(x => x.GenerateTokenForOrderItemAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync("token123");

            var loggerMock = new Mock<ILogger<OrderService>>();
            var service = new OrderService(context, downloadServiceMock.Object, loggerMock.Object);

            var order = new ShopOrder { Id = "order1", OrderNumber = "ORD1" };
            var items = new List<ShopOrderItem>
            {
                new ShopOrderItem { Id = "item1", OrderId = "order1" },
                new ShopOrderItem { Id = "item2", OrderId = "order1" }
            };

            context.ShopOrders.Add(order);
            foreach (var item in items) context.ShopOrderItems.Add(item);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GenerateDownloadTokensForOrderAsync("order1");

            // Assert
            Assert.True(result);
            downloadServiceMock.Verify(x => x.GenerateTokenForOrderItemAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(2));
        }
    }
}
