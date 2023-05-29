using trackingApi.GenericRepository;

namespace trackingApi.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
        Task<int> SaveChangesAsync();
        void Commit();
        void Rollback();
        void CreateTransaction();
    }
}
