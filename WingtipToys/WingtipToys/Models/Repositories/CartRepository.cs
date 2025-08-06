using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using WingtipToys.Models.Exceptions;

namespace WingtipToys.Models.Repositories
{
    /// <summary>
    /// Shopping cart repository implementation
    /// </summary>
    public class CartRepository : Repository<CartItem>, ICartRepository
    {
        public CartRepository(ProductContext context) : base(context) { }

        public IEnumerable<CartItem> GetCartItems(string cartId)
        {
            if (string.IsNullOrWhiteSpace(cartId))
                return new List<CartItem>();

            return _dbSet.Include(c => c.Product)
                        .Where(c => c.CartId == cartId)
                        .ToList();
        }

        public async Task<IEnumerable<CartItem>> GetCartItemsAsync(string cartId)
        {
            if (string.IsNullOrWhiteSpace(cartId))
                return new List<CartItem>();

            return await _dbSet.Include(c => c.Product)
                              .Where(c => c.CartId == cartId)
                              .ToListAsync()
                              .ConfigureAwait(false);
        }

        public CartItem GetCartItem(string cartId, int productId)
        {
            if (string.IsNullOrWhiteSpace(cartId))
                return null;

            return _dbSet.Include(c => c.Product)
                        .SingleOrDefault(c => c.CartId == cartId && c.ProductId == productId);
        }

        public async Task<CartItem> GetCartItemAsync(string cartId, int productId)
        {
            if (string.IsNullOrWhiteSpace(cartId))
                return null;

            return await _dbSet.Include(c => c.Product)
                              .SingleOrDefaultAsync(c => c.CartId == cartId && c.ProductId == productId)
                              .ConfigureAwait(false);
        }

        public decimal GetCartTotal(string cartId)
        {
            if (string.IsNullOrWhiteSpace(cartId))
                return 0;

            var total = _dbSet.Where(c => c.CartId == cartId)
                             .Sum(c => (decimal?)(c.Quantity * c.Product.UnitPrice));

            return total ?? 0;
        }

        public async Task<decimal> GetCartTotalAsync(string cartId)
        {
            if (string.IsNullOrWhiteSpace(cartId))
                return 0;

            var total = await _dbSet.Where(c => c.CartId == cartId)
                                   .SumAsync(c => (decimal?)(c.Quantity * c.Product.UnitPrice))
                                   .ConfigureAwait(false);

            return total ?? 0;
        }

        public int GetCartItemCount(string cartId)
        {
            if (string.IsNullOrWhiteSpace(cartId))
                return 0;

            var count = _dbSet.Where(c => c.CartId == cartId)
                             .Sum(c => (int?)c.Quantity);

            return count ?? 0;
        }

        public async Task<int> GetCartItemCountAsync(string cartId)
        {
            if (string.IsNullOrWhiteSpace(cartId))
                return 0;

            var count = await _dbSet.Where(c => c.CartId == cartId)
                                   .SumAsync(c => (int?)c.Quantity)
                                   .ConfigureAwait(false);

            return count ?? 0;
        }

        public void EmptyCart(string cartId)
        {
            if (string.IsNullOrWhiteSpace(cartId))
                return;

            var cartItems = _dbSet.Where(c => c.CartId == cartId).ToList();
            if (cartItems.Any())
            {
                _dbSet.RemoveRange(cartItems);
            }
        }

        public async Task EmptyCartAsync(string cartId)
        {
            if (string.IsNullOrWhiteSpace(cartId))
                return;

            var cartItems = await _dbSet.Where(c => c.CartId == cartId)
                                       .ToListAsync()
                                       .ConfigureAwait(false);
            if (cartItems.Any())
            {
                _dbSet.RemoveRange(cartItems);
            }
        }

        public void MigrateCart(string oldCartId, string newCartId)
        {
            if (string.IsNullOrWhiteSpace(oldCartId) || string.IsNullOrWhiteSpace(newCartId))
                throw new CartException("Cart IDs cannot be null or empty for migration");

            var cartItems = _dbSet.Where(c => c.CartId == oldCartId).ToList();
            foreach (var item in cartItems)
            {
                item.CartId = newCartId;
            }
        }

        public async Task MigrateCartAsync(string oldCartId, string newCartId)
        {
            if (string.IsNullOrWhiteSpace(oldCartId) || string.IsNullOrWhiteSpace(newCartId))
                throw new CartException("Cart IDs cannot be null or empty for migration");

            var cartItems = await _dbSet.Where(c => c.CartId == oldCartId)
                                       .ToListAsync()
                                       .ConfigureAwait(false);
            foreach (var item in cartItems)
            {
                item.CartId = newCartId;
            }
        }
    }
}