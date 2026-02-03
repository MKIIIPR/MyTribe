using Microsoft.EntityFrameworkCore;
using Tribe.Data;
using static Tribe.Bib.ShopRelated.ShopStruckture;

namespace TribeApp.Repositories
{

        public interface IProductRepository
        {
            Task<List<ShopProduct>> GetCreatorProducts(string creatorProfileId);
            Task<ShopProduct?> GetProductByIdAsync(string productId);
            Task<string> CreateProductAsync<T>(T product) where T : ShopProduct;
            Task<bool> UpdateProductAsync(ShopProduct product);
            Task<bool> DeleteProductAsync(string productId);
        }
   

    public class ProductRepository : IProductRepository
        {
            private readonly ShopDbContext _context;

            public ProductRepository(ShopDbContext context)
            {
                _context = context;
            }

            public async Task<List<ShopProduct>> GetCreatorProducts(string creatorProfileId)
            {
                return await _context.Products
                    .Where(p => p.CreatorProfileId == creatorProfileId)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();
            }

            public async Task<ShopProduct?> GetProductByIdAsync(string productId)
            {
                return await _context.Products.FindAsync(productId);
            }

            public async Task<string> CreateProductAsync<T>(T product) where T : ShopProduct
            {
                product.Id = Guid.NewGuid().ToString();
                product.CreatedAt = DateTime.UtcNow;
                product.UpdatedAt = DateTime.UtcNow;

                await _context.Set<T>().AddAsync(product);
                await _context.SaveChangesAsync();

                return product.Id;
            }

            public async Task<bool> UpdateProductAsync(ShopProduct product)
            {
                // Finden des existierenden Produkts im DbContext, um Änderungen zu verfolgen
                var existingProduct = await _context.Products.FindAsync(product.Id);
                if (existingProduct == null)
                {
                    return false;
                }

                _context.Entry(existingProduct).CurrentValues.SetValues(product);
                existingProduct.UpdatedAt = DateTime.UtcNow;

                try
                {
                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (DbUpdateConcurrencyException)
                {
                    return false;
                }
            }

            public async Task<bool> DeleteProductAsync(string productId)
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    return false;
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return true;
            }
        
    }
}