using System.Collections.Generic;
using System.Threading.Tasks;

namespace WingtipToys.Models
{
    /// <summary>
    /// Product-specific repository interface
    /// </summary>
    public interface IProductRepository : IRepository<Product>
    {
        // Product-specific methods
        IEnumerable<Product> GetProductsByCategory(int categoryId);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        
        IEnumerable<Product> GetProductsByName(string productName);
        Task<IEnumerable<Product>> GetProductsByNameAsync(string productName);
        
        IEnumerable<Product> GetFeaturedProducts();
        Task<IEnumerable<Product>> GetFeaturedProductsAsync();
        
        Product GetProductByName(string productName);
        Task<Product> GetProductByNameAsync(string productName);
    }
}