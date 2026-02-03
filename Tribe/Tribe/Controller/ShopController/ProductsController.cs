using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TribeApp.Repositories;
using static Tribe.Bib.ShopRelated.ShopStruckture;

namespace TribeApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<IActionResult> GetMyProducts()
        {
            var userId = GetUserId();
            var products = await _productRepository.GetCreatorProducts(userId);
            return Ok(products);
        }

        // GET: api/Products/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(string id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var userId = GetUserId();
            if (product.CreatorProfileId != userId)
            {
                return Forbid();
            }

            return Ok(product);
        }

        // POST: api/Products
        // Erstellt ein neues Produkt. Der Typ wird automatisch erkannt.
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ShopProduct product)
        {
            product.CreatorProfileId = GetUserId();
            var productId = await _productRepository.CreateProductAsync(product);
            return CreatedAtAction(nameof(GetProduct), new { id = productId }, product);
        }

        // PUT: api/Products/{id}
        // Aktualisiert ein Produkt. Der Typ wird automatisch erkannt.
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(string id, [FromBody] ShopProduct product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            var userId = GetUserId();
            if (product.CreatorProfileId != userId)
            {
                return Forbid();
            }

            var result = await _productRepository.UpdateProductAsync(product);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/Products/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var userId = GetUserId();
            if (product.CreatorProfileId != userId)
            {
                return Forbid();
            }

            var result = await _productRepository.DeleteProductAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}