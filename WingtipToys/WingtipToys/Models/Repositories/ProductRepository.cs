using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using WingtipToys.Models.Exceptions;

namespace WingtipToys.Models.Repositories
{
    /// <summary>
    /// Product repository implementation
    /// </summary>
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ProductContext context) : base(context) { }

        public IEnumerable<Product> GetProductsByCategory(int categoryId)
        {
            return _dbSet.Where(p => p.CategoryID == categoryId)
                        .Include(p => p.Category)
                        .ToList();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _dbSet.Where(p => p.CategoryID == categoryId)
                              .Include(p => p.Category)
                              .ToListAsync()
                              .ConfigureAwait(false);
        }

        public IEnumerable<Product> GetProductsByName(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
                return new List<Product>();

            return _dbSet.Where(p => p.ProductName.Contains(productName))
                        .Include(p => p.Category)
                        .ToList();
        }

        public async Task<IEnumerable<Product>> GetProductsByNameAsync(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
                return new List<Product>();

            return await _dbSet.Where(p => p.ProductName.Contains(productName))
                              .Include(p => p.Category)
                              .ToListAsync()
                              .ConfigureAwait(false);
        }

        public IEnumerable<Product> GetFeaturedProducts()
        {
            // For now, return all products. This could be enhanced with a Featured flag
            return _dbSet.Include(p => p.Category)
                        .Take(10)
                        .ToList();
        }

        public async Task<IEnumerable<Product>> GetFeaturedProductsAsync()
        {
            return await _dbSet.Include(p => p.Category)
                              .Take(10)
                              .ToListAsync()
                              .ConfigureAwait(false);
        }

        public Product GetProductByName(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
                throw new ProductNotFoundException(productName);

            var product = _dbSet.Include(p => p.Category)
                               .SingleOrDefault(p => p.ProductName == productName);

            if (product == null)
                throw new ProductNotFoundException(productName);

            return product;
        }

        public async Task<Product> GetProductByNameAsync(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
                throw new ProductNotFoundException(productName);

            var product = await _dbSet.Include(p => p.Category)
                                     .SingleOrDefaultAsync(p => p.ProductName == productName)
                                     .ConfigureAwait(false);

            if (product == null)
                throw new ProductNotFoundException(productName);

            return product;
        }

        public override T GetById(int id)
        {
            var product = _dbSet.Include(p => p.Category)
                               .SingleOrDefault(p => p.ProductID == id);

            if (product == null)
                throw new ProductNotFoundException(id);

            return product as T;
        }

        public override async Task<T> GetByIdAsync(int id)
        {
            var product = await _dbSet.Include(p => p.Category)
                                     .SingleOrDefaultAsync(p => p.ProductID == id)
                                     .ConfigureAwait(false);

            if (product == null)
                throw new ProductNotFoundException(id);

            return product as T;
        }
    }
}