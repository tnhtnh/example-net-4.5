using System.Collections.Generic;
using System.Threading.Tasks;

namespace WingtipToys.Models
{
    /// <summary>
    /// Category-specific repository interface
    /// </summary>
    public interface ICategoryRepository : IRepository<Category>
    {
        // Category-specific methods
        Category GetCategoryByName(string categoryName);
        Task<Category> GetCategoryByNameAsync(string categoryName);
        
        IEnumerable<Category> GetCategoriesWithProducts();
        Task<IEnumerable<Category>> GetCategoriesWithProductsAsync();
    }
}