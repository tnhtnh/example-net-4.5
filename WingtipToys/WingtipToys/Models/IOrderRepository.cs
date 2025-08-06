using System.Collections.Generic;
using System.Threading.Tasks;

namespace WingtipToys.Models
{
    /// <summary>
    /// Order-specific repository interface
    /// </summary>
    public interface IOrderRepository : IRepository<Order>
    {
        // Order-specific methods
        IEnumerable<Order> GetOrdersByUsername(string username);
        Task<IEnumerable<Order>> GetOrdersByUsernameAsync(string username);
        
        Order GetOrderWithDetails(int orderId);
        Task<Order> GetOrderWithDetailsAsync(int orderId);
        
        IEnumerable<Order> GetUnshippedOrders();
        Task<IEnumerable<Order>> GetUnshippedOrdersAsync();
        
        void MarkOrderAsShipped(int orderId);
        Task MarkOrderAsShippedAsync(int orderId);
    }
}