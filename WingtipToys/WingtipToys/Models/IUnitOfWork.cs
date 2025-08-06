using System;
using System.Threading.Tasks;

namespace WingtipToys.Models
{
    /// <summary>
    /// Unit of Work pattern interface for managing transactions
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        // Repository properties
        IProductRepository Products { get; }
        ICategoryRepository Categories { get; }
        ICartRepository CartItems { get; }
        IOrderRepository Orders { get; }
        IRepository<OrderDetail> OrderDetails { get; }

        // Transaction methods
        int SaveChanges();
        Task<int> SaveChangesAsync();
        
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
        
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}