using System.Collections.Generic;
using System.Threading.Tasks;

namespace WingtipToys.Models
{
    /// <summary>
    /// Shopping cart-specific repository interface
    /// </summary>
    public interface ICartRepository : IRepository<CartItem>
    {
        // Cart-specific methods
        IEnumerable<CartItem> GetCartItems(string cartId);
        Task<IEnumerable<CartItem>> GetCartItemsAsync(string cartId);
        
        CartItem GetCartItem(string cartId, int productId);
        Task<CartItem> GetCartItemAsync(string cartId, int productId);
        
        decimal GetCartTotal(string cartId);
        Task<decimal> GetCartTotalAsync(string cartId);
        
        int GetCartItemCount(string cartId);
        Task<int> GetCartItemCountAsync(string cartId);
        
        void EmptyCart(string cartId);
        Task EmptyCartAsync(string cartId);
        
        void MigrateCart(string oldCartId, string newCartId);
        Task MigrateCartAsync(string oldCartId, string newCartId);
    }
}