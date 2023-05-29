using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using trackingApi.Data;
using trackingApi.GenericRepository;

namespace trackingApi.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IssueDbContext _context;
        private bool _disposed;
        private IDbContextTransaction _transaction;

        public UnitOfWork(IssueDbContext context)
        {
            _context = context;
            _disposed = false;
        }

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            return new GenericRepository<TEntity>(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Commit()
        {
            _transaction?.Commit();
        }

        public void Rollback()
        {
            _transaction?.Rollback();
        }

        public void CreateTransaction()
        {
            _transaction = _context.Database.BeginTransaction();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _transaction?.Dispose();
                    _context.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
