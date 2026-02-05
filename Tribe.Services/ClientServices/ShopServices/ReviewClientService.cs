using Tribe.Client.Services;
using static Tribe.Bib.ShopRelated.ShopStruckture;

namespace Tribe.Services.ClientServices.ShopServices
{
    public interface IReviewClientService
    {
        Task<List<ProductReview>> GetProductReviewsAsync(string productId);
        Task<bool> PostReviewAsync(string productId, ProductReview review);
    }

    public class ReviewClientService : IReviewClientService
    {
        private readonly IApiService _apiService;

        public ReviewClientService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<List<ProductReview>> GetProductReviewsAsync(string productId)
        {
            var reviews = await _apiService.GetAsync<List<ProductReview>>($"api/shop/reviews/product/{productId}");
            return reviews ?? new List<ProductReview>();
        }

        public async Task<bool> PostReviewAsync(string productId, ProductReview review)
        {
            // Explicitly specify request and response types for PostAsync
            var result = await _apiService.PostAsync<ProductReview, bool>($"api/shop/reviews/product/{productId}", review);
            return result;
        }
    }
}
