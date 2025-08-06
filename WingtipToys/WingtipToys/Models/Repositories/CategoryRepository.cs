using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace WingtipToys.Models.Repositories
{
    /// <summary>
    /// Category repository implementation
    /// </summary>
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ProductContext context) : base(context) { }

        public Category GetCategoryByName(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
                return null;

            return _dbSet.SingleOrDefault(c => c.CategoryName == categoryName);
        }

        public async Task<Category> GetCategoryByNameAsync(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
                return null;

            return await _dbSet.SingleOrDefaultAsync(c => c.CategoryName == categoryName)
                              .ConfigureAwait(false);
        }

        public IEnumerable<Category> GetCategoriesWithProducts()
        {
            return _dbSet.Include(c => c.Products)
                        .Where(c => c.Products.Any())
                        .ToList();
        }

        public async Task<IEnumerable<Category>> GetCategoriesWithProductsAsync()
        {
            return await _dbSet.Include(c => c.Products)
                              .Where(c => c.Products.Any())
                              .ToListAsync()
                              .ConfigureAwait(false);
        }

        public override IEnumerable<T> GetAll()
        {
            return _dbSet.Include(c => c.Products)
                        .ToList() as IEnumerable<T>;
        }

        public override async Task<IEnumerable<T>> GetAllAsync()
        {
            var categories = await _dbSet.Include(c => c.Products)
                                        .ToListAsync()
                                        .ConfigureAwait(false);
            return categories as IEnumerable<T>;
        }
    }
}