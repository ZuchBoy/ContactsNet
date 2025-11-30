using Contacts.Interfaces;
using Contacts.Models;
using Microsoft.EntityFrameworkCore;

namespace Contacts.Services {
	public class CategoryService : ICategoryService {
		private readonly ContactContext _dbContext;
		
        /// <summary>
		/// Constructor that injects the database context dependency.
		/// </summary>
		/// <param name="contactContext"></param>
        public CategoryService(ContactContext contactContext) {
			_dbContext = contactContext;
		}

        /// <summary>
		/// Adds a new subcategory to a category if it does not already exist.
		/// </summary>
		/// <param name="categoryId"></param>
		/// <param name="subcategory"></param>
		/// <returns></returns>
        public async Task<bool> AddSubcategory(int categoryId, Subcategory subcategory) {
			
			var existedCategory = await _dbContext.Subcategories.Where(s => s.Name.Trim().ToLower() == subcategory.Name.Trim().ToLower() && s.CategoryId == categoryId).FirstOrDefaultAsync();
			if(existedCategory != null) {
				return true;
			}
			_dbContext.Subcategories.Add(subcategory);
			return await _dbContext.SaveChangesAsync() > 0;
		}

        /// <summary>
		/// Retrieves all categories from the database.
		/// </summary>
		/// <returns></returns>
        public async Task<IEnumerable<Category>> GetAll() {
			return await _dbContext.Categories.ToListAsync();
		}

        /// <summary>
		/// Retrieves a subcategory by its ID from the database.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
        public async Task<Subcategory> GetSubcategoryById(int id) {
			return await _dbContext.Subcategories
				.Where(sc => sc.Id == id)
				.FirstOrDefaultAsync();
		}

        /// <summary>
		/// Retrieves all subcategories for a given category ID from the database.
		/// </summary>
		/// <param name="categoryId"></param>
		/// <returns></returns>
        public async Task<IEnumerable<Subcategory>> GetSubcategoriesByCategoryId(int categoryId) {
			return await _dbContext.Subcategories
				.Where(sc => sc.CategoryId == categoryId)
				.ToListAsync();
		}

        /// <summary>
		/// Retrieves a category by its ID from the database.
		/// </summary>
		/// <param name="categoryId"></param>
		/// <returns></returns>
        public async Task<Category> GetById(int categoryId) {
			return await _dbContext.Categories
				.Where(c => c.Id == categoryId)
				.FirstOrDefaultAsync();
		}

        /// <summary>
		/// Retrieves all subcategories from the database.
		/// </summary>
		/// <returns></returns>
        public async Task<IEnumerable<Subcategory>> GetAllSubcategories() {
			return await _dbContext.Subcategories.ToListAsync();
		}
	}
}
