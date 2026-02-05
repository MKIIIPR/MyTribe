using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tribe.Data;
using Tribe.Controller.Services;
using static Tribe.Bib.ShopRelated.ShopStruckture;

namespace Tribe.Tests.Services
{
    public class DownloadServiceTests
    {
        private ShopDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ShopDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;
            return new ShopDbContext(options);
        }

        [Fact]
        public async Task GenerateTokenForOrderItemAsync_Should_Create_Valid_Token()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var loggerMock = new Mock<ILogger<DownloadService>>();
            var service = new DownloadService(context, loggerMock.Object);

            var orderItem = new ShopOrderItem { Id = Guid.NewGuid().ToString(), OrderId = "order1", ProductId = "product1", ProductTitle = "Test" };
            context.ShopOrderItems.Add(orderItem);
            await context.SaveChangesAsync();

            // Act
            var token = await service.GenerateTokenForOrderItemAsync(orderItem.Id, 7, 3);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);

            var updatedItem = context.ShopOrderItems.Find(orderItem.Id);
            Assert.NotNull(updatedItem);
            Assert.Equal(token, updatedItem.DownloadToken);
            Assert.NotNull(updatedItem.DownloadExpiry);
        }

        [Fact]
        public async Task ValidateAndGetDownloadUrlAsync_Should_Fail_For_Expired_Token()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var loggerMock = new Mock<ILogger<DownloadService>>();
            var service = new DownloadService(context, loggerMock.Object);

            var order = new ShopOrder { Id = "order1", OrderNumber = "ORD1", Status = OrderStatus.Confirmed, PaidAt = DateTime.UtcNow };
            var orderItem = new ShopOrderItem 
            { 
                Id = Guid.NewGuid().ToString(), 
                OrderId = "order1", 
                ProductId = "product1", 
                ProductTitle = "Test",
                DownloadToken = "test_token",
                DownloadExpiry = DateTime.UtcNow.AddDays(-1) // Expired
            };

            context.ShopOrders.Add(order);
            context.ShopOrderItems.Add(orderItem);
            await context.SaveChangesAsync();

            // Act
            var (success, url, reason) = await service.ValidateAndGetDownloadUrlAsync("test_token");

            // Assert
            Assert.False(success);
            Assert.Equal("token expired", reason);
        }

        [Fact]
        public async Task ValidateAndGetDownloadUrlAsync_Should_Increment_Download_Count()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var loggerMock = new Mock<ILogger<DownloadService>>();
            var service = new DownloadService(context, loggerMock.Object);

            var product = new ImageProduct { Id = "product1", Title = "Test", Price = 10, DownloadUrl = "https://example.com/download.zip" };
            var order = new ShopOrder { Id = "order1", OrderNumber = "ORD1", Status = OrderStatus.Confirmed, PaidAt = DateTime.UtcNow };
            var orderItem = new ShopOrderItem 
            { 
                Id = Guid.NewGuid().ToString(), 
                OrderId = "order1", 
                ProductId = "product1", 
                ProductTitle = "Test",
                DownloadToken = "test_token",
                DownloadExpiry = DateTime.UtcNow.AddDays(7),
                DownloadCount = 0,
                MaxDownloads = 5
            };

            context.ShopProducts.Add(product);
            context.ShopOrders.Add(order);
            context.ShopOrderItems.Add(orderItem);
            await context.SaveChangesAsync();

            // Act
            var (success, url, reason) = await service.ValidateAndGetDownloadUrlAsync("test_token");

            // Assert
            Assert.True(success);
            Assert.NotNull(url);

            var updatedItem = context.ShopOrderItems.Find(orderItem.Id);
            Assert.Equal(1, updatedItem?.DownloadCount);
        }
    }
}
