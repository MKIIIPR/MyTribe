using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Tribe.Controller.ShopController;
using Tribe.Data;
using Xunit;
using static Tribe.Bib.ShopRelated.ShopStruckture;

namespace Tribe.Tribe.Tests.Services
{
    public class ShopCategoriesControllerTests
    {
        private ShopDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ShopDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ShopDbContext(options);
        }

        private ShopCategoriesController CreateController(ShopDbContext context, string userId)
        {
            var logger = NullLogger<ShopCategoriesController>.Instance;
            var controller = new ShopCategoriesController(context, logger);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) }, "TestAuth"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            return controller;
        }

        [Fact]
        public async Task CreateCategory_AddsCategory_ReturnsCreated()
        {
            var ctx = CreateInMemoryContext();
            var controller = CreateController(ctx, "user-1");

            var category = new ShopCategory
            {
                Name = "TestCat",
                ColorHex = "#ffffff"
            };

            var result = await controller.CreateCategory(category);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            var returned = Assert.IsType<ShopCategory>(created.Value);
            Assert.Equal("TestCat", returned.Name);

            var saved = await ctx.ShopCategories.FindAsync(returned.Id);
            Assert.NotNull(saved);
            Assert.Equal("user-1", saved.CreatorProfileId);
        }

        [Fact]
        public async Task GetMyCategories_ReturnsOnlyUserCategories()
        {
            var ctx = CreateInMemoryContext();
            var userId = "creator-123";
            ctx.ShopCategories.Add(new ShopCategory { Id = "c1", Name = "A", CreatorProfileId = userId });
            ctx.ShopCategories.Add(new ShopCategory { Id = "c2", Name = "B", CreatorProfileId = "other" });
            await ctx.SaveChangesAsync();

            var controller = CreateController(ctx, userId);

            var result = await controller.GetMyCategories();

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<System.Collections.Generic.List<ShopCategory>>(ok.Value);
            Assert.Single(list);
            Assert.Equal("A", list.First().Name);
        }
    }
}
