using ContactsFront.Api;
using ContactsFront.Components.Pages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContactsFront.Services {
	public class CategoryService {
		private readonly HttpClient _http;
		public CategoryService(HttpClient httpClient, AuthService authService, ContactsApiClient contactsApiClient) {
			_http = httpClient;
		}

		// GET /Category
        // Retrieves all categories from the backend API.
        public async Task<List<Category>?> GetCategoriesAsync() {
			return await _http.GetFromJsonAsync<List<Category>>("/Category");
		}

		// GET /Category/{id}
        // Retrieves a category by its ID from the backend API.
        public Task<Category?> GetCategoryByIdAsync(int categoryId) {
			return _http.GetFromJsonAsync<Category>($"/Category/{categoryId}");
		}

		// GET /Category/{id}/subcategories
        // Retrieves all subcategories for a given category ID from the backend API.
        public async Task<List<Subcategory>?> GetSubcategoriesAsync(int categoryId) {
			return await _http.GetFromJsonAsync<List<Subcategory>>($"/Category/{categoryId}/subcategories");
		}

		// GET /Category/subcategory/
        // Retrieves all subcategories from the backend API.
        public async Task<List<Subcategory>?> GetAllSubcategories() {
			return await _http.GetFromJsonAsync<List<Subcategory>>($"/subcategory/");
		}

		// GET /Category/subcategory/{id}
        // Retrieves a subcategory by its ID from the backend API.
        public async Task<Subcategory?> GetSubcategoryById(int subcategoryId) {
			return await _http.GetFromJsonAsync<Subcategory>($"/Category/subcategory/{subcategoryId}");
		}

		// POST /Category/{id}/subcategories
        // Saves a new subcategory to the backend API for a given category.
        public async Task<HttpResponseMessage> SaveSubcategory(Subcategory subcategory) {
			return await _http.PostAsJsonAsync($"/Category/{subcategory.CategoryId}/subcategories", subcategory);
		}

	}
}
