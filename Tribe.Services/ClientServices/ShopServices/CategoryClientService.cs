using Tribe.Client.Services;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Tribe.Services.ClientServices.ShopServices
{
    public interface ICategoryClientService
    {
        void SetAuthToken(string token);
        void RemoveAuthToken();
        Task<List<CategoryDto>> GetMyCategoriesAsync();
        Task<CategoryDto?> GetCategoryByIdAsync(string categoryId);
        Task<CategoryDto?> CreateCategoryAsync(CategoryDto category);
        Task<bool> UpdateCategoryAsync(CategoryDto category);
        Task<bool> DeleteCategoryAsync(string categoryId);
    }

    public class CategoryClientService : ICategoryClientService
    {
        private readonly IApiService _apiService;
        private const string CategoriesEndpoint = "api/shop/categories";

        public CategoryClientService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public void SetAuthToken(string token) => _apiService.SetAuthToken(token);
        public void RemoveAuthToken() => _apiService.RemoveAuthToken();

        public async Task<List<CategoryDto>> GetMyCategoriesAsync()
        {
            var result = await _apiService.GetAsync<List<Tribe.Bib.ShopRelated.ShopStruckture.ShopCategory>>(CategoriesEndpoint);
            if (result == null) return new List<CategoryDto>();
            return result.Select(c => new CategoryDto { Id = c.Id, Name = c.Name, Description = null }).ToList();
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(string categoryId)
        {
            return await _apiService.GetAsync<CategoryDto>($"{CategoriesEndpoint}/{categoryId}");
        }

        public async Task<CategoryDto?> CreateCategoryAsync(CategoryDto category)
        {
            return await _apiService.PostAsync<CategoryDto, CategoryDto>(CategoriesEndpoint, category);
        }

        public async Task<bool> UpdateCategoryAsync(CategoryDto category)
        {
            return await _apiService.PutAsync($"{CategoriesEndpoint}/{category.Id}", category);
        }

        public async Task<bool> DeleteCategoryAsync(string categoryId)
        {
            return await _apiService.DeleteAsync($"{CategoriesEndpoint}/{categoryId}");
        }
    }
}
