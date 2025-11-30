using Contacts.Models;

namespace Contacts.Interfaces {
	public interface ICategoryService {
        /// <summary>
        /// Retrieves all categories.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Category>> GetAll();

        /// <summary>
        /// Retrieves all subcategories.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Subcategory>> GetAllSubcategories();

        /// <summary>
        /// Retrieves a category by its ID.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        Task<Category> GetById(int categoryId);

        /// <summary>
        /// Retrieves all subcategories for a given category ID.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        Task<IEnumerable<Subcategory>> GetSubcategoriesByCategoryId(int categoryId);
        
        /// <summary>
        /// Adds a new subcategory to a category.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="subcategory"></param>
        /// <returns></returns>
        Task<bool> AddSubcategory(int categoryId, Subcategory subcategory);
        
        /// <summary>
        /// Retrieves a subcategory by its ID.
        /// </summary>
        /// <param name="subcategoryId"></param>
        /// <returns></returns>
        Task<Subcategory>GetSubcategoryById(int subcategoryId);
	}
}
