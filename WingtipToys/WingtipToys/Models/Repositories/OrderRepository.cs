using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using WingtipToys.Models.Exceptions;

namespace WingtipToys.Models.Repositories
{
    /// <summary>
    /// Order repository implementation
    /// </summary>
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(ProductContext context) : base(context) { }

        public IEnumerable<Order> GetOrdersByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return new List<Order>();

            return _dbSet.Include(o => o.OrderDetails)
                        .Where(o => o.Username == username)
                        .OrderByDescending(o => o.OrderDate)
                        .ToList();
        }

        public async Task<IEnumerable<Order>> GetOrdersByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return new List<Order>();

            return await _dbSet.Include(o => o.OrderDetails)
                              .Where(o => o.Username == username)
                              .OrderByDescending(o => o.OrderDate)
                              .ToListAsync()
                              .ConfigureAwait(false);
        }

        public Order GetOrderWithDetails(int orderId)
        {
            var order = _dbSet.Include(o => o.OrderDetails)
                             .ThenInclude(od => od.Product)
                             .SingleOrDefault(o => o.OrderId == orderId);

            if (order == null)
                throw new OrderException($"Order with ID {orderId} was not found.", orderId);

            return order;
        }

        public async Task<Order> GetOrderWithDetailsAsync(int orderId)
        {
            var order = await _dbSet.Include(o => o.OrderDetails)
                                   .ThenInclude(od => od.Product)
                                   .SingleOrDefaultAsync(o => o.OrderId == orderId)
                                   .ConfigureAwait(false);

            if (order == null)
                throw new OrderException($"Order with ID {orderId} was not found.", orderId);

            return order;
        }

        public IEnumerable<Order> GetUnshippedOrders()
        {
            return _dbSet.Include(o => o.OrderDetails)
                        .Where(o => !o.HasBeenShipped)
                        .OrderBy(o => o.OrderDate)
                        .ToList();
        }

        public async Task<IEnumerable<Order>> GetUnshippedOrdersAsync()
        {
            return await _dbSet.Include(o => o.OrderDetails)
                              .Where(o => !o.HasBeenShipped)
                              .OrderBy(o => o.OrderDate)
                              .ToListAsync()
                              .ConfigureAwait(false);
        }

        public void MarkOrderAsShipped(int orderId)
        {
            var order = _dbSet.Find(orderId);
            if (order == null)
                throw new OrderException($"Order with ID {orderId} was not found.", orderId);

            order.HasBeenShipped = true;
        }

        public async Task MarkOrderAsShippedAsync(int orderId)
        {
            var order = await _dbSet.FindAsync(orderId).ConfigureAwait(false);
            if (order == null)
                throw new OrderException($"Order with ID {orderId} was not found.", orderId);

            order.HasBeenShipped = true;
        }
    }
}