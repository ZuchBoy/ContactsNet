using Contacts.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Contacts.Controllers {

	[ApiController]
	[Route("[controller]")]
	public class CategoryController : ControllerBase {
		private ICategoryService _categoryService;

        // Constructor that injects the category service dependency.
        public CategoryController(ICategoryService categoryService) {
			_categoryService = categoryService;
		}

		[HttpGet]
		[AllowAnonymous]
        // Retrieves all categories.
        public async Task<IActionResult> GetAll() {
			var categories = await _categoryService.GetAll();
			return Ok(categories);
		}

		[HttpGet("{id}")]
		[AllowAnonymous]
        // Retrieves a category by its ID.
        public async Task<IActionResult> GetCategory(int id) {
			var category = await _categoryService.GetById(id);
			if (category == null) {
				return NotFound();
			}

			return Ok(category);
		}

		[HttpGet("/subcategory")]
		[AllowAnonymous]
        // Retrieves all subcategories.
        public async Task<IActionResult> GetAllSubcategories() {
			var subcategories = await _categoryService.GetAllSubcategories();
			return Ok(subcategories);
		}

		[HttpGet("{categoryId}/subcategories")]
		[AllowAnonymous]
        // Retrieves all subcategories for a given category ID.
        public async Task<IActionResult> GetSubcategoriesByCategoryId(int categoryId) {
			var subcategories = await _categoryService.GetSubcategoriesByCategoryId(categoryId);
			return Ok(subcategories);
		}

		[HttpGet("subcategory/{subcategoryId}")]
		[AllowAnonymous]
        // Retrieves a subcategory by its ID.
        public async Task<IActionResult> GetSubcategoryById(int subcategoryId) {
			var subcategory = await _categoryService.GetSubcategoryById(subcategoryId);
			return Ok(subcategory);
		}

		[HttpPost("{categoryId}/subcategories")]
		[AllowAnonymous]
        // Adds a new subcategory to a category.
        public async Task<IActionResult> AddSubcategory(int categoryId, [FromBody] Models.Subcategory subcategory) {
			var result = await _categoryService.AddSubcategory(categoryId, subcategory);
			if (!result) {
				return BadRequest();
			}
			return Ok();
		}
	}
}
