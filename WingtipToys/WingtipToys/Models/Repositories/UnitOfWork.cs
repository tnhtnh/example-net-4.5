using System;
using System.Data.Entity;
using System.Threading.Tasks;
using WingtipToys.Models.Repositories;

namespace WingtipToys.Models.Repositories
{
    /// <summary>
    /// Unit of Work implementation for managing transactions and repositories
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ProductContext _context;
        private DbContextTransaction _transaction;

        // Repository instances
        private IProductRepository _products;
        private ICategoryRepository _categories;
        private ICartRepository _cartItems;
        private IOrderRepository _orders;
        private IRepository<OrderDetail> _orderDetails;

        public UnitOfWork(ProductContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Repository properties with lazy initialization
        public IProductRepository Products
        {
            get { return _products ?? (_products = new ProductRepository(_context)); }
        }

        public ICategoryRepository Categories
        {
            get { return _categories ?? (_categories = new CategoryRepository(_context)); }
        }

        public ICartRepository CartItems
        {
            get { return _cartItems ?? (_cartItems = new CartRepository(_context)); }
        }

        public IOrderRepository Orders
        {
            get { return _orders ?? (_orders = new OrderRepository(_context)); }
        }

        public IRepository<OrderDetail> OrderDetails
        {
            get { return _orderDetails ?? (_orderDetails = new Repository<OrderDetail>(_context)); }
        }

        // Transaction methods
        public int SaveChanges()
        {
            try
            {
                return _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Log the exception here if logging is available
                throw new InvalidOperationException("An error occurred while saving changes to the database.", ex);
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Log the exception here if logging is available
                throw new InvalidOperationException("An error occurred while saving changes to the database.", ex);
            }
        }

        public void BeginTransaction()
        {
            if (_transaction != null)
                throw new InvalidOperationException("A transaction is already in progress.");

            _transaction = _context.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            if (_transaction == null)
                throw new InvalidOperationException("No transaction is in progress.");

            try
            {
                SaveChanges();
                _transaction.Commit();
            }
            catch
            {
                _transaction.Rollback();
                throw;
            }
            finally
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }

        public void RollbackTransaction()
        {
            if (_transaction == null)
                throw new InvalidOperationException("No transaction is in progress.");

            try
            {
                _transaction.Rollback();
            }
            finally
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction != null)
                throw new InvalidOperationException("A transaction is already in progress.");

            _transaction = _context.Database.BeginTransaction();
            await Task.CompletedTask.ConfigureAwait(false);
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("No transaction is in progress.");

            try
            {
                await SaveChangesAsync().ConfigureAwait(false);
                _transaction.Commit();
            }
            catch
            {
                _transaction.Rollback();
                throw;
            }
            finally
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("No transaction is in progress.");

            try
            {
                _transaction.Rollback();
            }
            finally
            {
                _transaction.Dispose();
                _transaction = null;
                await Task.CompletedTask.ConfigureAwait(false);
            }
        }

        // Dispose pattern
        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _transaction?.Dispose();
                    _context?.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}